using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BT_Stalker_Follow : BT_Node {

	public override ReturnType Execute()
	{
		BT_Stalker_Blackboard blackboard_mine = blackboard as BT_Stalker_Blackboard;
		BT_AI ai = blackboard_mine.ai;
		
		//Get obstacles in range && !visible
		List<Vector3> obstacles_in_range = new List<Vector3>();
		foreach(Vector3 point in ai.obstacles)
		{
			float distance = Vector3.Distance(point, ai.target.position);
			if(distance >= blackboard_mine.min_range && distance <= blackboard_mine.max_range)
			{
				if(PlayerCanSeePoint(point)) continue;
				obstacles_in_range.Add(point);	
			}
		}
		
		//Get closest obstacle to player
		Vector3 closest = new Vector3(65536, 65536, 65536);
		foreach(Vector3 point in obstacles_in_range)
		{
			Auxilliary.DrawPoint(point, Color.red);
			if(Vector3.Distance(closest, ai.transform.position) > Vector3.Distance(point, ai.transform.position))
			{
				closest = point;
			}
		}
		ai.ApproachTarget(closest);
		
		//Solve a path
		SolverNode ai_position_node = new SolverNode(ai.transform.position);
		SolverNode target_position_node = new SolverNode(closest);
		ai_position_node.important = true;
		target_position_node.important = true;
		
		List<Node> solver_nodes = new List<Node>();
		solver_nodes.Add(ai_position_node);
		solver_nodes.Add(target_position_node);
		
		foreach(Vector3 position in obstacles_in_range)
		{
			solver_nodes.Add(new SolverNode(position));
		}
		foreach(Node node in solver_nodes)
		{
			(node as SolverNode).AddAdjacentNodes(solver_nodes);
		}
		
		
		AStar_NodeBased solver = new AStar_NodeBased();
		
		List<Node> solved = solver.solve(solver_nodes, ai_position_node, target_position_node);
		foreach(Node node in solved)
		{
			Auxilliary.DrawPoint((node as SolverNode).position, Color.yellow);
		}
		Auxilliary.DrawPoint(ai_position_node.position, Color.white);
		Auxilliary.DrawPoint(target_position_node.position, Color.white);		
		
		return ReturnType.Success;
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
