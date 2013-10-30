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
	public virtual int GetCost()
	{
		return cost;
	}
	public virtual void SetF(Node _start, Node _end)
	{
		f = 1;	
	}
}
