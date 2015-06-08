(**
- title : F# in 5 minutes
- description : A lightning talk presentation on F#
- author : Marcus Griep
- theme : Sky
- transition : default

***
*)
(*** hide ***)
#r "../packages/NodaTime/lib/portable-net4+sl5+netcore45+wpa81+wp8+MonoAndroid1+MonoTouch1+XamariniOS1/NodaTime.dll"
#r "../packages/FSharp.Data/lib/net40/FSharp.Data.dll"
type Lanuguage = int
let functional : Lanuguage = 0
let ``C#`` = functional
(**

## F# in 5 minutes

![FSharp Logo](http://fsharp.org/img/logo/fsharp256.png)

### Being Functional in .NET

*)
let ``F#`` = ``C#`` + functional
(**
***

### What is F#?

* Strongly-typed
* Functional-first language
* Fully integrated with the .NET CLR
* Cross-platform

***

### Functional-First
*)
let a = 5
let rec factorial x =
  match x with
  | 0 -> 1
  | _ -> x * factorial (x - 1)
let c = factorial a
(**
Generates the following result:
*)
(*** include-value: c ***)
(**
---

### Object-Oriented
*)
type IInterface =
  abstract member Hello : unit -> string
  abstract member Name : string with get, set

type Concrete() =
  interface IInterface with
    member val Name = "World" with get, set
    member x.Hello() = sprintf "Hello %s!" (x :> IInterface).Name
(**
---

### Static Mocking
*)
let myObj =
  { new IInterface with
      member __.Hello() = "Mocked"
      member __.Name
        with get () = "MockWorld"
        and set _ = () }
(**
---

### Imparative
*)
let mutable imparativeStr = ""

for i in 1 .. 10 do
  imparativeStr <- imparativeStr + myObj.Hello()
(**
Generates:
*)
(*** include-value: imparativeStr ***)
(**
***

### Why F#

* Immutability
* Correctness
* `null` prohibited (in F# types)
* High-fidelity code
* Embarassingly parallelizable

***

### Fully integrated with the .NET CLR
*)
open NodaTime

let compiledAt = SystemClock.Instance.Now

let mockClock =
  { new IClock with
      member __.Now
        with get() = Instant.FromSecondsSinceUnixEpoch(0L) }

let mockTime = mockClock.Now
(**
***

### Everything is a function
*)
type HttpContext = { OwinEnvironment : Map<string,obj> }

type processHttpRequestAsync =
  HttpContext -> Async<HttpContext option>
(**
***

### Immutable Structures

#### Tuples
*)
let myTuple = 5, "string"

let result, output = System.Int64.TryParse("NotALong")
(**
---

### Immutable Structures

#### Records
*)
type MyRecord =
  { Index : int
    Key : string }

let record = { Index = 5; Key = myObj.Name }

let newRecord = { record with Index = 7 }
(**
---

### Immutable Structures

#### Discriminated Unions
*)
type Result<'a,'b> =
  | Success of 'a
  | Failure of 'b

type RiskyFailure =
  | DependencyTimedOut
  | ValidationError of string
(**
---

### Immutable Structures

#### Discriminated Unions
*)
let riskyFunction r =
  match r with
  | { Key = null } ->
    Failure <| ValidationError "Null Name"
  | _ ->
    Success r
(**
Example result:
*)
(*** include-value: riskyFunction newRecord ***)
(*** hide ***)
let badRecord = { newRecord with Key = null }
(*** include-value: riskyFunction badRecord ***)
(**
***

### Units of Measure

*)
[<Measure>] type m
[<Measure>] type s
[<Measure>] type hr
[<Measure>] type mi
[<Measure>] type mph = mi/hr

let toMiles dis = dis * (1M/1609.34M<m/mi>)
let toHours sec = sec * (1M/3600M<s/hr>)
let invert (v : decimal<'u>) : decimal<1/'u> = (1M/v)
let toMph = invert >> toHours >> invert >> toMiles

let speedOfSound = toMph 343.59M<m/s>
(**

`speedOfSound`:

*)
(*** include-value: speedOfSound ***)
(**
***
### Type Providers
*)
open FSharp.Data

type SwaggerApi = JsonProvider<"../petstore-expanded.json">

let petstore = SwaggerApi.GetSample()

type SwaggerRoot = SwaggerApi.Root

let version = petstore.Swagger
(**
---
### Active Patterns
*)
let (|IsRequired|IsOptional|) (param : SwaggerApi.Parameter2) =
  match param.Required with
  | true -> IsRequired
  | false -> IsOptional

(**
---
### Strongly Types
*)
let requiredPostParameters =
  petstore.Paths.Pets.Post.Parameters
  |> Seq.choose
    (fun p ->
      match p with
      | IsRequired ->
        Some <| sprintf "%s (%s)" p.Name p.Description
      | IsOptional ->
        None)
  |> String.concat ", "
(**
Required Parameters:
*)
(*** include-value: requiredPostParameters ***)

(**
***

## Skip a Grade
* Don Syme, creator of F#, co-created generics in the CLR
* Asynchronous computation expressions
 * `await`/`async`
* Type Inference
 * `var`
* Pattern Matching
 * Exception filters
* Railway-Oriented Programming
 * `Nullable` and `null` propogation
* Immutable by default
* Write less code that provides more value
*)

(**
---

### Async Computation Expressions
*)
(*** hide ***)
let buildRandomOrgUrl : int64 -> string =
  sprintf "https://www.random.org/integers/?num=1&min=1&max=%d&col=1&base=10&format=plain&rnd=new"
(**
*)

let getRandomLongOverTheWebAsync maxValue = async {
  printfn "Getting Bytes..."
  let! bytes =
    buildRandomOrgUrl maxValue
    |> Http.AsyncRequestString
  return int64 bytes
}

let myRand =
  getRandomLongOverTheWebAsync 1000L
  |> Async.RunSynchronously
(**
 * Generates:
*)
(*** include-value: myRand ***)
(**

***

### F# Projects

* Infrastructure
 * [FAKE](http://fsharp.github.io/FAKE)
 * [Paket](https://fsprojects.github.io/Paket/)
* Data
 * [FsLab](http://fslab.org)
 * [Streams](http://nessos.github.io/Streams/)
* Testing
 * [Unquote](https://code.google.com/p/unquote/wiki/GettingStarted)
 * [FsCheck](https://github.com/fsharp/FsCheck)

---

### F# Projects

* Web
 * [WebSharper](http://websharper.com)
 * [Suave](http://suave.io)
 * [Canopy](https://lefthandedgoat.github.io/canopy/index.html)
* [Akka.Net](http://getakka.net)
* [FsReveal](https://fsprojects.github.io/FsReveal/index.html)

***
### Find out more

[F# Software Foundation](http://fsharp.org/)

[Try F#](http://www.tryfsharp.org/)

[F# for Fun and Profit](http://fsharpforfunandprofit.com/)

![F# |> I <3](http://fsharpforfunandprofit.com/assets/img/IHeartFsharp160.png)

*)