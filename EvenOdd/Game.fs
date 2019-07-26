namespace EvenOdd

type EvenOddGame () =
    let mutable state: EvenOdd.Game = { Players = []; Round = [] }

    let setState = function
        | Ok s -> state <- s; Ok s
        | Error e -> Error e
    
    
    member s.Start(player) =
        player |> Rules.tryStart|> setState

    member __.Join(player) =
        state |> Rules.tryJoin player |> setState

    member __.Leave(player) =
        state |> Rules.tryLeave player |> setState

    member __.Turn(player, turn) =
        state |> Rules.tryMakeTurn player turn  |> setState
    
    static  member Fn()=1   
        

        

        
