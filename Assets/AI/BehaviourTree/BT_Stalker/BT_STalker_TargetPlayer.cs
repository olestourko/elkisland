using UnityEngine;
using System.Collections;

public class BT_STalker_TargetPlayer : BT_Node {
	
	public override ReturnType Execute ()
	{
		BT_Stalker_Blackboard blackboard_mine = blackboard as BT_Stalker_Blackboard;
		BT_AI ai = blackboard_mine.ai;
		blackboard_mine.target = blackboard_mine.ai.target.transform.position;
		return ReturnType.Success;
	}	
}
