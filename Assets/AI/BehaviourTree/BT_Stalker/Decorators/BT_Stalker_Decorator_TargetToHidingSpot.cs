using UnityEngine;
using System.Collections;

public class BT_Stalker_Decorator_TargetToHidingSpot : BT_Node {
	
	public override ReturnType Execute()
	{
		BT_Stalker_Blackboard blackboard_mine = blackboard as BT_Stalker_Blackboard;
		blackboard_mine.hidding_location = blackboard_mine.target;
		return (adjacent_nodes[0] as BT_Node).Execute();
	}
	
}
