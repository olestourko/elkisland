using UnityEngine;
using System.Collections;

public class BT_Stalker_Approach : BT_Node {

	public override ReturnType Execute ()
	{
		BT_Stalker_Blackboard blackboard_mine = blackboard as BT_Stalker_Blackboard;
		BT_AI ai = blackboard_mine.ai;
		ai.ApproachTarget(blackboard_mine.target);
		Auxilliary.DrawPoint(blackboard_mine.target, Color.blue);
		return ReturnType.Success;
	}
}
