module TvApi

open Newtonsoft.Json.Linq
open System
open System.IO
open System.Net
open System.Text
open Newtonsoft.Json
open Types

type TvApi() =
    let mutable apikey = ""
    let mutable user = ""
    let mutable userkey = ""
    let mutable token = ""


    let extractValue (line:string) = 
        (line.Split '=').[1].Trim()

    let readcredentials = 
        match File.ReadAllLines("./credentials.cred") |> Array.take 3 with
        |[|keyline;userline;pwline|] ->
            apikey <- keyline |> extractValue
            user <- userline |> extractValue
            userkey <- pwline |> extractValue
        |_ -> raise (Exception "invalid configuration")

    do readcredentials

    let apiUrl = "https://api.thetvdb.com"

    let login = 
        let t = async {
            use client = new WebClient()
            let body = "{\"apikey\":\"" + apikey + "\", \"username\":\"" + user + "\", \"userkey\":\"" + userkey + "\"}" 
            client.Headers.Set("Content-Type", "application/json")
            let! response = client.UploadDataTaskAsync(apiUrl + "/login", "POST",  body |> Encoding.UTF8.GetBytes) |> Async.AwaitTask
            let jsonToken = JObject.Parse(Encoding.UTF8.GetString response)
            return jsonToken.["token"] |> string } |> Async.RunSynchronously
        token <- t

    let rec getAuthorizedClient t =
        match t with
        |"" -> login
               getAuthorizedClient token
        |_ -> let client = new WebClient()
              client.Encoding <- Encoding.UTF8
              client.Headers.Set("Content-Type", "application/json")
              client.Headers.Set("Authorization", "Bearer " + t)
              client.Headers.Set("Accept-Language", "en")
              client

    member this.GetAsync uri = async {
        let client = getAuthorizedClient token
        let! response = client.AsyncDownloadString(Uri(apiUrl + uri))
        return response
    }
    