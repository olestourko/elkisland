using UnityEngine;
using System.Collections;

public class BT_Stalker_PlayerInRange : BT_Node {
	
	public float min;
	public float max;
	
	public BT_Stalker_PlayerInRange(float _min, float _max)
	{
		min = _min;
		max = _max;
	}
	
	public override ReturnType Execute()
	{
		BT_Stalker_Blackboard blackboard_mine = blackboard as BT_Stalker_Blackboard;
		BT_AI ai = blackboard_mine.ai;
		float distance = Vector3.Distance(ai.transform.position, ai.target.position);
		//Debug.Log (distance);
		if(distance < min || distance > max) return ReturnType.Fail;
		return ReturnType.Success;
	}
}
