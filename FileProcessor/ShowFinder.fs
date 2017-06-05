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
        try
            let! showResponse = api.GetAsync("/search/series?name=" + show)
            return Some(JsonConvert.DeserializeObject<SearchResponse>(showResponse))
        with
            | :? WebException as ex -> return None
        }

    member this.FindShow showName = async {
        let (found, show) = cache.TryGetShow showName
        match (found, show) with
        |(true, _) -> return Some show
        |(false, _) -> 
            let! dbResult = searchShow showName
            match dbResult with
            |Some response ->   let dbShows = response.data |> List.ofSeq
                                match dbShows with
                                |[] -> return None
                                |[dbShow] -> 
                                    cache.CacheShow showName dbShow
                                    return Some dbShow
                                |_ -> return None
            |None -> return None
        }