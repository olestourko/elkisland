using UnityEngine;
using System.Collections;

public class BT_Node : Node {
	
	public BT_Blackboard blackboard = null;
	
	public enum ReturnType
	{
		Success,
		Fail,
		Running
	};
	
	public BT_Node() {}
	public BT_Node(BT_Blackboard _blackboard)
	{
		blackboard = _blackboard;	
	}
	
	public void AddChild(BT_Node _node)
	{
		adjacent_nodes.Add(_node);
		//Set the blackboard for this and children
		SetBlackboard(blackboard);
	}
	//Recursively sets the blackboard
	public void SetBlackboard(BT_Blackboard _blackboard)
	{
		blackboard = _blackboard;
		foreach(BT_Node node in adjacent_nodes) node.SetBlackboard(_blackboard);
	}
	
	//This should never be called in this class
	public virtual ReturnType Execute()
	{
		return ReturnType.Success;
	}
}
