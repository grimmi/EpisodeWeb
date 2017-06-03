module ShowParser

open System
open System.IO

let parseShowName (file:string) = 
    let filename = Path.GetFileName file
    if filename.IndexOf "__" <> -1 then
        match filename.Split([|"__"|], StringSplitOptions.RemoveEmptyEntries) |> List.ofArray with
        |show::t -> Some(show.Replace("_", " " ))
        |_ -> None
    else
        let pointIdx = filename.IndexOf '.'
        Some(filename.Substring(0, pointIdx - 3).Replace("_", " "))

let parseEpisodeName (file:string) = 
    let filename = Path.GetFileName file
    let pointIdx = filename.IndexOf '.'
    let dblUnderscoreIdx = filename.IndexOf "__"

    if pointIdx = -1 || dblUnderscoreIdx = -1 then
        None
    else
        let length = (pointIdx - 3) - (dblUnderscoreIdx + 2)
        match length with
        |_ when length > 0 -> Some(filename.Substring(dblUnderscoreIdx + 2, length).Replace("_", " "))
        |_ -> None

let parseDate (file:string) = 
    let filename = Path.GetFileName file
    let pointIdx = filename.IndexOf '.'
    match pointIdx with
    |(-1) -> None
    |_ -> 
        let datePart = filename.Substring(pointIdx - 2, 8)
        Some(DateTime.ParseExact(datePart, "yy.MM.dd", null))