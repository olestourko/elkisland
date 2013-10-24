using UnityEngine;
using System.Collections;

public class State_Approach : State {
	
	public override void Enter(BaseAI _ai)
	{
		Debug.Log ("Entered Approach State");
	}
	
	public override void Execute(BaseAI _ai)
	{
		float d = Vector3.Distance(_ai.transform.position, new Vector3(_ai.target.transform.position.x, 0, _ai.target.transform.position.z));
		
		if(d <= 1.5f)
		{
			_ai.ChangeState(new State_Attack());
		}
		else if(_ai.TargetCanSeeMe())
		{
			_ai.ChangeState(new State_Hide());
		}
		else
		{
			Vector3 closest_position = Vector3.left * 65536.0f;
			foreach(Vector3 obstacle in _ai.obstacles)
			{
				//Highlight obstacle position
				//Debug.DrawLine(obstacle + Vector3.right*0.5f, obstacle - Vector3.right*0.5f, Color.green);
				//Debug.DrawLine(obstacle + Vector3.forward*0.5f, obstacle - Vector3.forward*0.5f, Color.green);
				
				//Find and highlight closest obstacle-adjacent postion to player
				Vector3 v1 = new Vector3(_ai.target.position.x, 0, _ai.target.position.z);
				Vector3 v2 = (obstacle - v1).normalized;
				Vector3 v3 = obstacle + v2 * 0.25f;
				//Debug.DrawLine(_ai.transform.position, v3, Color.green);
				if(Vector3.Distance(_ai.target.transform.position, closest_position) > Vector3.Distance(_ai.target.transform.position, v3)) closest_position = v3;
			}
			_ai.ApproachTarget(closest_position);
			Debug.DrawLine(closest_position + Vector3.right*0.5f, closest_position - Vector3.right*0.5f, Color.blue);
			Debug.DrawLine(closest_position + Vector3.forward*0.5f, closest_position - Vector3.forward*0.5f, Color.blue);
		}
	}
	
	public override void Exit(BaseAI _ai)
	{
		Debug.Log ("Exited Approach State");
	}
	
}
