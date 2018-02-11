var updateBehaviourEditor = function () {
	$('.behaviourSimpleEditor').each(function () {
		var editor = $(this);
		var behaviour = $('#behaviour').val().split("\n");
		editor.text("");

		for (let i = 0; i < behaviour.length; i++) {
			if (behaviour[i] == "@Begin")
				editor.append('<div class="btn-group"><button class="btn">Begin</button><button class="btn btn-info addNewAction">+</button></div>' + newAction(null, i) + '<br>');
			else if (behaviour[i] == "@End")
				editor.append('<div class="btn-group"><button class="btn">End</button></div><br>');
			else if (behaviour[i].indexOf(";") != -1) {
				var str = '<div class="btn-group">';
				var behaviours = behaviour[i].split(";");
				for (let j = 0; j < behaviours.length; j++)
					str += createButton(behaviours[j].trim());
				editor.append(str + '<button class="btn btn-info addNewAction">+</button></div>' + newAction(null, i) + '<br>');
			} else if (behaviour[i] == "") {
				editor.append('<div class="btn-group"><button class="btn btn-info addNewAction">+</button></div>' + newAction(null, i) + '<br>');
			} else {
				var str = '<div class="btn-group">';
				str += createButton(behaviour[i]);
				editor.append(str + '<button class="btn btn-info addNewAction">+</button></div>' + newAction(null, i) + '<br>');
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

				if (actionButtons.hasClass('in')) {
					actionButtons.find('*').removeClass('hidden');
					actionButtons.css('padding', '10px');
				} else {
					actionButtons.find('*').addClass('hidden');
					actionButtons.css('padding', '0');
				}
			});
		});
	}).then(function () {
		$('.newAction').each(function () {
			var actionButton = $(this);
			var actionText = actionButton.text();
			var textEditor = $('#behaviour').val().split("\n");

			actionButton.off('click').click(function () {
				var line = parseInt(actionButton.parent().data("line"));

				switch (actionText) {
					case "Next row":
						textEditor[line] += "\n";
						break;
					case "Move":
						textEditor[line] += (textEditor[line] == "" ? "" : "; ") + "@Move -> 0";
						break;
					case "Log":
						textEditor[line] += (textEditor[line] == "" ? "" : "; ") + "@Log -> Some log.";
						break;
					case "Log+":
						textEditor[line] += (textEditor[line] == "" ? "" : "; ") + "@Log+ -> Player says...";
						break;
					case "Goto":
						textEditor[line] += (textEditor[line] == "" ? "" : "; ") + "@Goto -> Section";
						break;
					case "Monologue":
						textEditor[line] += (textEditor[line] == "" ? "" : "; ") + "@Monologue -> Text.";
						break;
					case "GiveItem":
						textEditor[line] += (textEditor[line] == "" ? "" : "; ") + "@GiveItem -> Item";
						break;
					case "RemoveItem":
						textEditor[line] += (textEditor[line] == "" ? "" : "; ") + "@RemoveItem -> Item";
						break;
					case "Money":
						textEditor[line] += (textEditor[line] == "" ? "" : "; ") + "@Money -> +0";
						break;
					case "Section":
						textEditor[line] += (textEditor[line] == "" ? "" : "; ") + "@Section";
						break;
					case "Shop":
						textEditor[line] += (textEditor[line] == "" ? "" : "; ") + "@Shop";
						break;
					case "OnEvent":
						textEditor[line] += (textEditor[line] == "" ? "" : "; ") + "@OnEvent -> Event";
						break;
					case "HasItem":
						textEditor[line] += (textEditor[line] == "" ? "" : "; ") + "@HasItem -> Item";
						break;
					case "NoEvent":
						textEditor[line] += (textEditor[line] == "" ? "" : "; ") + "@NoEvent -> Section";
						break;
					case "XX%":
						textEditor[line] += (textEditor[line] == "" ? "" : "; ") + "@50%";
						break;
					case "Choice":
						textEditor[line] += (textEditor[line] == "" ? "" : "; ") + "@Choice -> Choose something.";
						break;
					case "ChoosePlayer":
						textEditor[line] += (textEditor[line] == "" ? "" : "; ") + "@ChoosePlayer -> 150";
						break;
					case "New choice":
						if (textEditor[line].startsWith("@C8"))
							break;
						if (textEditor[line].startsWith("@Choice")) {
							textEditor[line] += "\n@C1 -> Choice";
							break;
						}
						textEditor[line] += "\n@C" + (parseInt(textEditor[line].substring(2).split(" -> ")[0]) + 1) + " -> Choose something.";
						break;
				}

				updateTextEditor(textEditor);
				updateBehaviourEditor();
			});

		});
	}).then(function () {
		$('.bAction').each(function () {
			var bAction = $(this);
			var textEditor = $('#behaviour').val().split("\n");
			var line = parseInt(bAction.parent().next().data("line"));
			var text = bAction.find('span').html();

			bAction.on("keydown", function (e) {
				if (e.which == 32) {
					var input = bAction.find('span').find('input');
					e.preventDefault();
					var cursorI = getCursorPosition(input);
					input.val(input.val().substring(0, cursorI) + " " + input.val().substring(cursorI));
					input.setCursorPosition(cursorI + 1);
				}
			});

			bAction.find('span.edit').editable({
				type: 'text',
				value: text,
				success: function (response, newValue) {
					newValue = newValue.replace(/[>;@]*/g, "");
					if (!textEditor[line].startsWith("@Monologue -> "))
						newValue = newValue.replace(/:*/g, "");
					newValue = newValue.replace("https//", "https://");
					textEditor[line] = textEditor[line].replace("https//", "https://");

					if (newValue == "")
						newValue = "Empty";

					textEditor[line] = textEditor[line].split('').reverse().join('').replace(text.split('').reverse().join(''), newValue.split('').reverse().join('')).split('').reverse().join('');

					updateTextEditor(textEditor);
					updateBehaviourEditor();

					$("#save-item-behaviour, #save-event-behaviour").text("Save");
					//$("#save-item-behaviour, #save-event-behaviour").prop("disabled", false);
				}
			});

			bAction.find('span.glyphicon.glyphicon-remove').off('click').click(function () {
				var subBegin = textEditor[line].substring(0, textEditor[line].indexOf(text) + 1).lastIndexOf('@');
				var subEnd = textEditor[line].indexOf(text) + text.length;

				textEditor[line] = textEditor[line].split('').reverse().join('').replace(textEditor[line].substring(subBegin, subEnd).split('').reverse().join(''), '').split('').reverse().join('').trim();

				textEditor[line] = textEditor[line].replace(' ;', ';');
				while (textEditor[line].indexOf(';;') != -1)
					textEditor[line] = textEditor[line].replace(';;', ';');
				
				if (textEditor[line][0] == ';')
					textEditor[line] = textEditor[line].substring(1);
				if (textEditor[line][textEditor[line].length - 1] == ';')
					textEditor[line] = textEditor[line].substring(0, textEditor[line].length - 1);

				updateTextEditor(textEditor);
				updateBehaviourEditor();
			});

		});
	});

	function newAction(previousActions, line) {
		if (previousActions == null)
			return '<div data-line="' + line + '" class="btn-group collapse width row col-xs-offset-1 well" style="width:0px; padding:0; margin:0;"></div>';

		var actionButtons = '';

		if (previousActions.length > 0 && previousActions[0].startsWith("Choice: "))
			return '<button class="btn btn-default newAction">New choice</button>';

		if (previousActions.length == 1 && previousActions[0].startsWith("ChoosePlayer: "))
			return '<button class="btn btn-default newAction">Move</button><button class="btn btn-default newAction">GiveItem</button><button class="btn btn-default newAction">RemoveItem</button><button class="btn btn-default newAction">Money</button>';

		if (previousActions.length > 1 && previousActions[0].startsWith("ChoosePlayer: "))
			return '<button class="btn btn-default newAction">Next row</button>';

		if (previousActions.length == 1 && ((previousActions[0].indexOf("%") == -1 && previousActions[0].indexOf(":") == -1) || previousActions[previousActions.length - 1].startsWith("NoEvent: ") || previousActions[previousActions.length - 1].startsWith("Monologue: ") || previousActions[previousActions.length - 1].startsWith("Buy: ") || previousActions[previousActions.length - 1] == "Shop"))
			return '<button class="btn btn-default newAction">Next row</button>';

		if (previousActions.length == 0)
			actionButtons += (window.location.pathname.substring(window.location.pathname.lastIndexOf("/") + 1) == "Item" ? '<button class="btn btn-default newAction">OnEvent</button><button class="btn btn-default newAction">NoEvent</button>' : '') +
			'<button class="btn btn-default newAction">HasItem</button><button class="btn btn-default newAction">Section</button><button class="btn btn-default newAction">XX%</button><button class="btn btn-default newAction">Choice</button><button class="btn btn-default newAction">ChoosePlayer</button><button class="btn btn-default newAction">Shop</button>';

		if (previousActions[0] == "")
			actionButtons += '<button class="btn btn-default newAction">New choice</button>';
		for (let i = 0; i < previousActions.length; i++) {
			if (previousActions[i].startsWith("Monologue") || previousActions[i].startsWith("Buy"))
				return actionButtons + '<button class="btn btn-default newAction">Next row</button>';
		}

		actionButtons += '<button class="btn btn-default newAction">Move</button>';
		actionButtons += '<button class="btn btn-default newAction">Log</button>';
		actionButtons += '<button class="btn btn-default newAction">Log+</button>';
		actionButtons += '<button class="btn btn-default newAction">Goto</button>';
		actionButtons += '<button class="btn btn-default newAction">Monologue</button>';
		actionButtons += '<button class="btn btn-default newAction">GiveItem</button>';
		actionButtons += '<button class="btn btn-default newAction">RemoveItem</button>';
		actionButtons += '<button class="btn btn-default newAction">Money</button>';

		actionButtons += '<button class="btn btn-default newAction">Next row</button>';
		return actionButtons;
	}

	function createButton(action) {
		if (action.startsWith("@Buy") || action.startsWith("@RemoveItem") || action.startsWith("@GiveItem"))
			return '<button disabled class="btn btn-default bAction">' + action.split(" -> ")[0].substring(1) + ': <span class="edit" style="color: magenta;">' + action.split(" -> ")[1] + '</span> <span class="glyphicon glyphicon-remove" style="color:red"></span></button>';
		else if (action.startsWith("@OnEvent") || action.startsWith("@HasItem"))
			return '<button disabled class="btn btn-default bAction">' + action.split(" -> ")[0].substring(1) + ': <span class="edit" style="color: red;">' + action.split(" -> ")[1] + '</span> <span class="glyphicon glyphicon-remove" style="color:red"></span></button>';
		else if (action.startsWith("@NoEvent") || action.startsWith("@Goto"))
			return '<button disabled class="btn btn-default bAction">' + action.split(" -> ")[0].substring(1) + ': <span class="edit" style="color: blue;">' + action.split(" -> ")[1] + '</span> <span class="glyphicon glyphicon-remove" style="color:red"></span></button>';
		else if (action.startsWith("@Move") || action.startsWith("@Money") || action.startsWith("@Log") || action.startsWith("@Monologue"))
			return '<button disabled class="btn btn-default bAction">' + action.split(" -> ")[0].substring(1) + ': <span class="edit">' + action.split(" -> ")[1] + '</span> <span class="glyphicon glyphicon-remove" style="color:red"></span></button>';
		else if (action.startsWith("@Choice"))
			return '<button disabled class="btn btn-warning bAction">' + action.substring(1).split(" -> ")[0] + ': <span class="edit">' + action.substring(1).split(" -> ")[1] + '</span> <span class="glyphicon glyphicon-remove" style="color:red"></span></button>';
		else if (action.startsWith("@ChoosePlayer"))
			return '<button disabled class="btn btn-warning bAction">' + action.substring(1).split(" -> ")[0] + ': <span class="edit">' + action.substring(1).split(" -> ")[1] + '</span ><span class="glyphicon glyphicon-remove" style="color:red"></span></button>';
		else if (action.startsWith("@C") && action.indexOf(" -> ") != -1 && action.substring(2).split(" ")[0].replace(/[0-9]*/, "") == "")
			return '<button disabled class="btn btn-link"></button><button class="btn btn-warning bAction"><span class="edit">' + action.split(" -> ")[1] + '</spam> <span class="glyphicon glyphicon-remove" style="color:red"></span></button>';
		else if (action.startsWith("@") && action.replace(/\s/g, "") == action)
			return '<button disabled class="btn btn-primary bAction"><span class="edit">' + action.substring(1) + '</span> <span class="glyphicon glyphicon-remove" style="color:red"></span></button>';
		else
			return '<button disabled class="btn btn-default bAction"><span class="edit">' + action.substring(1) + '</span> <span class="glyphicon glyphicon-remove" style="color:red"></span></button>';
	}
}

function updateTextEditor(rows) {
	var str = "";
	for (let i = 0; i < rows.length; i++)
		str += "\n" + rows[i];
	$('#behaviour').val(str.substring(1));
}

$(document).ready(function () {
	$('#switchToEditor').click(function () {
		updateBehaviourEditor();
	});
	$.fn.editable.defaults.mode = 'inline';
	updateBehaviourEditor();
});

function getCursorPosition(jqueryItem) {
	var input = jqueryItem.get(0);
	if (!input)
		return;
	if ('selectionStart' in input) {
		return input.selectionStart;
	} else if (document.selection) {
		input.focus();
		var sel = document.selection.createRange();
		var selLen = document.selection.createRange().text.length;
		sel.moveStart('character', -input.value.length);
		$("#sel").html(sel);
		$("#selLen").html(selLen);
		return sel.text.length - selLen;
	}
}
$.fn.setCursorPosition = function (pos) {
	this.each(function (index, elem) {
		if (elem.setSelectionRange) {
			elem.setSelectionRange(pos, pos);
		} else if (elem.createTextRange) {
			var range = elem.createTextRange();
			range.collapse(true);
			range.moveEnd('character', pos);
			range.moveStart('character', pos);
			range.select();
		}
	});
	return this;
};