using UnityEngine;
using System.Collections;

public class Exploration_State_L1 : Exploration_State {

	public override void Enter(ExperienceManager _manager)
	{
		Debug.Log ("Entered L1");
		gui.location_description = 
			"In European folklore, these lights are held to be either mischievous spirits of the dead, or other supernatural beings or spirits such as fairies, attempting to lead travellers astray.\n\n" +
			"A modern Americanized adaptation of this travellers' association frequently places swaying ghost-lights along roadsides and railroad tracks. Here a swaying movement of the lights is alleged to be that of 19th- and early 20th-century railway workers supposedly killed on the job.\n\n" +
			"Sometimes the lights are believed to be the spirits of unbaptized or stillborn children, flitting between heaven and hell. Modern occultist elaborations bracket them with the salamander, a type of spirit wholly independent from humans (unlike ghosts, which are presumed to have been humans at some point in the past).\n\n";
	}
	public override void Execute(ExperienceManager _manager)
	{
		if(!_manager.locations_visited.Contains(_manager.last_visited_location))
		{
			_manager.ChangeState(new Exploration_State_L2());
		}
	}
	public override void Exit(ExperienceManager _manager)
	{
		Debug.Log ("Exited L1");
	}

}
