using UnityEngine;
using System.Collections;

public class Exploration_State_L2 : Exploration_State {

	public override void Enter(ExperienceManager _manager)
	{
		Debug.Log ("Entered L2");
		gui.location_description = 
			"In Sweden, the will-o'-the-wisp represents the soul of an unbaptized soul trying to lead travellers to water in the hope of being baptized." +
			"Danes, Finns, Swedes, Estonians, Irish people and Latvians amongst some other groups believed that a will-o'-the-wisp also marked the location of a treasure deep in ground or water, which could be taken only when the fire was there. Sometimes magical tricks, and even dead man's hand, were required as well, to uncover the treasure. In Finland and other northern countries it was believed that early autumn was the best time to search for will-o'-the-wisps and treasures below them. It was believed that when someone hid treasure, in the ground, he made the treasure available only at the Saint John's Day, and set will-o'-the-wisp to mark the exact place and time so that he could come to take the treasure back.";
	}
	public override void Execute(ExperienceManager _manager)
	{
		if(!_manager.locations_visited.Contains(_manager.last_visited_location))
		{
			_manager.ChangeState(new Exploration_State_L3());
		}
	}
	public override void Exit(ExperienceManager _manager)
	{
		Debug.Log ("Exited L2");
	}
}
