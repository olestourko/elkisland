using UnityEngine;
using System.Collections;

public class BT_Selector : BT_Node {

	public BT_Selector() {}
	public BT_Selector(BT_Blackboard _blackboard) : base(_blackboard) {}
	
	public override ReturnType Execute ()
	{
		foreach(BT_Node node in adjacent_nodes)
		{
			if(node.Execute() == BT_Node.ReturnType.Success) return ReturnType.Success;
		}
		return ReturnType.Fail;
	}
	
}
