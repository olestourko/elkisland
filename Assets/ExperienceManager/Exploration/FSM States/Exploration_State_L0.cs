using UnityEngine;
using System.Collections;

public class Exploration_State_L0 : Exploration_State {

	public override void Enter(ExperienceManager _manager)
	{
		Debug.Log ("Entered L0");
	}
	public override void Execute(ExperienceManager _manager)
	{
		if(!_manager.locations_visited.Contains(_manager.last_visited_location))
		{
			_manager.ChangeState(new Exploration_State_L1());
		}
	}
	public override void Exit(ExperienceManager _manager)
	{
		Debug.Log ("Exited L0");
	}
}
