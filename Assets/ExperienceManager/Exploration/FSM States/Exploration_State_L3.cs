using UnityEngine;
using System.Collections;

public class Exploration_State_L3 : Exploration_State {

	public override void Enter(ExperienceManager _manager)
	{
		Debug.Log ("Entered L3");
		gui.location_description = 
			"Mexico has its own equivalent as well. Folklore explains will-o-the-wisp to be witches who transformed into these lights. The reason for this, however, varies according to the region." +
			"The swampy area of Massachusetts known as the Bridgewater Triangle has folklore of ghostly orbs of light, and there have been modern observations of these ghost-lights in this area as well.";
	}
	public override void Execute(ExperienceManager _manager)
	{
		/*
		if(!_manager.locations_visited.Contains(_manager.last_visited_location))
		{
			_manager.ChangeState(new Exploration_State_L3());
		}
		*/
	}
	public override void Exit(ExperienceManager _manager)
	{
		Debug.Log ("Exited L3");
	}
}
