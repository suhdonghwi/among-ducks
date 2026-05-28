module AmongDucks.Game

open System
open System.Diagnostics
open System.Threading
open AmongDucks.ConsoleInput
open AmongDucks.Constants
open AmongDucks.Renderer
open AmongDucks.Types

let private random = Random()

let private runCooldown score =
    for _ in 1 .. cooldownTicks do
        drawScreen score Cooldown None ""
        drainInput()
        Thread.Sleep tickMilliseconds
        drainInput()

let private chooseDuckCall () =
    duckCalls[random.Next duckCalls.Length]

let private waitForDuckCall score =
    let mutable selectedCall = None

    while selectedCall.IsNone do
        drawScreen score Waiting None ""
        Thread.Sleep tickMilliseconds
        drainInput()

        if random.NextDouble() < waitingCallChance then
            selectedCall <- Some(chooseDuckCall())

    selectedCall.Value

let private remainingSecondsFromElapsed (elapsed: TimeSpan) =
    let elapsedTicks =
        elapsed.TotalMilliseconds / float tickMilliseconds
        |> Math.Floor
        |> int
        |> min callTicks

    max 0.0 (2.0 - float elapsedTicks * tickSeconds)

let private runCall score selectedCall =
    let mutable inputText = ""
    let mutable submittedInput = None
    let mutable submittedAt = None
    let mutable lastRenderedTick = 0

    drawScreen score (Call 2.0) (Some selectedCall) inputText
    let stopwatch = Stopwatch.StartNew()

    while submittedInput.IsNone && stopwatch.Elapsed < responseLimit do
        while submittedInput.IsNone && stopwatch.Elapsed < responseLimit && safeKeyAvailable () do
            match readKeySafely () with
            | None -> ()
            | Some key ->
                match key.Key with
                | ConsoleKey.Enter ->
                    submittedInput <- Some inputText
                    submittedAt <- Some stopwatch.Elapsed
                | ConsoleKey.Backspace ->
                    if inputText.Length > 0 then
                        inputText <- inputText.Substring(0, inputText.Length - 1)

                    let remaining = remainingSecondsFromElapsed stopwatch.Elapsed
                    drawScreen score (Call remaining) (Some selectedCall) inputText
                | _ ->
                    inputText <- appendCharacter key inputText
                    let remaining = remainingSecondsFromElapsed stopwatch.Elapsed
                    drawScreen score (Call remaining) (Some selectedCall) inputText

        if submittedInput.IsNone then
            let elapsedTick =
                stopwatch.Elapsed.TotalMilliseconds / float tickMilliseconds
                |> Math.Floor
                |> int
                |> min callTicks

            if elapsedTick <> lastRenderedTick then
                lastRenderedTick <- elapsedTick
                let remaining = remainingSecondsFromElapsed stopwatch.Elapsed
                drawScreen score (Call remaining) (Some selectedCall) inputText

            if stopwatch.Elapsed < responseLimit then
                Thread.Sleep 5

    stopwatch.Stop()

    match submittedInput with
    | Some response when response = selectedCall && submittedAt.IsSome && submittedAt.Value <= responseLimit -> Correct
    | Some response -> Incorrect response
    | None -> TimedOut inputText

let rec private readMenuChoice () =
    Console.Write("Enter restart or quit: ")
    let choice = Console.ReadLine()

    match choice with
    | null -> Quit
    | "restart" -> Restart
    | "quit" -> Quit
    | _ ->
        Console.WriteLine("Invalid choice. Enter restart or quit.")
        readMenuChoice()

let private showGameOver score selectedCall finalInput =
    drawScreen score (Call 0.0) (Some selectedCall) finalInput
    Console.WriteLine()
    Console.WriteLine()
    Console.WriteLine("Game over! The goose was exposed.")
    Console.WriteLine(sprintf "Final score: %d" score)
    readMenuChoice()

let rec playGame () =
    let mutable score = 0
    let mutable running = true
    let mutable nextChoice = None

    runCooldown score

    while running do
        let selectedCall = waitForDuckCall score

        match runCall score selectedCall with
        | Correct ->
            score <- score + 1
            runCooldown score
        | Incorrect submittedInput ->
            running <- false
            nextChoice <- Some(showGameOver score selectedCall submittedInput)
        | TimedOut typedInput ->
            running <- false
            nextChoice <- Some(showGameOver score selectedCall typedInput)

    match nextChoice with
    | Some Restart -> playGame()
    | Some Quit
    | None -> ()
