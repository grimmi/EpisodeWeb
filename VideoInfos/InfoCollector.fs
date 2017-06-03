module InfoCollector

open Types
open System

let private GetEpisodeInfoAsync file = async {
        let showParsed = ShowParser.parseShowName file
        let makerParse = makeParseInfo file showParsed
        let! show = Renamer.findShow showParsed
        let makerShow = match show with
                        |None -> makerParse None
                        |Some s -> makerParse(Some(s.seriesName))
        let episodeParsed = ShowParser.parseEpisodeName file
        let makerEpisodeParsed = makerShow episodeParsed
        let! makerEpisode = async{
                           let zero = Nullable<int>(0)
                           match show with
                           |None -> return makerEpisodeParsed "" zero zero ""
                           |Some s -> 
                                let! episode = Renamer.getEpisode show file
                                match episode with
                                |None -> return makerEpisodeParsed "" zero zero ""
                                |Some e -> 
                                    let info = makeParseInfo file showParsed (Some(s.seriesName)) episodeParsed e.episodeName e.airedSeason e.airedEpisodeNumber e.firstAired
                                    return info
                           }
        return makerEpisode
        }

let GetEpisodeInfoTaskAsync file =
    (GetEpisodeInfoAsync file) |> Async.StartAsTask