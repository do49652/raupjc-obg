﻿$(document).ready(function () {
	$("#save-item-behaviour, #save-event-behaviour").off("click").click(function () {
		var l = Ladda.create(this);
		$.ajax({
			type: "POST",
			url: this.id == "save-item-behaviour" ? "SaveItemBehaviour" : "SaveEventBehaviour",
			beforeSend: function () {
				l.start();
			},
			data: {
				Id: $("#id").val(),
				Name: $("#name").val(),
				Behaviour: $("#behaviour").val()
			},
			dataType: "json",
			success: function (res) {
				l.stop();
			}
		});
	});

	$("#save-event-items").off("click").click(function () {
		var l = Ladda.create(this);
		var obj = {
			Id: $("#id").val(),
			Name: $("#name").val(),
			Items: ""
		};

		$('[id^="event-item-"]').each(function () {
			if (this.value == "")
				return;
			if (obj.Items != "")
				obj.Items += "\n";
			obj.Items += this.id.substring(11) + ":" + parseFloat(this.value);
		});

		$.ajax({
			type: "POST",
			url: "SaveEventItems",
			beforeSend: function () {
				l.start();
			},
			data: obj,
			dataType: "json",
			success: function (res) {
				l.stop();
			}
		});
	});

	$("#save-game-startingMoney").off("click").click(function () {
		var l = Ladda.create(this);
		$.ajax({
			type: "POST",
			url: "../SaveGameStartingMoney",
			beforeSend: function () {
				l.start();
			},
			data: {
				Id: $("#id").val(),
				Name: $("#name").val(),
				StartingMoney: parseFloat($("#game-startingMoney").val())
			},
			dataType: "json",
			success: function (res) {
				l.stop();
			}
		});
	});

	$("#save-event-random-set").off("click").click(function () {
		var l = Ladda.create(this);
		var obj = {
			Id: $("#id").val(),
			Name: $("#name").val(),
			SetEvents: "",
			MiniEvents: ""
		};

		$('[id^="event-random-"]').each(function () {
			if (!$(this).is(':checked'))
				return;
			if (obj.MiniEvents != "")
				obj.MiniEvents += "\n";
			obj.MiniEvents += this.id.substring(13);
		});

		$('[id^="event-set-"]').each(function () {
			if (this.value == "")
				return;
			if (obj.SetEvents != "")
				obj.SetEvents += "\n";
			obj.SetEvents += parseInt(this.value) + ":" + this.id.substring(10);
		});

		$.ajax({
			type: "POST",
			url: "../SaveGameRandomSetEvents",
			beforeSend: function () {
				l.start();
			},
			data: obj,
			dataType: "json",
			success: function (res) {
				l.stop();
			}
		});
	});

});
