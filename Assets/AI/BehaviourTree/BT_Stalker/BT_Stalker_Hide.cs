using UnityEngine;
using System.Collections;

public class BT_Stalker_Hide : BT_Node {

	public override ReturnType Execute ()
	{
		BT_Stalker_Blackboard blackboard_mine = blackboard as BT_Stalker_Blackboard;		
		BT_AI ai = blackboard_mine.ai;
		ai.Halt();
		return ReturnType.Success;
	}
}
