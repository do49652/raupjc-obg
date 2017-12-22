﻿var start = function () {
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
				document.getElementById('startgame').innerHTML = "<button class='btn btn-primary' onclick=\"ws.send('start');\">Start game</button>";
			} else if (message == "wrong-password") {
				inc.innerHTML = "wrong password";
			} else if (message == "username-taken") {
				inc.innerHTML = "username taken";
			} else if (message == "start") {
				document.getElementById("lobby").classList.add("hidden");
				document.getElementById('title').innerHTML = gamename;
				document.getElementById('desc').innerHTML = "";
				ws.send("ready");
				gameStarted = true;
				document.getElementById("game").classList.remove("hidden");
			} else {
				inc.innerHTML = message;
				document.getElementById('desc').innerHTML = "Waiting for all players to join.";
			}
			return;
		}

		if (message == "ready") {
			ws.send("ready");
		} else {
			var game = JSON.parse(message);

			$(function () {
				$('#clipboard').text(message);

				var log = "";
				for (let i = 0; i < game["Log"].length; i++) {
					log += game["Log"][i] + "\n";
				}
				$('#log').text(log);
			});

			var t = parseInt(game["Turn"]) % Object.keys(game["Players"]).length;
			var playingUsername = game["Players"][Object.keys(game["Players"])[t]]["Username"];

			var sceneManager = function () {
				if (game["Scene"] == "roll") {
					if (playingUsername == username) {
						$(function () {
							$('#rollDice').attr("disabled", false);

							$("#rollDice").off('click').click(function () {
								ws.send('roll');

								$('#rollDice').attr("disabled", true);
								$("#rollDice").off('click');
							});
						});
					} else {
						$(function () {
							$('#rollDice').attr("disabled", true);
							$("#rollDice").off('click');
						});
					}
				} else {
					if (playingUsername == username) {
						$(function () {
							if (game["Scene"] == "rolled")
								$('#message').text("You rolled " + game["LastRoll"] + ".");
							else if (game["Scene"] == "event")
								$('#message').text(game["Message"]);

							$("#proceed").off('click').click(function () {
								ws.send('move');
							});
						});
					} else {
						$(function () {
							if (game["Scene"] == "rolled")
								$('#message').text(playingUsername + " rolled " + game["LastRoll"] + ".");
							else if (game["Scene"] == "event")
								$('#message').text(game["Message"]);

							$('#proceed').attr("disabled", true);
							$("#proceed").off('click');
						});
					}
				}
			}

			$("#game").load("/html/" + game["Scene"] + ".html", sceneManager);
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
