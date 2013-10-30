using UnityEngine;
using System.Collections;

public class BT_Test_B : BT_Node {

	public override ReturnType Execute()
	{
		BT_Test_Blackboard blackboard_mine = blackboard as BT_Test_Blackboard;
		blackboard_mine.data += "B";
		Debug.Log ("Executed B");
		return ReturnType.Success;
	}
}
