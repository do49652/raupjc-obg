$(document).ready(function () {
	$('.behaviourSimpleEditor').each(function () {
		var editor = $(this);
		var behaviour = $('#behaviour').val().split("\n");

		for (let i = 0; i < behaviour.length; i++) {
			if (behaviour[i] == "@Begin")
				editor.append('<div class="btn-group"><button class="btn">Begin</button><button class="btn btn-info">+</button></div><br>');
			else if (behaviour[i] == "@End")
				editor.append('<div class="btn-group"><button class="btn">End</button></div><br>');
			else if (behaviour[i].indexOf(";") != -1) {
				editor.append('<div class="btn-group">');
				var behaviours = behaviour[i].split(";");
				for (let j = 0; j < behaviours.length; j++)
					editor.append(createButton(behaviours[j].trim()));
				editor.append('<button class="btn btn-info">+</button></div><br>');
			} else if (behaviour[i] == "") {
				editor.append('<br>');
			} else {
				editor.append('<div class="btn-group">');
				editor.append(createButton(behaviour[i]));
				editor.append('<button class="btn btn-info">+</button></div><br>');
			}
		}
	}).promise().done(function () {});

	function createButton(action) {
		if (action.startsWith("@Buy") || action.startsWith("@Remove") || action.startsWith("@Give"))
			return '<button class="btn btn-default">' + action.split(" -> ")[0].substring(1) + ': <span style="color: magenta";>' + action.split(" -> ")[1] + '</span></button>';
		else if (action.startsWith("@OnEvent"))
			return '<button class="btn btn-default">' + action.split(" -> ")[0].substring(1) + ': <span style="color: red";>' + action.split(" -> ")[1] + '</span></button>';
		else if (action.startsWith("@NoEvent") || action.startsWith("@Goto"))
			return '<button class="btn btn-default">' + action.split(" -> ")[0].substring(1) + ': <span style="color: blue";>' + action.split(" -> ")[1] + '</span></button>';
		else if (action.startsWith("@Move") || action.startsWith("@Money") || action.startsWith("@Log") || action.startsWith("@Monologue"))
			return '<button class="btn btn-default">' + action.split(" -> ")[0].substring(1) + ': ' + action.split(" -> ")[1] + '</button>';
		else if (action.startsWith("@Choice"))
			return '<button class="btn btn-warning">' + action.split(" -> ")[1] + '</button>';
		else if (action.startsWith("@C") && action.indexOf(" -> ") != -1 && action.substring(2).split(" ")[0].replace(/[0-9]*/, "") == "")
			return '<button class="btn btn-link"></button><button class="btn btn-warning">' + action.split(" -> ")[1] + '</button>';
		else if (action.startsWith("@") && action.replace(/\s/g, "") == action)
			return '<button class="btn btn-primary">' + action.substring(1) + '</button>';
		else
			return '<button class="btn btn-default">' + action.substring(1) + '</button>';
	}
});
