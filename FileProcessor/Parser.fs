namespace FileProcessor

open System
open System.IO

type Processor(f: string, c: Cache) = 
    let file = f
    let cache = c

    member this.ParseShowName =
        let filename = Path.GetFileName file
        if filename.IndexOf "__" <> -1 then
            match filename.Split([|"__"|], StringSplitOptions.RemoveEmptyEntries) |> List.ofArray with
            |show::t -> Some(show.Replace("_", " " ))
            |_ -> None
        else
            let pointIdx = filename.IndexOf '.'
            Some(filename.Substring(0, pointIdx - 3).Replace("_", " "))

    member this.FindShow =
        
        
