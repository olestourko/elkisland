using UnityEngine;
using System.Collections;

public class BT_GetValue : BT_Node {
	
	private string key;
	public BT_GetValue(string _key)
	{
		key = _key;
	}
	
	public override ReturnType Execute ()
	{
		if(!blackboard.values.ContainsKey(key)) return ReturnType.Fail;
		if(blackboard.values[key] == 0) return ReturnType.Success;
		return ReturnType.Fail;
	}
}
