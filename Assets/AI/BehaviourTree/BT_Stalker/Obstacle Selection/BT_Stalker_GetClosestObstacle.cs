using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BT_Stalker_GetClosestObstacle : BT_Node {
	
	public enum Mode
	{
		New,
		Loaded
	}
	private Mode mode;
	
	public BT_Stalker_GetClosestObstacle(Mode _mode) : base()
	{
		mode = _mode;
	}
	
	public override ReturnType Execute ()
	{
		BT_Stalker_Blackboard blackboard_mine = blackboard as BT_Stalker_Blackboard;
		BT_AI ai = blackboard_mine.ai;

		switch(mode)
		{
		case Mode.New:
			//Needs implementation
			break;
			
		case Mode.Loaded:
			Vector3 closest = new Vector3(65536, 65536, 65536);
			foreach(Vector3 point in blackboard_mine.obstacles)
			{
				if(Vector3.Distance(closest, ai.transform.position) > Vector3.Distance(point, ai.transform.position))
				{
					closest = point;
				}
			}
			blackboard_mine.obstacles = new List<Vector3>();
			blackboard_mine.obstacles.Add(closest);
			blackboard_mine.target = closest;
			break;
		}
		return ReturnType.Success;
	}	
}
