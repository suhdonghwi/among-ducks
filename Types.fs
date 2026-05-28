module AmongDucks.Types

type ScreenStatus =
    | Waiting
    | Cooldown
    | Call of remainingSeconds: float

type CallResult =
    | Correct
    | Incorrect of submittedInput: string
    | TimedOut of typedInput: string

type MenuChoice =
    | Restart
    | Quit
