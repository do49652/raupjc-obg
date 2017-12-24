/*
Rules:
	different usernames per game
	after starting the game, no joining
	don't reload page when in-game			WIP
	
	@Monologue and @Buy must be last in chain of commands
	@Shop and @Choice can't be in chain
	@End is used for ending turn and is required to be at the end of the behaviour list and only there
	
	random action example:					(recommended use)
		@20%; @Goto -> FirstAction
		@25%; @Goto -> SecondAction
		@50%; @Goto -> ThirdAction
		@05%; @Goto -> FourthAction
		
	@Goto -> funcName acts finds @funcName in the behaviour and jupms there (like B in ARM)
	
	@Buy -> itemName						WIP
	@Move -> nubmerOfSpaces
	


Scenes:
	roll
	rolled
	chance
	event
	shop

Behaviour:
	@Buy
	@Move
	@Use		WIP
	@Log+
	@Log
	@Goto
	@Choice
	@Shop
	@Monologue
	@End
	@Give
	@Remove
	@Money
	
	@OnEvent		(only for Item behaviour)
	@NoEvent		(only for Item behaviour)
	
	TODO:
		@SkipTurn	:)
		@MoveTo -> space
		@SetEvent -> Event
		@GiveEffect -> Effect
		@RemoveEffect -> Effect






















*/