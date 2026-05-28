module AmongDucks.ConsoleInput

open System

let safeKeyAvailable () =
    try
        Console.KeyAvailable
    with
    | _ -> false

let readKeySafely () =
    try
        Some(Console.ReadKey true)
    with
    | _ -> None

let drainInput () =
    while safeKeyAvailable () do
        readKeySafely () |> ignore

let appendCharacter (key: ConsoleKeyInfo) (inputText: string) =
    if Char.IsControl key.KeyChar then
        inputText
    else
        inputText + string key.KeyChar
