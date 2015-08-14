module Fexpress =
	type App = {route:string}
	type Req = {host:string; query:string}
	
	let send x =
		printfn x
	
	let get = fun route handle ->
		let req = {host="localhost"; query="/"}
		handle req "dummy"
	
module App = 
	open Fexpress
	
	get "/"
		(fun req res ->
			printfn "%A" req.host
			send "Hello!")
		
//	get "/home"
//		(fun req res -> send "hi, home!")
//	
//	let app = App
//	app "customers"
//		{create: fun req -> ignore;
//		 update: fun req -> ignore;
//		 remove: fun req -> ignore}
//	

//var express = require('express');
//var app = express();
//
//app.get('/', function (req, res) {
//  res.send('Hello World!');
//});
//
//var server = app.listen(3000, function () {
//  var host = server.address().address;
//  var port = server.address().port;
//
//  console.log('Example app listening at http://%s:%s', host, port);
//});
