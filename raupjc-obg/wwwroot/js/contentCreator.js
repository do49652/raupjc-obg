$(document).ready(function () {
	$("#save-item-behaviour").off("click").click(function () {
		var l = Ladda.create(this);
		$.ajax({
			type: "POST",
			url: "SaveItemBehaviour",
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

	$("#save-event-behaviour").off("click").click(function () {
		var l = Ladda.create(this);
		$.ajax({
			type: "POST",
			url: "SaveEventBehaviour",
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
});
