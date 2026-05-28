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

let private supportsAnsiStyle () =
    let noColor = Environment.GetEnvironmentVariable("NO_COLOR")
    let term = Environment.GetEnvironmentVariable("TERM")
    let colorDisabled = not (String.IsNullOrEmpty noColor)
    let terminalIsDumb = String.Equals(term, "dumb", StringComparison.OrdinalIgnoreCase)

    not colorDisabled && not terminalIsDumb

let private ansiEscape = string (char 27)

let private styleWith ansiCode text =
    if supportsAnsiStyle () then
        ansiEscape + "[" + ansiCode + "m" + text + ansiEscape + "[0m"
    else
        text

let private styleDuckCall text = styleWith "1;30;43" text

let private joinCells cells =
    cells
    |> Array.map (center cellWidth)
    |> String.concat "  "

let private joinCallCells call =
    [| call; call; ""; call; call |]
    |> Array.map (fun text ->
        if String.IsNullOrEmpty text then
            center cellWidth text
        else
            center cellWidth text |> styleDuckCall)
    |> String.concat "  "

let private pixelToText pixel =
    let backgroundCode =
        match pixel with
        | 'Y' -> Some "43"
        | 'O' -> Some "41"
        | 'K' -> Some "40"
        | 'C' -> Some "46"
        | 'W' -> Some "47"
        | _ -> None

    match backgroundCode with
    | Some code -> styleWith code "  "
    | None -> "  "

let private pixelRowToText row =
    row
    |> Seq.map pixelToText
    |> String.concat ""

let private joinPixelAnimalCells artLineIndex =
    [|
        duckPixelArt[artLineIndex] |> pixelRowToText
        duckPixelArt[artLineIndex] |> pixelRowToText
        goosePixelArt[artLineIndex] |> pixelRowToText
        duckPixelArt[artLineIndex] |> pixelRowToText
        duckPixelArt[artLineIndex] |> pixelRowToText
    |]
    |> String.concat "  "

let private joinAsciiAnimalCells artLineIndex =
    joinCells
        [|
            duckArt[artLineIndex]
            duckArt[artLineIndex]
            gooseArt[artLineIndex]
            duckArt[artLineIndex]
            duckArt[artLineIndex]
        |]

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
        | Some call -> joinCallCells call
        | None -> ""

    lines.Add(callLine)
    lines.Add("")

    if supportsAnsiStyle () then
        for artLineIndex in 0 .. duckPixelArt.Length - 1 do
            lines.Add(joinPixelAnimalCells artLineIndex)
    else
        for artLineIndex in 0 .. duckArt.Length - 1 do
            lines.Add(joinAsciiAnimalCells artLineIndex)

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
