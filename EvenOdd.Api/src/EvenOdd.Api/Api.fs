module Api

open Microsoft.AspNetCore.Http
open FSharp.Control.Tasks
open Giraffe
open EvenOdd
open Back
open Newtonsoft.Json


let response = function
    | Ok x -> Successful.OK x
    | Error e -> RequestErrors.BAD_REQUEST(e.ToString())


let execute
    (fn: PlayerManager -> GameManager -> Result<'a, 'b>) (next: HttpFunc) (ctx: HttpContext) =
    task {
        let gameManager = ctx.GetService<GameManager>()
        let playerManager = ctx.GetService<PlayerManager>()
        let result = fn playerManager gameManager |> response
        return! result next ctx
    }


let games _ (gameManager: GameManager) =
    gameManager.GetAll() |> Ok

let game id _ (gameManager: GameManager) =
    gameManager.GetById id |> Ok

let start playerId (playerManager: PlayerManager) (gameManager: GameManager) =
    playerManager.GetById playerId
    |> Result.map gameManager.Start

let join (gameId, playerId) (playerManager: PlayerManager) (gameManager: GameManager) =
    playerManager.GetById playerId
    |> Result.bind (gameManager.Join gameId)

let leave (gameId, playerId) (playerManager: PlayerManager) (gameManager: GameManager) =
    playerManager.GetById playerId
    |> Result.bind (gameManager.Leave gameId)

let turn gameId playerId turn (playerManager: PlayerManager) (gameManager: GameManager) =
    playerManager.GetById playerId
    |> Result.bind (gameManager.Turn turn gameId)

let pass (gameId, playerId) = turn gameId playerId EvenOdd.Turn.Pass

let even (gameId, playerId, bet) = turn gameId playerId (EvenOdd.Turn.Even bet)

let odd (gameId, playerId, bet) = turn gameId playerId (EvenOdd.Turn.Odd bet)

let players (playerManager: PlayerManager) _ =
    playerManager.GetAll() |> Ok

let create id (playerManager: PlayerManager) _ =
    playerManager.Create id

let (``(.)(.)``) = (+)


let webApp() =
    choose [
        subRoute "/api" (setHttpHeader "Content-Type" "application/json" >=> choose [
            subRoute "/games" (choose [
                GET >=> choose [
                    route "" >=> (games |> execute)
                    routef "/%i" (game >> execute) ]
                POST >=> choose [
                    routef "/create/%s" (start >> execute)
                    routef "/%i/join/%s" (join >> execute)
                    routef "/%i/leave/%s" (leave >> execute)
                    routef "/%i/pass/%s" (pass >> execute)
                    routef "/%i/even/%s/bet/%i" (even >> execute)
                    routef "/%i/odd/%s/bet/%i" (odd >> execute) ]
            ])
            subRoute "/players" (choose [
                GET >=> route "" >=> (players |> execute)
                POST >=> routef "/create/%s" (create >> execute)
                GET >=>  route "/tits" >=> text ((``(.)(.)``) "(.)" "(.)" )
            ])
        ])
        setStatusCode 404 >=> text "Not Found" ]
