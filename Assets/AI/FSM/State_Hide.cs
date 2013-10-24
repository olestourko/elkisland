using UnityEngine;
using System.Collections;

public class State_Hide : State {
	
	//private Vector3 hide_target;
	
	public override void Enter(BaseAI _ai)
	{
		Debug.Log ("Entered Hide State");
		_ai.Halt();
		_ai.attack_range = 0.75f;
	}
	
	public override void Execute(BaseAI _ai)
	{
		float d = Vector3.Distance(_ai.transform.position, new Vector3(_ai.target.position.x, 0, _ai.target.position.z));
		if(d < _ai.attack_range)
		{
			_ai.attack_range = 1.5f;
			_ai.ChangeState(new State_Attack());	
		}
		else if(_ai.TargetCanSeeMe())
		{
			
			Vector3 closest_hide_target = Vector3.left * 65536.0f;
			foreach(Vector3 obstacle in _ai.obstacles)
			{
				//Highlight obstacle position
				//Debug.DrawLine(obstacle + Vector3.right*0.5f, obstacle - Vector3.right*0.5f, Color.green);
				//Debug.DrawLine(obstacle + Vector3.forward*0.5f, obstacle - Vector3.forward*0.5f, Color.green);
				
				//Find and highlight hiding position
				Vector3 v1 = new Vector3(_ai.target.position.x, 0, _ai.target.position.z);
				Vector3 v2 = (obstacle - v1).normalized;
				Vector3 v3 = obstacle + v2 * 0.25f;
				//Debug.DrawLine(_ai.target.transform.position, v3, Color.red);
				if(Vector3.Distance(_ai.transform.position, closest_hide_target) > Vector3.Distance(_ai.transform.position, v3)) closest_hide_target = v3;
			}
			_ai.ApproachTarget(closest_hide_target);
			Debug.DrawLine(closest_hide_target + Vector3.right*0.5f, closest_hide_target - Vector3.right*0.5f, Color.blue);
			Debug.DrawLine(closest_hide_target + Vector3.forward*0.5f, closest_hide_target - Vector3.forward*0.5f, Color.blue);
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
