namespace EvenOdd


module EvenOdd =

    [<CLIMutable>]
    type Player = { Id: string; Score: int }

    type Turn =
        | Pass
        | Even of int
        | Odd of int    
    
    [<CLIMutable>]
    type Game = { Players: Player list; Round: (Player * Turn) list }


    let createPlayer id =
        { Player.Id = id; Player.Score = 10 }

    let start player =
        { Players = [ player ]; Round = [] }

    let join player game =
        { game with Players = player :: game.Players }

    let leave player game =
        { game with Players = game.Players |> List.filter ( (<>)  player) }

    let replaceByPlayer pl = replaceBy (fun x y -> x.Id = y.Id) pl

    let getBet = function
        | Even v | Odd v -> v
        | Pass -> 0

    let makeTurn player turn game =
        let bet =  getBet turn 
        let p = { player with Score = player.Score - bet }
        { game with Round = (p, turn) :: game.Round; Players = game.Players |> replaceByPlayer [ p ] }

    let next winnerFn tossFn game =
        if (game.Players.Length > game.Round.Length) then game
        else
            let toss = tossFn()
            let winners = winnerFn toss game
            { game with Round = []; Players = game.Players |> replaceBy (fun x y -> x.Id = y.Id) winners }

