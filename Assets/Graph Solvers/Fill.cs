using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Fill {

	public List<Node> Solve(Node _node)
	{
		List<Node> open = new List<Node>();
		List<Node> closed = new List<Node>();
		open.Add(_node);

		while(open.Count > 0)
		{
			for(int i = 0; i < open.Count; i++)
			{
				Node node = open[i];
				open.Remove(node);	
				closed.Add(node);
				foreach(Node adjacent_node in node.GetAdjacent())
				{
					if(!open.Contains(adjacent_node) && !closed.Contains(adjacent_node))
						open.Add(adjacent_node);
				}
			}
		}
		
		return closed;
		
	}
}
