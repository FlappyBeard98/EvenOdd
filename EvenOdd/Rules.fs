namespace EvenOdd

module Rules =

    open EvenOdd

    type Errors =
        | PlayerAlreadyInGame
        | PlayerNotInGame
        | PlayerMadeTurn
        | PlayerCanNotBetMoreThenHave
        | PlayerAlreadyExists
        | PlayerNotExists

    let tryStart: Player -> Result<EvenOdd.Game,Errors>  =
        start >> Ok
        
    let private validateJoin player game =
        if game.Players |> List.exists (fun x -> x.Id = player.Id) then Error PlayerAlreadyInGame
        else Ok game
       
    let tryJoin player =
        validateJoin player
        >> Result.map (join player)

    let private validateLeave player game =
        if game.Players |> List.exists (fun x -> x.Id = player.Id) &&
           game.Round |> List.map fst |> List.exists (fun x -> x.Id = player.Id)
        then Error PlayerAlreadyInGame
        else Ok game

    let tryLeave player =
        validateLeave player
        >> Result.map (leave player)

    let private validatePlayersTurn player = function
        | g when g.Players |> List.exists (fun x -> x.Id = player.Id) |> not -> Error PlayerNotInGame
        | g when g.Round |> List.map fst |> List.exists (fun x -> x.Id = player.Id) -> Error PlayerMadeTurn
        | g -> Ok g

    let private validatePlayersBet player turn game =
        let bet = turn |> getBet
        if (player.Score < bet) then Error PlayerCanNotBetMoreThenHave
        else Ok game

    type Toss =
        | EvenWins
        | OddWins

    let toss () =
        let rnd = System.Random()
        if rnd.NextDouble() > 0.5 then EvenWins
        else OddWins

    let private winnersFilter toss roundItem =
        let (_, bet) = roundItem
        match bet, toss with
        | Even _, EvenWins | Odd _, OddWins -> true
        | _ -> false

    let private giveScores value player =
        { player with Score = player.Score + value }

    let getWinners toss game =
        let bank = game.Round |> List.map snd |> List.sumBy getBet
        let winners = game.Round |> List.filter (winnersFilter toss)
        let score = bank / winners.Length
        winners |> List.map fst |> List.map (giveScores score)
    
    let tryMakeTurn player turn =
       validatePlayersTurn player
       >> Result.bind (validatePlayersBet player turn)
       >> Result.map (makeTurn player turn)
       >> Result.map (next getWinners toss)
    
