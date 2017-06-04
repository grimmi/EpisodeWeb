module TvCache

open System
open System.Collections.Generic
open System.IO
open Types

type Cache() = 
    let showMap = Dictionary<string, Show>()
    let showEpisodes = Dictionary<Show, Episode seq>()

    let serializeEpisode episode =
        let serialized = sprintf "%d *** %d *** %s *** %s" episode.airedSeason.Value episode.airedEpisodeNumber.Value episode.firstAired episode.episodeName
        serialized

    let deserializeEpisode (line:string) =
        let parts = line.Split([|"***"|], StringSplitOptions.RemoveEmptyEntries)
        let season = parts.[0] |> int
        let epNr = parts.[1] |> int
        let aired = parts.[2].Trim()
        let name = parts.[3].Trim()
        { airedSeason = Nullable(season); airedEpisodeNumber = Nullable(epNr); firstAired = aired; episodeName = name; absoluteNumber = Nullable(0) }
        
    let deserializeShow (line: string) = 
        let parts = line.Split([|"->"|], StringSplitOptions.RemoveEmptyEntries)
        let parsedName = parts.[0].Trim()
        let values = parts.[1].Split([|"***"|], StringSplitOptions.RemoveEmptyEntries)
        (parsedName, { id = values.[1].Trim() |> int; seriesName = values.[0].Trim(); aliases = [|""|]})
        
    member this.cacheEpisodes episodes show = 
        showEpisodes.[show] <- episodes
        let path = sprintf "./%d.cache" show.id
        File.WriteAllLines(path, episodes 
            |> Seq.sortBy(fun e -> (e.airedSeason.Value, e.airedEpisodeNumber.Value)) 
            |> Seq.map serializeEpisode)

    member this.loadEpisodes show =
        let path = sprintf "./%d.cache" show.id
        if File.Exists path && not (showEpisodes.ContainsKey show) then
            let episodes =  File.ReadAllLines path
                            |> Seq.map deserializeEpisode
            showEpisodes.[show] <- episodes

    member this.fillShowCache =
        File.ReadAllLines("./shows.mapping")
        |> Seq.filter(fun line -> line |> String.length > 0)
        |> Seq.map deserializeShow
        |> Seq.iter(fun (key, show) -> showMap.[key] <- show)

    member this.cacheShow parsedName foundShow = 
        showMap.[parsedName] <- foundShow
        let mapping = sprintf "%s -> %s *** %d" parsedName foundShow.seriesName foundShow.id
        if not (File.ReadAllLines("./shows.mapping") |> Seq.exists(fun line -> line = mapping)) then
            File.AppendAllText("./shows.mapping", Environment.NewLine + mapping)

    member this.tryGetShow show = showMap.TryGetValue show
    member this.tryGetEpisodes show = showEpisodes.TryGetValue show