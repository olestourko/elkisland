using UnityEngine;
using System.Collections;

public class State_Attack : State {

	public override void Enter(BaseAI _ai)
	{
		Debug.Log ("Entered Attack State");
		_ai.max_speed = 1.5f;
		_ai.PlayAttackSound();
	}
	
	public override void Execute(BaseAI _ai)
	{
		Vector3 v1 = new Vector3(_ai.target.position.x, 0, _ai.target.position.z);
		float d = Vector3.Distance(_ai.transform.position, v1);
		
		if(d > 1.5f)
		{
			_ai.ChangeState(new State_Approach());
		}
		else
		{
			_ai.ApproachTarget(_ai.target.position);
			Debug.DrawLine(_ai.transform.position, v1, Color.red);
		}
	}
	
	public override void Exit(BaseAI _ai)
	{
		Debug.Log ("Exited Attack State");
		_ai.max_speed = 0.5f;
	}
}
