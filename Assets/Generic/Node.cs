using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Node {
	
	//For A* solver
	public Node AStar_Parent;
	public float g = 0;
	public float f = 65536;
	public float h = 0;
	public int cost = 1;
	
	
	public List<Node> adjacent_nodes = new List<Node>();
	public Node() {}
	public virtual float GetCost(Node _to)
	{
		return 0.0f;
	}
	//Sets the hueristic distance to target
	public virtual void SetF(Node _end)
	{
		f = 1;	
	}
	
	public List<Node> GetAdjacent()
	{
		return adjacent_nodes;	
	}
}
