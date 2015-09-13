module Constants =
	let slash = "/"
	let textHtml = "text/html" // todo: use Fsharp.Data
	
module FsExpress =
	open System
	open System.Net
	open System.Text
	open System.IO	
	open FSharp.Data
	open Newtonsoft.Json
	
	open Constants

	type Handler = HttpListenerRequest->HttpListenerResponse->Async<unit>
	
	// todo: non-default port
	let defaultHost = "http://localhost:8762"
	
	// Helpers //
	let readData (req:HttpListenerRequest) =
		use reader = new IO.StreamReader(req.InputStream) 
		reader.ReadToEnd()
		
	let toObjectOf<'a> json =
		JsonConvert.DeserializeObject<'a>(json)
	
	let fromJsonOf<'T> req =
		readData req |> toObjectOf<'T>
		
	let toJson obj =
		JsonConvert.SerializeObject obj
	
	// HttpListener //	
	let listener (route:string) (handler:Handler) =
		let hl = new HttpListener()
		let postfixedRoute = if route.EndsWith slash then route else (route + slash)
		hl.Prefixes.Add (defaultHost + postfixedRoute)
		hl.Start()
		
		let task = Async.FromBeginEnd(hl.BeginGetContext, hl.EndGetContext)
		async {
			while true do
				let! context = task
				Async.Start(handler context.Request context.Response)
		} |> Async.Start
		
	// Routing //
	let get = fun route txtOf ->
		listener route (fun req resp ->
			let processRequest =			
				let text = txtOf req
				async {
					let txtBytes = Encoding.ASCII.GetBytes(text:string)
					resp.ContentType <- textHtml
					resp.OutputStream.Write(txtBytes, 0, txtBytes.Length)
					resp.OutputStream.Close()
				}
			
			if req.HttpMethod = "GET" then processRequest else async { printfn "()" }
		)
			
//			match req.HttpMethod with
//			| method when method = "GET" -> printfn "get"
//			| method when method = "POST" -> printfn "post"
//			| method when method = "PUT" -> printfn "put"
//			| _ -> printfn "HTTP verb is unknown"
			
	let post = get
	
	let put = get
	
module App =
	open FsExpress
	
	type User = {id:int; age:int; name:string}
	
	get "/" (fun req -> "Hi!")
	
	get "/home" (fun req -> "hi, from home")
	
// todo: split GET/PUT/POST/PATCH/DELETE

	get "/user" (fun req ->
		req.QueryString.["id"] |> Dump // json query to db?
		let mary = {id=876; age=21; name="Mary"}
		mary |> toJson)
		
	// curl --data "{\"name\": \"dude\"}" http://yourhosthere/user
	
	// curl --data "{\"name\": \"dude\"}" "http://localhost:8762"
	
//	post "/user" (fun req ->
//		let user = req |> fromJsonOf<User>
//		user |> Dump
//		"hello, new User")
		
	// todo: get static file
	
