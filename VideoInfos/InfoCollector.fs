module InfoCollector

open Types

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
                           match show with
                           |None -> return makerEpisodeParsed None None None None
                           |Some s -> 
                                let! episode = Renamer.getEpisode show file
                                match episode with
                                |None -> return makerEpisodeParsed None None None None
                                |Some e -> return makerEpisodeParsed (Some(""), Some(0), Some(0), Some(DateTime.Now)) //e.airedSeason e.airedEpisodeNumber DateTime.Now
                           }
        return makerEpisode
        }

let GetEpisodeInfoTaskAsync file =
    (GetEpisodeInfoAsync file) |> Async.StartAsTask