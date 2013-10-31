using UnityEngine;
using System.Collections;

public class BT_SetValue : BT_Node {
	
	private string key;
	private int v;
	public BT_SetValue(string _key, int _value)
	{
		key = _key;
		v = _value;
	}
	
	public override ReturnType Execute ()
	{
		if(blackboard.values.ContainsKey(key)) blackboard.values[key] = v;
		else blackboard.values.Add(key, v);
		return ReturnType.Success;
	}
}
