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
			float min_range = 3.0f;
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
				float d_to_current = Vector3.Distance(_ai.target.transform.position, v3);
				
				//Consider this position if its the closest, is out of min range, and is behind the target
				if(Vector3.Distance(_ai.target.transform.position, closest_position) > d_to_current
					&& d_to_current >= min_range
					&& IsBehind(_ai.target, v3)) closest_position = v3;
				
				//highlight positions that are too close
				if(!(d_to_current >= min_range))
				{
					Debug.DrawLine(obstacle + Vector3.right*0.5f, obstacle - Vector3.right*0.5f, Color.red);
					Debug.DrawLine(obstacle + Vector3.forward*0.5f, obstacle - Vector3.forward*0.5f, Color.red);	
				}
				if(!IsBehind(_ai.target, v3))
				{
					Debug.DrawLine(obstacle + Vector3.right*0.5f, obstacle - Vector3.right*0.5f, Color.cyan);
					Debug.DrawLine(obstacle + Vector3.forward*0.5f, obstacle - Vector3.forward*0.5f, Color.cyan);					
				}
				
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
	
	//Used to check if the position is behind the ai target
	private bool IsBehind(Transform _target, Vector3 _position)
	{
		Vector3 local_position = _target.InverseTransformPoint(_position);
		if(local_position.z > 0.0f) return false;
		return true;
	}
}
