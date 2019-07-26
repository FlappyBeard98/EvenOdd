module EvenOddTests

open EvenOdd
open Xunit

[<Fact>]
let ``Player can create game``() =
    let game = playerOne
               |> EvenOdd.start

    game.Players |> List.contains playerOne |> Assert.True
    game.Round |> List.isEmpty |> Assert.True

[<Fact>]
let ``Player can join game``() =
    let game = playerOne
               |> EvenOdd.start
               |> EvenOdd.join playerTwo

    game.Players |> List.contains playerOne |> Assert.True
    game.Players |> List.contains playerTwo |> Assert.True

[<Fact>]
let ``Player can leave game``() =
    let game = playerOne
               |> EvenOdd.start
               |> EvenOdd.join playerTwo
               |> EvenOdd.leave playerOne

    game.Players |> List.contains playerOne |> Assert.False

[<Fact>]
let ``Player can make turn``() =
    let turn = EvenOdd.Pass
    let game = playerOne
               |> EvenOdd.start
               |> EvenOdd.makeTurn playerOne turn

    game.Round |> List.contains (playerOne, turn) |> Assert.True

[<Fact>]
let ``When player make bet then his score decreased``() =
    let game = playerOne
               |> EvenOdd.start
               |> EvenOdd.makeTurn playerOne (EvenOdd.Even playerOne.Score)
    game.Players |> List.exists (fun x -> x.Score < playerOne.Score) |> Assert.True
    game.Round |> List.map fst |> List.exists (fun x -> x.Score < playerOne.Score) |> Assert.True


[<Fact>]
let ``When winners selected then start new round``() =
    let game = playerOne
               |> EvenOdd.start
               |> EvenOdd.join playerTwo
               |> EvenOdd.makeTurn playerOne (EvenOdd.Even playerOne.Score)
               |> EvenOdd.makeTurn playerTwo (EvenOdd.Odd playerTwo.Score)
               |> EvenOdd.next (fun _ _ -> [ playerOne ]) (fun _ -> Rules.EvenWins)

    game.Round |> List.isEmpty |> Assert.True