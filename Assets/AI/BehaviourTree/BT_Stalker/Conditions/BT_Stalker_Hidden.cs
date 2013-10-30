using UnityEngine;
using System.Collections;

public class BT_Stalker_Hidden : BT_Node {

	public override ReturnType Execute()
	{
		BT_Stalker_Blackboard blackboard_mine = blackboard as BT_Stalker_Blackboard;
		float distance = Vector3.Distance(blackboard_mine.hidding_location, blackboard_mine.ai.transform.position);
		if(distance <= 1.5f) 
		{	
			return ReturnType.Success;
		}
		return ReturnType.Fail;
	}
	
}
