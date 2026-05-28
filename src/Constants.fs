module AmongDucks.Constants

open System

let tickMilliseconds = 100
let tickSeconds = 0.1
let cooldownTicks = 20
let callTicks = 20
let responseLimit = TimeSpan.FromSeconds 2.0
let waitingCallChance = 0.05
let cellWidth = 12

let duckCalls =
    [|
        "qquaaack"
        "qquaaak"
        "qquaack"
        "qquaak"
        "qquack"
        "qquak"
        "qquuaaak"
        "qquuaack"
        "qquuaak"
        "qquuack"
        "qquuak"
        "quaaaack"
        "quaaaak"
        "quaaack"
        "quaaak"
        "quaack"
        "quaak"
        "quack"
        "quak"
        "quuaaack"
        "quuaaak"
        "quuaack"
        "quuaak"
        "quuack"
        "quuak"
    |]

let duckArt =
    [|
        "  __   "
        "<(o )__"
        " (____)"
        "  duck "
    |]

let gooseArt =
    [|
        "  ___  "
        " (o  )>"
        "/(____)"
        " goose "
    |]

let duckPixelArt =
    [|
        "..YY.."
        ".YYKYO"
        "YYYYY."
        ".YYYY."
        ".O..O."
    |]

let goosePixelArt =
    [|
        "..CC.."
        "..CKCO"
        ".CWWW."
        "CWWWW."
        ".O..O."
    |]
