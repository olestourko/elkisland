using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SolverNode : Node {
	
	public Vector3 position;
	public bool important = false;
	
	public SolverNode(Vector3 _position) : base() 
	{
		position = _position;
	}
	
	public void AddAdjacentNodes(List<Node> _nodes)
	{
		foreach(SolverNode node in _nodes)
		{
			if(!this.adjacent_nodes.Contains(node))	
			{
				SolverNode sn = node as SolverNode;
				if(Vector3.Distance(sn.position, this.position) >= 2.0f && !sn.important) continue;
				this.adjacent_nodes.Add(node);
			}
		}
	}
	
	public override void SetF(Node _end)
	{
		SolverNode end = _end as SolverNode;
		float x = Mathf.Abs(end.position.x - position.x);
		float z = Mathf.Abs(end.position.z - position.z);
		this.f =  x + z;
	}
	
	public override float GetCost(Node _to)
	{
		SolverNode to = _to as SolverNode;
		float cost = Vector3.Distance(this.position, to.position);
		return cost;
	}
}
