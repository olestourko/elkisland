using UnityEngine;
using System.Collections;

public class BT_Stalker_CanSeeMe : BT_Node {

	public override ReturnType Execute()
	{
		BT_Stalker_Blackboard blackboard_mine = blackboard as BT_Stalker_Blackboard;
		if(blackboard_mine.ai.TargetCanSeeMe()) return ReturnType.Success;
		return ReturnType.Fail;
	}
}
