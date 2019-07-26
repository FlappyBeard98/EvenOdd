module Back

open System.Collections.Concurrent
open System.Linq
open EvenOdd

type GameManager() =
    
    let games = new ConcurrentDictionary<int,EvenOddGame>()
    
    let getNextId () =
        if games.Any() then games.Keys.ToList().Max() + 1
        else 1
        
    member __.Start(player) =
         
        let id = getNextId()        
        let game = EvenOddGame()
        if  games.TryAdd(id,game) then
            games.[id].Start player |> ignore
            id
        else failwith "Can not create game, try later"

    member __.Join id player=
        games.[id].Join player

    member __.Leave id player =
        games.[id].Leave player 

    member __.Turn turn id player=
        games.[id].Turn (player,turn)
    
    member __.GetById id =
        games.[id]
        
    member __.GetAll() =
        games.Keys.AsEnumerable() |> Array.ofSeq

type PlayerManager() =
    
    let players = new ConcurrentDictionary<string,EvenOdd.Player>()
    
    member __.Create(id) =
        let player = EvenOdd.createPlayer id
        if players.TryAdd(id,player) then player |> Ok
        else Error Rules.Errors.PlayerAlreadyExists
            
    member __.GetById(id) =
        if players.ContainsKey(id) then players.[id] |> Ok
        else Error Rules.Errors.PlayerNotExists
    
    member __.GetAll() =
        players.Values.AsEnumerable() |> Array.ofSeq