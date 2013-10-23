using UnityEngine;
using System.Collections;

public class State_Approach : State {
	
	public override void Enter(BaseAI _ai)
	{
		Debug.Log ("Entered Approach State");
	}
	
	public override void Execute(BaseAI _ai)
	{
		if(_ai.TargetCanSeeMe())
		{
			_ai.ChangeState(new State_Hide());
		}
		else
		{
			_ai.ApproachTarget();
		}
	}
	
	public override void Exit(BaseAI _ai)
	{
		Debug.Log ("Exited Approach State");
	}
	
}
