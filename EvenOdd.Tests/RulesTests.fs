module RulesTests

open EvenOdd
open Xunit

[<Fact>]
let ``Player is unique for game``() =
    let result = playerOne
               |> EvenOdd.start
               |> Rules.tryJoin playerOne
    
    Result.Error Rules.PlayerAlreadyInGame = result |> Assert.True

[<Fact>]
let ``Player can not leave game when make a bet``() =
    let result = playerOne
               |> EvenOdd.start
               |> EvenOdd.makeTurn playerOne EvenOdd.Pass
               |> Rules.tryLeave playerOne
    Result.Error Rules.PlayerAlreadyInGame = result |> Assert.True

[<Fact>]
let ``Only player of game can turn``() =
    let result = playerOne
               |> EvenOdd.start
               |> Rules.tryMakeTurn playerTwo EvenOdd.Pass
    Result.Error Rules.PlayerNotInGame = result |> Assert.True

[<Fact>]
let ``Player can turn only once``() =
    let result = playerOne
               |> EvenOdd.start
               |> EvenOdd.makeTurn playerOne EvenOdd.Pass
               |> Rules.tryMakeTurn playerOne EvenOdd.Pass
    Result.Error Rules.PlayerMadeTurn = result |> Assert.True

[<Fact>]
let ``Player score can not decreased less then zero``() =
    let result = playerOne
               |> EvenOdd.start
               |> Rules.tryMakeTurn playerOne (EvenOdd.Even(playerOne.Score + 1))
    Result.Error Rules.PlayerCanNotBetMoreThenHave = result |> Assert.True

[<Fact>]
let ``When tossed select winners``() =
    let game = playerOne
               |> EvenOdd.start
               |> EvenOdd.join playerTwo
               |> EvenOdd.makeTurn playerOne (EvenOdd.Even playerOne.Score)
               |> EvenOdd.makeTurn playerTwo (EvenOdd.Odd playerTwo.Score)

    let winners = Rules.getWinners Rules.EvenWins game
    let winner = { playerOne with Score = playerOne.Score + playerTwo.Score }
    winners |> List.exactlyOne |> (=) winner |> Assert.True

