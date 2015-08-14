module FsExpress =
	type App = {route:string}
	type Req = {host:string; route:string}
	
	let send text =
		printfn text
	
	let send2 text code headers =
		printf text
		printf "%i"code
		printf headers
	
	let get = fun route handle ->
		let req = {host="localhost"; route="/home"}
		if route = req.route then handle req "dummy"
	
module App = 
	open FsExpress
	
	get "/"
		(fun req res ->
			printf "%A" req.host
			printf "%A" req.route
			send "Hello!")
	
	get "/home"
		(fun req res ->
			printf "%A" req.host
			printf "%A" req.route
			send2 "Hi, Home!" 200 "{'Content-Type': 'text/plain'}")
