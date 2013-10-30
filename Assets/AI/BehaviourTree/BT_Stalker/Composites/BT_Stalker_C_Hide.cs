using UnityEngine;
using System.Collections;

public class BT_Stalker_C_Hide : BT_Node {

	public enum Mode
	{
		Closest,
		NotVisible,
		Target
	};
	private Mode mode;
	
	public BT_Stalker_C_Hide(Mode _mode)
	{
		mode = _mode;
		//BT_Sequence subtree = new BT_Sequence();
	}
	
	public override ReturnType Execute()
	{
		switch(mode)
		{
		case Mode.Closest:
			DoClosest();
			break;
		case Mode.NotVisible:
			DoNotVisible();
			break;
		case Mode.Target:
			DoTarget();
			break;
		}
		return ReturnType.Success;
	}
	
	private void DoClosest()
	{		
		BT_Stalker_Blackboard blackboard_mine = blackboard as BT_Stalker_Blackboard;
		BT_AI ai = blackboard_mine.ai;
		Vector3 closest = new Vector3(65536, 65536, 65536);
		foreach(Vector3 point in blackboard_mine.obstacles)
		{
			if(Vector3.Distance(closest, ai.transform.position) > Vector3.Distance(point, ai.transform.position)) closest = point;
		}
		
		blackboard_mine.hidding_location = closest;
		blackboard_mine.target = closest;
		
		//Offset the position relative to ai's target
		Vector3 v1 = new Vector3(ai.target.position.x, 0, ai.target.position.z);
		Vector3 v2 = (closest - v1).normalized;
		closest += (v2 * 0.25f);
		ai.ApproachTarget(closest);
		Auxilliary.DrawPoint(closest, Color.magenta);
	}
	
	private void DoNotVisible()
	{
		BT_Stalker_Blackboard blackboard_mine = blackboard as BT_Stalker_Blackboard;
		BT_AI ai = blackboard_mine.ai;
		Vector3 closest = new Vector3(65536, 65536, 65536);
		foreach(Vector3 point in blackboard_mine.obstacles)
		{
			if(PlayerCanSeePoint(point)) continue;
			if(Vector3.Distance(closest, ai.transform.position) > Vector3.Distance(point, ai.transform.position)) closest = point;
		}
		
		blackboard_mine.hidding_location = closest;
		blackboard_mine.target = closest;
		
		//Offset the position relative to ai's target
		Vector3 v1 = new Vector3(ai.target.position.x, 0, ai.target.position.z);
		Vector3 v2 = (closest - v1).normalized;
		closest += (v2 * 0.25f);
		ai.ApproachTarget(closest);
		Auxilliary.DrawPoint(closest, Color.cyan);
	}
	
	private void DoTarget()
	{
		BT_Stalker_Blackboard blackboard_mine = blackboard as BT_Stalker_Blackboard;
		BT_AI ai = blackboard_mine.ai;
		//Offset the position relative to ai's target
		Vector3 v1 = new Vector3(ai.target.position.x, 0, ai.target.position.z);
		Vector3 v2 = (blackboard_mine.target - v1).normalized;
		Vector3 point = blackboard_mine.target + (v2 * 0.25f);
		ai.ApproachTarget(point);
		Auxilliary.DrawPoint(point, Color.magenta);
	}
	
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
