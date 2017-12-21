var start = function () {
	var inc = document.getElementById('joined');
	var wsImpl = window.WebSocket || window.MozWebSocket;
	window.ws = new wsImpl('ws://localhost:8181');

	var username = document.getElementById('username').innerHTML;
	var gamename = document.getElementById('gamename').innerHTML;
	var gameStarted = false;

	ws.onmessage = function (evt) {
		var message = evt.data;

		if (!gameStarted) {
			if (message == "admin") {
				document.getElementById('startgame').innerHTML = "<button onclick=\"ws.send('start');\">Start game</button>";
			} else if (message == "wrong-password") {
				inc.innerHTML = "wrong password";
			} else if (message == "username-taken") {
				inc.innerHTML = "username taken";
			} else if (message == "start") {
				document.getElementById('joined').innerHTML = "";
				document.getElementById('startgame').innerHTML = "";
				ws.send("ready");
				gameStarted = true;
				document.getElementById("game").classList.remove("hidden");
			} else {
				inc.innerHTML = message;
			}
			return;
		}

		if (message == "ready") {
			ws.send("ready");
		} else {
			var game = JSON.parse(message);
			console.log(game["Log"][game["Log"].length - 1]);
			
			console.log(game);

			var t = parseInt(game["Turn"]) % Object.keys(game["Players"]).length;
			console.log(game["Players"][Object.keys(game["Players"])[t]]["Username"]);
			
			if (game["Players"][Object.keys(game["Players"])[t]]["Username"] == username) {
				$('#rollDice').attr("disabled", false);
				$(function () {
					$("#rollDice").click(function () {
						ws.send('roll');
					});
				});
			} else {
				$('#rollDice').attr("disabled", true);
				$(function () {
					$("#rollDice").click(function () {});
				});
			}
		}
	};

	ws.onopen = function () {
		inc.innerHTML = 'connection open';
		ws.send("new:" + username + ":" + gamename + ":" + document.getElementById('password').innerHTML);
	};

	ws.onclose = function () {
		inc.innerHTML = 'connection closed';
	};
};
window.onload = start;
