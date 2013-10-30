using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BT_Stalker_GetObstaclesInRange : BT_Node {
	
	public enum Mode
	{
		New,
		Loaded
	}
	private Mode mode;
	
	public BT_Stalker_GetObstaclesInRange(Mode _mode) : base()
	{
		mode = _mode;
	}
	
	public override ReturnType Execute ()
	{
		BT_Stalker_Blackboard blackboard_mine = blackboard as BT_Stalker_Blackboard;
		switch(mode)
		{
		case Mode.New:
			//Debug.Log ("Operating on new obstaces");
			blackboard_mine.obstacles = new List<Vector3>();
			foreach(Vector3 point in blackboard_mine.ai.obstacles)
			{
				float distance = Vector3.Distance(blackboard_mine.ai.target.transform.position, point);
				if(distance >= blackboard_mine.min_range && distance <= blackboard_mine.max_range)
				{
					Auxilliary.DrawPoint(point, Color.green);
					blackboard_mine.obstacles.Add(point);	
				}
			}
			break;
		case Mode.Loaded:
			List<Vector3> new_obstacles = new List<Vector3>();
			foreach(Vector3 point in blackboard_mine.obstacles)
			{
				float distance = Vector3.Distance(blackboard_mine.ai.target.transform.position, point);
				if(distance >= blackboard_mine.min_range && distance <= blackboard_mine.max_range)
				{
					Auxilliary.DrawPoint(point, Color.green);
					new_obstacles.Add(point);
				}
			}
			blackboard_mine.obstacles = new_obstacles;
			//Debug.Log ("Operating on loaded obstaces");
			break;
		}
		return ReturnType.Success;
	}
}
