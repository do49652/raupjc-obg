var updateBehaviourEditor = function () {
	$('.behaviourSimpleEditor').each(function () {
		var editor = $(this);
		var behaviour = $('#behaviour').val().split("\n");
		editor.text("");

		for (let i = 0; i < behaviour.length; i++) {
			if (behaviour[i] == "@Begin")
				editor.append('<div class="btn-group"><button class="btn">Begin</button><button class="btn btn-info addNewAction">+</button></div>' + newAction() + '<br>');
			else if (behaviour[i] == "@End")
				editor.append('<div class="btn-group"><button class="btn">End</button></div><br>');
			else if (behaviour[i].indexOf(";") != -1) {
				var str = '<div class="btn-group">';
				var behaviours = behaviour[i].split(";");
				for (let j = 0; j < behaviours.length; j++)
					str += createButton(behaviours[j].trim());
				editor.append(str + '<button class="btn btn-info addNewAction">+</button></div>' + newAction() + '<br>');
			} else if (behaviour[i] == "") {
				editor.append('<div class="btn-group"><button class="btn btn-info addNewAction">+</button></div>' + newAction() + '<br>');
			} else {
				var str = '<div class="btn-group">';
				str += createButton(behaviour[i]);
				editor.append(str + '<button class="btn btn-info addNewAction">+</button></div>' + newAction() + '<br>');
			}
		}
	}).promise().done(function () {
		$('.addNewAction').each(function () {
			var actionButtons = $(this).parent().next();
			var previousActions = [];
				$(this).parent().children().each(function () {
					if ($(this).text() != "+")
						previousActions.push($(this).text());
				});
			actionButtons.text("").append(newAction(previousActions));
			actionButtons.find('*').addClass('hidden');
			
			$(this).off('click').click(function () {
				var actionButtons = $(this).parent().next();

				actionButtons.css('width', 'auto');
				actionButtons.toggleClass('in');
				actionButtons.css('width', 'auto');

				if (actionButtons.hasClass('in'))
					actionButtons.find('*').removeClass('hidden');
				else
					actionButtons.find('*').addClass('hidden');
			});
		});
	}).then(function () {
		$('.newAction').each(function () {
			var actionButton = $(this);
			var actionText = actionButton.text();
			
			actionButton.off('click').click(function(){
				
				
				
				updateBehaviourEditor();
			});
			
		});
	});

	function newAction(previousActions) {
		if (previousActions == null)
			return '<div class="btn-group collapse width" style="width:0px;"></div>';

		var actionButtons = '';

		if (previousActions.length > 0 && previousActions[0].startsWith("Choice: "))
			return '<button class="btn btn-default newAction">New choice</button>';

		if (previousActions.length == 1 && ((previousActions[0].indexOf("%") == -1 && previousActions[0].indexOf(":") == -1) || previousActions[previousActions.length - 1].startsWith("Monologue: ") || previousActions[previousActions.length - 1].startsWith("Buy: ") || previousActions[previousActions.length - 1] == "Shop"))
			return '<button class="btn btn-default newAction">Next row</button>';

		if (previousActions.length == 0)
			actionButtons += '<button class="btn btn-default newAction">XX%</button><button class="btn btn-default newAction">Choice</button><button class="btn btn-default newAction">Shop</button>';

		if (previousActions[0] == "")
			actionButtons += '<button class="btn btn-default newAction">New choice</button>';
		for (let i = 0; i < previousActions.length; i++) {
			if (previousActions[i].startsWith("Monologue") || previousActions[i].startsWith("Buy"))
				return actionButtons + '<button class="btn btn-default newAction">Next row</button>';
		}

		actionButtons += '<button class="btn btn-default newAction">Move</button>';
		actionButtons += '<button class="btn btn-default newAction">Use</button>';
		actionButtons += '<button class="btn btn-default newAction">Log</button>';
		actionButtons += '<button class="btn btn-default newAction">Log+</button>';
		actionButtons += '<button class="btn btn-default newAction">Goto</button>';
		actionButtons += '<button class="btn btn-default newAction">Monologue</button>';
		actionButtons += '<button class="btn btn-default newAction">Give</button>';
		actionButtons += '<button class="btn btn-default newAction">Remove</button>';
		actionButtons += '<button class="btn btn-default newAction">Money</button>';

		actionButtons += '<button class="btn btn-default newAction">Next row</button>';
		return actionButtons;
	}

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
			return '<button class="btn btn-warning">' + action.substring(1).replace(" -> ", ": ") + '</button>';
		else if (action.startsWith("@C") && action.indexOf(" -> ") != -1 && action.substring(2).split(" ")[0].replace(/[0-9]*/, "") == "")
			return '<button class="btn btn-link"></button><button class="btn btn-warning">' + action.split(" -> ")[1] + '</button>';
		else if (action.startsWith("@") && action.replace(/\s/g, "") == action)
			return '<button class="btn btn-primary">' + action.substring(1) + '</button>';
		else
			return '<button class="btn btn-default">' + action.substring(1) + '</button>';
	}
}

$(document).ready(function () {
	updateBehaviourEditor();
});
