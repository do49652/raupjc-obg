$(document).ready(function () {
	$("#save-item-behaviour, #save-event-behaviour").off("click").click(function () {
		var l = Ladda.create(this);
		var btn = $(this);

		var behaviour = $("#behaviour").val().split("\n");
		var behaviourTrimmed = "";
		for (let i = 0; i < behaviour.length; i++)
			behaviourTrimmed += "\n" + behaviour[i].trim();
		behaviourTrimmed = behaviourTrimmed.substring(1);

		$.ajax({
			type: "POST",
			url: this.id == "save-item-behaviour" ? "SaveItemBehaviour" : "SaveEventBehaviour",
			beforeSend: function () {
				l.start();
			},
			data: {
				Id: $("#id").val(),
				Name: $("#name").val(),
				Behaviour: behaviourTrimmed
			},
			dataType: "json",
			success: function (res) {
				l.stop();
				btn.text("Saved");
				btn.prop("disabled", true);
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
			console.log(2);
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

	$("#behaviour").keypress(function () {
		$("#save-item-behaviour, #save-event-behaviour").text("Save");
		$("#save-item-behaviour, #save-event-behaviour").prop("disabled", false);
	});

	$("#publish-game").off("click").click(function () {
		var l = Ladda.create(this);
		var obj = {
			Id: $("#id").val(),
			Name: $("#name").val()
		};

		$.ajax({
			type: "POST",
			url: "../PublishGame",
			beforeSend: function () {
				l.start();
			},
			data: obj,
			dataType: "json",
			success: function (res) {
				if (res)
					window.location = "../../ContentCreator";
				l.stop();
			}
		});
	});
	
	$("#remove-game").off("click").click(function () {
		var l = Ladda.create(this);
		var obj = {
			Id: $("#id").val(),
			Name: $("#name").val()
		};

		$.ajax({
			type: "POST",
			url: "../RemoveGame",
			beforeSend: function () {
				l.start();
			},
			data: obj,
			dataType: "json",
			success: function (res) {
				if (res)
					window.location = "../../ContentCreator";
				l.stop();
			}
		});
	});
});
