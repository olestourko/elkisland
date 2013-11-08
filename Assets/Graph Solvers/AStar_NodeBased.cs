using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AStar_NodeBased 
{
	public List<Node> solve(List<Node> _graph, Node _start, Node _end)
	{
		List<Node> open = new List<Node>();
		List<Node> closed = new List<Node>();
		_start.g = 0;
		_start.SetF(_end);
		_start.AStar_Parent = null;
		open.Add(_start);
		
		//Set F for each node
		foreach(Node node in _graph)
		{
			SolverNode sn = node as SolverNode;
			sn.SetF(_end);
			//Auxilliary.DrawPoint(sn.position, new Color(0, (sn.f / 100.0f) * 4.0f, 0));
		}
		
		while(open.Count > 0) 
		{
			Node current = GetLowest(open);
			if(current == _end) 
			{	
				return GeneratePath(_end);
			}
			
			open.Remove(current);
			closed.Add(current);
			foreach(Node adjacent in current.adjacent_nodes)
			{
				//Elimate cells not in the input graph (but may be linked to)
				if(!_graph.Contains(adjacent)) continue;
				
				float tentative_g = current.g + adjacent.GetCost(current);
				float tentative_f = tentative_g + adjacent.f;
				
				//if(closed.Contains(adjacent) && tentative_f >= adjacent.f) continue;
				if(closed.Contains(adjacent)) continue;
				if(!open.Contains(adjacent) || tentative_f < adjacent.f)
				{
					adjacent.AStar_Parent = current;
					adjacent.g = tentative_g;
					adjacent.f = tentative_f;
					if(!open.Contains(adjacent)) 
					{
						open.Add(adjacent);
					}
						
				}
			}
		}
		return null;
	}
	
	//This generates the path backwards (needs fixing)
	private List<Node> GeneratePath(Node _end)
	{
		Node current = _end;
		List<Node> path = new List<Node>();
		while(current.AStar_Parent != null)
		{
			path.Add(current);
			current = current.AStar_Parent;
		}
		path.Add(current);
		path.Reverse();
		return path;
	}
		
	private Node GetLowest(List<Node> _set)
	{
		float lowest_F = 655360.0f;
		Node current = null;
		foreach(Node node in _set)
		{
			if(node.f <= lowest_F)
			{
				lowest_F = node.f;
				current = node;
			}
		}
		return current;
	}
}
