namespace EvenOdd

    [<AutoOpen>]
    module Helper =

        let replaceBy fn lst =
            List.filter (fun x -> lst |> List.exists (fn x) |> not) >> (@) lst

