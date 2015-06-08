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
But please don't write code like this in F#

***

### Why F#

* .NET language
 * Fully supported
 * Open source with a significant community
* Quicker time-to-market
 * Read-Evaluate-Print Loop (REPL)
* Correctness
 * Immutability
 * `null` prohibited (in F# types)
* High-fidelity code
 * Succinct Domain-Driven Design
 * Avoid "primitive obsession"
* Embarassingly parallelizable
 * Highly distributable

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
### Correctness
*)
let myTuple = 5, "string"

// This is a boolean test, not an assignment
myTuple = (10, "newString")
(**
Mutation must be explicit
*)
let mutable mutatable = 10

mutatable <- 42
(**
F# types don't have the concept of null

    [lang=fs]
    // Compiler error:
    // The type (int * string) does not have 'null' as a proper value
    if myTuple = null then failwith "Bad Nulls!"

---
### Records
*)
type MyRecord =
  { Index : int
    Key : string }

let record = { Index = 5; Key = myObj.Name }

let newRecord = { record with Index = 7 }
(**

Includes structural equality and comparison for free

---
### Discriminated Unions
*)
type Result<'a,'b> =
  | Success of 'a
  | Failure of 'b

type RiskyFailure =
  | DependencyTimedOut
  | ValidationError of string
(**

Model your domain explicitly

---
### Discriminated Unions
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
---
### Domain Modeling

A leaky model
*)
type LeakyEmailContactInfo =
  { EmailAddress : string
    IsVerified : bool }
(**
---
### Domain Modeling
*)
type EmailAddress = | EmailAddress of string

type EmailContactInfo =
  private
    | Unverified of EmailAddress
    | Verified of EmailAddress * Instant

let create email = Unverified email

let verify info dateVerified =
  match info with
  | Unverified email -> Verified (email, dateVerified)
  | Verified _ -> info
(**

***
### Time to market
#### Type Providers
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
### Strongly Typed
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
### Highly Distributable

![MBrace Example](http://www.m-brace.net/assets/images/code/example-flow.png)

This is the only piece of code that you've seen that hasn't been executed in this presentation.

***

## Skip a Grade
* Don Syme, creator of F#, co-created generics in the CLR
* Asynchronous computation expressions <span style="font-size:75%">(C#: `await`/`async`)</span>
* Type Inference <span style="font-size:75%">(C#: `var`)</span>
* Pattern Matching <span style="font-size:75%">(C#: exception fiters)</span>
* Railway-Oriented Programming <span style="font-size:75%">(C#: `?.` `null` propogation)</span>
* Immutable by default
* Write less code that provides more value

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
* Distributed
 * [Akka.Net](http://getakka.net)
 * [MBrace](http://www.m-brace.net/)
* [FsReveal](https://fsprojects.github.io/FsReveal/index.html)

***
### Find out more

[F# Software Foundation](http://fsharp.org/)

[Try F#](http://www.tryfsharp.org/)

[F# for Fun and Profit](http://fsharpforfunandprofit.com/)

![F# |> I <3](http://fsharpforfunandprofit.com/assets/img/IHeartFsharp160.png)

*)