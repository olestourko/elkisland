using UnityEngine;
using System.Collections;

public class BT_Timeout : BT_Node {
	
	private float end_time;
	private float time;
	
	public BT_Timeout(float _time) : base()
	{
		time = _time;	
	}
	
	//Use: On success, the timeout is started.
	public override ReturnType Execute()
	{
		if(blackboard.elapsed_time >= end_time) 
		{
			end_time = blackboard.elapsed_time + time;
			return ReturnType.Success;
		}
		return ReturnType.Fail;
		
	}
}
