﻿module Renamer

open System
open System.Collections.Generic
open System.Globalization
open System.IO
open System.Net
open System.Text
open Newtonsoft.Json.Linq
open Newtonsoft.Json

open TvCache
open ShowParser
open Types
open OtrBatchDecoder
open Episodes
open Framework

let cache = Cache()

let searchShow show = async {
            let client = getAuthorizedClient token
            let! response = client.AsyncDownloadString(Uri(apiUrl + "/search/series?name=" + show))
            return JsonConvert.DeserializeObject<SearchResponse>(response) }

let choose (options: (int*string) seq) =
    match options |> Seq.tryFind(fun (idx, title) -> title.StartsWith("Doctor Who (2005)")) with
    |Some (idx, title) -> idx
    |_ -> 0
    //match options with
    //|_ when Seq.length options = 1 -> 
    //        printfn "automatically chose first option"
    //        0
    //|_ ->   options
    //        |> Seq.iter(fun (idx, desc) -> printfn "[%d]: %s" idx desc)
    //        printf "Which option? "
    //        let input = Console.ReadLine()    
    //        printfn "your choice: %s" input
    //        input |> int

let rec findShow showName =

    let rec findShowInDb original current = async{
        try
            match cache.tryGetShow current with
            |true, mappedShow -> 
                            return Some(mappedShow)
            |false, _ ->    let! shows = searchShow current
                            let chosenIdx = choose (shows.data |> Seq.mapi(fun idx show -> (idx, show.seriesName)))
                            let chosenShow = shows.data.[chosenIdx]
                            cache.cacheShow original chosenShow      
                            return Some(chosenShow)
        with
            | :? WebException as ex ->  return None
                                        //printfn "Konnte zu '%s' keine Show ermitteln" current
                                        //printfn "Unbekannte Show, bitte Namen eingeben:"
                                        //let newShowName = Console.ReadLine()
                                        //if newShowName <> "x" then
                                        //    let! nextShow = findShowInDb original newShowName
                                        //    return nextShow
                                        //else
                                        //    return None
    }
    match showName with
    |None -> async { return  None }
    |Some n -> findShowInDb n n


let findEpisode file postprocess = async {
    cache.fillShowCache
    let parsedName = file |> parseShowName
    let! show = findShow parsedName
    match show with
    |None -> printfn "Keine Show gefunden! ('%s')" file
    |Some s ->  cache.loadEpisodes s
                let! episode = matchEpisode s file
                match episode with
                | None -> printfn "Episode konnte nicht zugeordnet werden"
                | Some ep -> 
                    let info = sprintf "%s %02dx%02d %s" s.seriesName ep.airedSeason.Value ep.airedEpisodeNumber.Value ep.episodeName
                    printfn "%s" info
                    postprocess file s ep
}

let getEpisode show file = async{
    cache.fillShowCache
    let parsedName = file |> parseShowName
    match show with
    |None -> 
        printfn "Keine Show gefunden! ('%s')" file
        return None
    |Some s ->  cache.loadEpisodes s
                let! episode = matchEpisode s file
                match episode with
                | None -> return None
                | Some ep -> return Some ep
}

let getEpisodeAsTask show file = (getEpisode show file) |> Async.StartAsTask

let cp newdir file show ep =
    

    let epName = sprintf "%s %02dx%02d %s" show.seriesName ep.airedSeason.Value ep.airedEpisodeNumber.Value ep.episodeName
    let validName = epName |> String.map(fun c -> 
        match (Path.GetInvalidFileNameChars() |> Seq.contains c) with
        |true -> '_'
        |_ -> c)
    let newPath = Path.Combine(newdir, show.seriesName, epName + ".avi")

    if not (Directory.Exists(Path.GetDirectoryName(newPath))) then
        (Directory.CreateDirectory(Path.GetDirectoryName(newPath))) |> ignore

    if File.Exists newPath then
        printfn "file already exists, moving '%s' to '%s'" file (newPath + "_2")
        File.Move(file, newPath + "_2")
    else
        printfn "moving '%s' to '%s'" file newPath
        File.Move(file, newPath)

let decoderOptions = DecoderOptions()
decoderOptions.AutoCut <- true
decoderOptions.ContinueWithoutCutlist <- true
decoderOptions.CreateDirectories <- true
decoderOptions.DecoderPath <- @"Z:\Downloads\OTR\DecoderCLI\otrdecoder.exe"
decoderOptions.Email <- File.ReadAllLines("./otrcredentials.cred").[0].Split('=').[1]
decoderOptions.FileExtensions <- [|".otrkey"|]
decoderOptions.ForceOverwrite <- true
decoderOptions.InputDirectory <- @"Z:\Downloads"
decoderOptions.OutputDirectory <- @"Z:\Downloads\fsdecoded"
decoderOptions.Password <- File.ReadAllLines("./otrcredentials.cred").[1].Split('=').[1]

let decoder = OtrBatchDecoder()
let files = decoder.Decode decoderOptions

let target = "z:\\downloads\\target\\fstest"
let postCopy = cp target 

login
// let files = Directory.GetFiles(@"z:\downloads\fsdecoded", "*.avi")

for ep in files |> Seq.sort do
    (findEpisode ep postCopy) |> Async.RunSynchronously