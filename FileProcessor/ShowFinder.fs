module ShowFinder

open Cache
open Newtonsoft.Json
open System.Net
open Types
open TvApi

type ShowFinder(c: Cache, a: TvApi) =
    let cache = c
    let api = a

    let searchShow show = async {
        let! showResponse = api.GetAsync("/search/series?name=" + show)
        return JsonConvert.DeserializeObject<SearchResponse>(showResponse) }

    member this.FindShow showName = 
        let rec findShowInDb original current = async{
            try
                match cache.TryGetShow current with
                |true, mappedShow -> return Some(mappedShow)
                |false, _ ->    let! shows = searchShow current
                                let chosenShow = match shows.data with
                                                 |s when s |> Seq.length > 0 -> Some(s |> Seq.head)
                                                 |_ -> None
                                match chosenShow with
                                |Some show -> cache.CacheShow original show      
                                              return Some(show)
                                |None -> return None
            with
                | :? WebException as ex ->  return None
        }
        match showName with
        |None -> async { return  None }
        |Some n -> findShowInDb n n