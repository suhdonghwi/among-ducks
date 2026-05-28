module AmongDucks.Renderer

open System
open AmongDucks.Constants
open AmongDucks.Types

let private center width (text: string) =
    if text.Length >= width then
        text
    else
        let leftPadding = (width - text.Length) / 2
        let rightPadding = width - text.Length - leftPadding
        String(' ', leftPadding) + text + String(' ', rightPadding)

let private joinCells cells =
    cells
    |> Array.map (center cellWidth)
    |> String.concat " "

let private statusText status =
    match status with
    | Waiting -> "Status: Waiting"
    | Cooldown -> "Status: Cooldown"
    | Call remaining -> sprintf "Status: Call (%.1fs remaining)" remaining

let buildScreen score status callText inputText =
    let lines = ResizeArray<string>()
    lines.Add(sprintf "Score: %d" score)
    lines.Add(statusText status)
    lines.Add("")

    let callLine =
        match callText with
        | Some call -> joinCells [| call; call; ""; call; call |]
        | None -> ""

    lines.Add(callLine)

    for artLineIndex in 0 .. duckArt.Length - 1 do
        lines.Add(
            joinCells
                [|
                    duckArt[artLineIndex]
                    duckArt[artLineIndex]
                    gooseArt[artLineIndex]
                    duckArt[artLineIndex]
                    duckArt[artLineIndex]
                |]
        )

    lines.Add(joinCells [| ""; ""; "^ You"; ""; "" |])
    lines.Add("")
    lines.Add("Input:")
    lines.Add("> " + inputText)
    String.concat Environment.NewLine lines

let private clearConsole () =
    try
        Console.Clear()
    with
    | _ -> ()

let drawScreen score status callText inputText =
    clearConsole()
    Console.Write(buildScreen score status callText inputText)
    Console.Out.Flush()
