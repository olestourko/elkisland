using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BT_Stalker_GetObstaclesNotVisible : BT_Node {
	
	public enum Mode
	{
		New,
		Loaded
	}
	private Mode mode;
	
	public BT_Stalker_GetObstaclesNotVisible(Mode _mode) : base()
	{
		mode = _mode;
	}
	
	public override ReturnType Execute ()
	{
		BT_Stalker_Blackboard blackboard_mine = blackboard as BT_Stalker_Blackboard;
		switch(mode)
		{
		case Mode.New:
			blackboard_mine.obstacles = new List<Vector3>();
			foreach(Vector3 point in blackboard_mine.ai.obstacles)
			{
				if(!PlayerCanSeePoint(point))
				{
					Auxilliary.DrawPoint(point, Color.red);
					blackboard_mine.obstacles.Add(point);
				}
			}
			break;
			
		case Mode.Loaded:
			List<Vector3> new_obstacles = new List<Vector3>();
			foreach(Vector3 point in blackboard_mine.obstacles)
			{
				if(!PlayerCanSeePoint(point))
				{
					Auxilliary.DrawPoint(point, Color.red);
					new_obstacles.Add(point);
				}
			}
			blackboard_mine.obstacles = new_obstacles;
			break;
		}
		return ReturnType.Success;
	}
	
	//To be moved to the player class
	private bool PlayerCanSeePoint(Vector3 _point)
	{
		BT_Stalker_Blackboard blackboard_mine = blackboard as BT_Stalker_Blackboard;
		float x = Camera.main.WorldToViewportPoint(_point).x;
		Vector3 my_local_position = blackboard_mine.ai.target.InverseTransformPoint(_point);
		if(x > 0.0f && x < 1.0f) 
		{
			if(my_local_position.z >= 0.0f) return true;
		}
		return false;	
	}
}
