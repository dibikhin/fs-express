//module FsExpress =
//	type Status = Ok=200 | NotFound=404
////	type ContentType = PlainText="asdf"
//	
//	type ContentType =
//  		| PlainText
//  		override this.ToString() =
//    		match this with
//    		| PlainText -> "Content-Type: text/plain"
//	
////	printfn "%O" ContentType.PlainText
//	
//	type Req = {Host:string; Route:string}
//	
//	type Response = {Data:string; Status:Status; ContentType:ContentType}
//	
//	let defaultResponse = {Data=""; Status=Status.Ok; ContentType=ContentType.PlainText}
//	
//	let send res =
//		printf "%A" res
//	
//	let get = fun route handle ->
//		let req = {Host="localhost"; Route="/home"}
//		if route = req.Route then handle req "dummy"
	
module FsExpress =
	open System
	open System.Net
	open System.Text
	open System.IO
 
	let host = "http://localhost:8765"
 
	let listener (route:string) (handler:(HttpListenerRequest->HttpListenerResponse->Async<unit>)) =
		let hl = new HttpListener()
		hl.Prefixes.Add (host + route)
		hl.Start()
		
		let task = Async.FromBeginEnd(hl.BeginGetContext, hl.EndGetContext)
		async {
			while true do
				let! context = task
				Async.Start(handler context.Request context.Response)
		} |> Async.Start
		
	let get (route:string) (txt:string) =
		listener route (fun req resp ->
			async {
				let txtBytes = Encoding.ASCII.GetBytes(txt)
//				let txt = Encoding.ASCII.GetBytes(DateTime.Now.ToString())				
				resp.ContentType <- "text/html"
				resp.OutputStream.Write(txtBytes, 0, txtBytes.Length)
				resp.OutputStream.Close()
			})
	
module App =
	open FsExpress
	
	get "/" "Hi!"
	
	get "/home/" "hi, from home"
	
//	get "/"
//		(fun req res ->
//			printf "%A" req.Host
//			printf "%A" req.Route
//			send {defaultResponse with Data="Hello:)"})
//	
//	get "/home"
//		(fun req res ->
//			printf "%A" req.Host
//			printf "%A" req.Route
//			send {defaultResponse with Data="Hi, Home!"})
