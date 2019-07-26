module HelperTests

open EvenOdd
open Xunit

[<Fact>]
let ``When use replaceBy then items are replaced``() =
    let sourceList = [ playerOne; playerTwo; playerThree ]
    let newPlayerTwo = { playerTwo with Score = 2 }
    let newPlayerOne = { playerOne with Score = 1 }
    let act = sourceList |> replaceBy (fun x y -> x.Id = y.Id) [ newPlayerTwo; newPlayerOne ]
    act.Length = 3 |> Assert.True
    act |> List.contains newPlayerTwo |> Assert.True
    act |> List.contains newPlayerOne |> Assert.True
    act |> List.contains playerThree |> Assert.True