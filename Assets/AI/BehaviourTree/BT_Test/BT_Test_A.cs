using UnityEngine;
using System.Collections;

public class BT_Test_A : BT_Node {

	public override ReturnType Execute()
	{
		BT_Test_Blackboard blackboard_mine = blackboard as BT_Test_Blackboard;
		blackboard_mine.data += "A";
		Debug.Log ("Executed A");
		return ReturnType.Fail;
	}
}
