using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BT_Stalker_Blackboard : BT_Blackboard {
	public BT_AI ai;
	public List<Vector3> obstacles;
		
	public float min_range;
	public float max_range;
	
	public Vector3 target;
	
	public Vector3 hidding_location;
}
