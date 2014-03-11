using UnityEngine;
using System.Collections;

public class Exploration_State : State {

	public override void Enter() {}
	public override void Execute() {}
	public override void Exit() {}

	public virtual void Enter(ExperienceManager _manager){}
	public virtual void Execute(ExperienceManager _manager){}
	public virtual void Exit(ExperienceManager _manager){}
}
