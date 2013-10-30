using UnityEngine;
using System.Collections;

public class BT_Sequence : BT_Node {

	public BT_Sequence() {}
	public BT_Sequence(BT_Blackboard _blackboard) : base(_blackboard) {}
	
	public override ReturnType Execute ()
	{
		foreach(BT_Node node in adjacent_nodes)
		{
			if(node.Execute() == BT_Node.ReturnType.Fail) return ReturnType.Fail;
		}
		return ReturnType.Success;
	}
	
}
