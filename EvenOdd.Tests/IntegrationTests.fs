module IntegrationTests

open System.Diagnostics
open EvenOdd
open Xunit

[<Fact>]
let ``Make game )``() =
    let game = EvenOddGame()
    let fst = EvenOdd.createPlayer "first"
    let snd = EvenOdd.createPlayer "second"

    let newGame = game.Start fst
    let joinedGame = game.Join snd
    let turn = game.Turn(fst, EvenOdd.Even 2)
    let result = game.Turn(snd, EvenOdd.Odd 2)
    sprintf "%A" result |> Debug.WriteLine
