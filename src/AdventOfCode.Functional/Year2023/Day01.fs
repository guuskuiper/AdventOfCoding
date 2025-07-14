module AdventOfCode.Functional.Year2023.Day01

open System

let charIsNumber2 = Char.IsAsciiDigit

let charIsNumber (c : char) =
    Char.IsAsciiDigit c

let rec first (xs : List<char>) =
   let h = List.head xs
   let l = List.last xs
   [h ; l]

let filterNumbers (l : String) =
    l
    |> List.ofSeq
    |> List.filter charIsNumber
    |> first
    |> String.Concat

let parseFilterResult (l : String) =
    l
    |> filterNumbers
    |> int
 
let parseFilterResult2 =
    filterNumbers >> int
    
let solveALines ls =
    ls
    |> Array.map parseFilterResult
    |> Array.sum

let digits = seq { 0 .. 9 }
let digitsString =
    digits
    |> Seq.map (fun x -> $"%d{x}")
    
let digitsAnd = ["zero"; "one"; "two"; "three"; "four"; "five"; "six"; "seven"; "eight"; "nine"]
let digitsAnd2 = seq {"zero"; "one"; "two"; "three"; "four"; "five"; "six"; "seven"; "eight"; "nine"}
let allDigits =
    Seq.append digitsString digitsAnd2
    
let notNegative (i : int) =
    i >= 0
    
let parseComplex (l : string) =
    if l.Length = 1 then int l
    else Seq.findIndex (fun e -> e = l) digitsAnd2

let parseComplexList (l : string) =
    if l.Length = 1 then int l
    else List.findIndex (fun e -> e = l) digitsAnd

let findIndex (l : string) =
    allDigits
    |> Seq.map (fun x -> (l.IndexOf(x), x))
    |> Seq.filter (fun (x, _) -> x >= 0)
    |> Seq.minBy fst
    |> snd

let findLastIndex (l : string) =
    allDigits
    |> Seq.map (fun x -> (l.LastIndexOf(x), x))
    |> Seq.filter (fun (x, _) -> x >= 0)
    |> Seq.maxBy fst
    |> snd

let solveBFull (l : string) =
    let h = (findIndex >> parseComplex) l
    let t = (findLastIndex >> parseComplex) l
    $"%d{h}%d{t}"
    |> int
    
let solveBLines ls =
    ls
    |> Array.map solveBFull
    |> Array.sum

let replacements = [
        ( "zero", "ze0o");
        ( "one", "o1e");
        ( "two", "t2o");
        ( "three", "th3ee");
        ( "four", "fo4r");
        ( "five", "fi5e");
        ( "six", "s6x");
        ( "seven", "se7en");
        ( "eight", "ei8ht");
        ( "nine", "ni9e");
    ]

let replace (l : string) =
    replacements
    |> List.fold (fun (a : string) r -> a.Replace(fst r, snd r)) l

let solveBLinesReplace (ls : string array) =
    ls
    |> Array.map replace
    |> solveALines
    
    
    
