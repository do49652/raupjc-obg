var start = function () {
	var inc = document.getElementById('joined');
	var wsImpl = window.WebSocket || window.MozWebSocket;
	window.ws = new wsImpl('ws://192.168.1.5:8181');

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
				document.getElementById("gameGameContainer").classList.remove("hidden");
			} else {
				inc.innerHTML = message;
				document.getElementById('desc').innerHTML = "Waiting for all players to join.";
			}
			return;
		}

		if (message == "ready") {
			ws.send("ready");
		} else if (message.startsWith("item:")) {
			console.log(message);
			$(function () {
				$('#itemModal .modal-title').text(message.split(":")[1]);

				$('#itemModal .modal-body').text("").append(message.split(":")[2] + "<br>");
				$('#itemModal .modal-body').append('<button class="btn btn-default" id="item1">Continue</button>');
				$('#itemModal .modal-body #item1').off('click').click(function () {
					ws.send('item:Zet karta');
				});

				var msg = message.split(":")[2].trim();

				if (msg.startsWith("@Choice")) {
					$("#itemModal .modal-body").load("/html/choice.html", () => {

						var title = msg.split("\n")[0].split("->")[1].trim();
						var choices = [];

						for (let i = 1; i < msg.split("\n").length; i++) {
							choices.push({
								n: parseInt(msg.split("\n")[i].split("->")[0].replace("@C", "").trim()),
								text: msg.split("\n")[i].split("->")[1].split(";")[0].trim(),
								action: msg.split("\n")[i].substring(msg.split("\n")[i].indexOf(";") + 1).trim()
							});
						}

						$('#itemModal .modal-body #message').text("").append(title);

						for (let i = 1; i <= 8; i++) {
							$('#itemModal .modal-body #proceed' + i).addClass("hidden");
							$("#itemModal .modal-body #proceed" + i).off('click');
						}

						for (i in choices) {
							$('#itemModal .modal-body #proceed' + choices[i].n).removeClass("hidden");
							$('#itemModal .modal-body #proceed' + choices[i].n).text("").append(choices[i].text);
							$('#itemModal .modal-body #proceed' + choices[i].n).data('move', choices[i].action);
							$("#itemModal .modal-body #proceed" + choices[i].n).off('click').click(function () {
								ws.send('item:' + message.split(":")[1] + ':' + $(this).data('move'));
							});
						}

					});

				}

				$('#itemModal').modal();

				if (message.split(":")[2].trim() == "@End") {
					$('#itemModal .modal-body').text("");
					$('#itemModal').modal("hide");
				}

			});
		} else {
			var game = JSON.parse(message);
			console.log(game);

			$(function () {
				$('#clipboard').text(message);

				var log = "";
				for (let i = 0; i < game["Log"].length; i++)
					log += '<span data-toggle="tooltip" data-placement="left auto" data-container="body" title="' + game["Log"][i].split(']')[0].substring(1) + '">' + game["Log"][i].replace(/\[([a-z0-9_ :-]*)\]/i, '') + '</span><br>';
				$('#log').text("").append(log);
				$('[data-toggle="tooltip"]').tooltip();

				log = "";
				for (let i = 0; i < Object.keys(game["Players"]).length; i++)
					log += Object.keys(game["Players"])
					[i] + ": " + game["Players"][Object.keys(game["Players"])[i]]["Space"] + "\n";

				$('#players').text(log);

				log = "<p>Money: " + game["Players"][username]["Money"] + "</p>";
				for (let i = 0; i < game["Players"][username]["Items"].length; i++)
					log += '<button class="btn btn-default" id="item' + (i + 1) + '">' + game["Players"][username]["Items"][i]["Name"] + '</button>';

				$('#items').text("").append(log);
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
				} else if (game["Scene"] == "choice") {
					$(function () {
						var title = game["Message"].split("\n")[0].split("->")[1].trim();
						var choices = [];

						for (let i = 1; i < game["Message"].split("\n").length; i++) {
							choices.push({
								n: parseInt(game["Message"].split("\n")[i].split("->")[0].replace("@C", "").trim()),
								text: game["Message"].split("\n")[i].split("->")[1].split(";")[0].trim(),
								action: game["Message"].split("\n")[i].substring(game["Message"].split("\n")[i].indexOf(";") + 1).trim()
							});
						}

						$('#message').text("").append(title);

						for (let i = 1; i <= 8; i++) {
							$('#proceed' + i).addClass("hidden");
							$('#proceed' + i).attr("disabled", true);
							$("#proceed" + i).off('click');
						}

						for (i in choices) {
							$('#proceed' + choices[i].n).removeClass("hidden");
							$('#proceed' + choices[i].n).text("").append(choices[i].text);
							if (playingUsername == username) {
								$('#proceed' + choices[i].n).data('move', choices[i].action);
								$('#proceed' + choices[i].n).attr("disabled", false);
								$("#proceed" + choices[i].n).off('click').click(function () {
									ws.send('move:' + $(this).data('move'));
								});
							}
						}
					});
				} else if (game["Scene"] == "shop") {
					$(function () {
						var title = game["Message"];
						var items = [];

						for (let i = 0; i < Object.keys(game["Game"]["Items"]).length; i++) {
							items.push({
								name: Object.keys(game["Game"]["Items"])[i],
								description: game["Game"]["Items"][Object.keys(game["Game"]["Items"])[i]][0]["Description"],
								price: parseFloat(game["Game"]["Items"][Object.keys(game["Game"]["Items"])[i]][1])
							});
						}

						$('#message').text("").append(title);

						for (let i = 1; i <= 8; i++) {
							$('#proceed' + i).addClass("hidden");
							$('#proceed' + i).attr("disabled", true);
							$("#proceed" + i).off('click');
						}

						for (i in items) {
							i = parseInt(i);
							$('#proceed' + (i + 1)).removeClass("hidden");
							$('#proceed' + (i + 1)).text("").append(items[i].name + " <i>(Cost: " + items[i].price + ")</i>");
							if (playingUsername == username && items[i].price <= parseFloat(game["Players"][username]["Money"])) {
								$('#proceed' + (i + 1)).data('move', "@Buy -> " + items[i].name);
								$('#proceed' + (i + 1)).attr("disabled", false);
								$("#proceed" + (i + 1)).off('click').click(function () {
									ws.send('move:' + $(this).data('move'));
								});
							}
						}

						$('#cancel').addClass("hidden");
						$('#cancel').attr("disabled", true);
						$('#cancel').off('click');
						if (playingUsername == username) {
							$('#cancel').removeClass("hidden");
							$('#cancel').attr("disabled", false);
							$('#cancel').text("Cancel");
							$("#cancel").off('click').click(function () {
								ws.send('move');
							});
						}
					});
				} else {
					$(function () {
						if (game["Scene"] == "rolled")
							$('#message').text("You rolled " + game["LastRoll"] + ".");
						else if (game["Scene"] == "event")
							$('#message').text("").append(game["Message"]);

						if (playingUsername == username) {
							$("#proceed").off('click').click(function () {
								ws.send('move');
							});
						} else {
							$('#proceed').attr("disabled", true);
							$("#proceed").off('click');
						}
					});
				}
				
				$(function () {
					for (let i = 0; i < game["Players"][username]["Items"].length; i++) {
						$('#item' + (i + 1)).off('click').click(function () {
							ws.send('item:' + game["Players"][username]["Items"][i]["Name"]);
						});
					}
				});
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
