module Constants =
	let slash = "/"
	let textHtml = "text/html" // todo: use Fsharp.Data
	
module FsExpress =
	open System
	open System.Net
	open System.Text
	open System.IO
	open Constants

	type Handler = HttpListenerRequest->HttpListenerResponse->Async<unit>
	
	let defaultHost = "http://localhost:8760"
	
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
		
	let get = fun route txt ->
		listener route (fun req resp ->
			let text = txt req
			async {
				let txtBytes = Encoding.ASCII.GetBytes(text:string)
				resp.ContentType <- textHtml
				resp.OutputStream.Write(txtBytes, 0, txtBytes.Length)
				resp.OutputStream.Close()
			})
	
module App =
	open FsExpress
	
	get "/" (fun req -> "Hi!")
	
	get "/home" (fun req -> "hi, from home")
	
	get "/person" (fun req ->
		req.QueryString.["oh"] |> Dump
		"hello, Person")
	// post "/person" // parse json to Person
