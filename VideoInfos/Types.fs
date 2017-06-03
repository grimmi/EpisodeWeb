module Types

open System
open Newtonsoft.Json.Linq

type Show = { aliases: string[]; id: int; seriesName: string }
type SearchResponse = { data: Show[] }
type Links = { first: Nullable<int>; last: Nullable<int>; next: Nullable<int>; prev: Nullable<int> }
type Episode = { absoluteNumber: Nullable<int>; airedEpisodeNumber: Nullable<int>; airedSeason: Nullable<int>; episodeName: string; firstAired: string }
type EpisodeResult = { links: Links; data: Episode[] }

type VideoInfo(title: string) =
    member this.Title = title
    abstract member VideoType: string
    abstract member ToJson: unit -> JObject

    default this.VideoType = "video"

    default this.ToJson() = 
        let json = JObject()
        json.Add("title", JToken.FromObject(this.Title))
        json.Add("type", JToken.FromObject(this.VideoType))
        json

type EpisodeInfo(show: string, title: string, season: int, number: int, firstAired: DateTime) =
    inherit VideoInfo(title)
    member this.Show = show
    member this.Season = season
    member this.Number = number
    member this.FirstAired = firstAired

    override this.VideoType = "episode"

    override this.ToJson() =
        let json = base.ToJson()
        json.Add("show", JToken.FromObject(this.Show))
        json.Add("season", JToken.FromObject(this.Season))
        json.Add("number", JToken.FromObject(this.Number))
        json.Add("firstaired", JToken.FromObject(this.FirstAired))
        json

type MovieInfo(title: string) =
    inherit VideoInfo(title)

    override this.VideoType = "movie"
    
type ParseInfo(path: string, parsedshow: string option, dbshow: string option, parsedtitle: string option, dbtitle: string, season: Nullable<int>, number: Nullable<int>, firstaired: string) =
    member this.ToJson() =
        let json = JObject()
        json.Add("path", JToken.FromObject(path))
        json.Add("parsedshow", JToken.FromObject(parsedshow))
        json.Add("dbshow", JToken.FromObject(dbshow))
        json.Add("parsedtitle", JToken.FromObject(parsedtitle))
        json.Add("dbtitle", JToken.FromObject(dbtitle))
        json.Add("season", JToken.FromObject(season))
        json.Add("number", JToken.FromObject(number))
        json.Add("firstaired", JToken.FromObject(firstaired))
        json

let makeParseInfo path parsedshow dbshow parsedtitle dbtitle season number firstaired =
    ParseInfo(path, parsedshow, dbshow, parsedtitle, dbtitle, season, number, firstaired)