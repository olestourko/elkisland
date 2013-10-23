using UnityEngine;
using System.Collections;

public class State_Hide : State {
	
	
	public override void Enter(BaseAI _ai)
	{
		Debug.Log ("Entered Hide State");
		_ai.Halt();
	}
	
	public override void Execute(BaseAI _ai)
	{
		if(_ai.TargetCanSeeMe())
		{
			//Debug.Log ("Hiding");
		}
		else
		{
			_ai.ChangeState(new State_Approach());
		}
		
	}
	
	public override void Exit(BaseAI _ai)
	{
		Debug.Log ("Exited Hide State");
		_ai.Halt();
	}
}
