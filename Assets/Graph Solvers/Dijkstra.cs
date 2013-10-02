using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Dijkstra {

	private Dictionary<Cell, int> dist;
	private Dictionary<Cell, Cell> prev;
	
	public Dijkstra()
	{
		
	}
	
	public Path solve(List<Cell> _graph, Cell _start, Cell _end)
	{
		List<Cell> Q = new List<Cell>();
		foreach(Cell cell in _graph) Q.Add(cell);	
		dist = new Dictionary<Cell, int>();
		prev = new Dictionary<Cell, Cell>();
		
		foreach(Cell cell in Q)
		{
				dist.Add (cell, 65536);
				prev.Add (cell, null);
		}
		
		dist[_start] = 0;
		prev[_start] = _start;
		
		while(Q.Count > 0)
		{
			/*Find the cell with the shortest distance in Q (this will be the starting cell at first)*/
			Cell u = null;
			foreach(Cell cell in Q)
			{
				if(u == null) u = cell;
				else if(dist[u] > dist[cell]) u = cell;
			}
			Q.Remove(u);			
			foreach(Cell v in u.dijkstra_GetNeighbors())
			{
				if(Q.Contains(v)) 
				{
					int alt = dist[u] + v.cost;
					if(alt < dist[v]) 
					{
						dist[v] = alt;
						prev[v] = u;
					}
				}

			}
		}

		/*Generate path*/
		Cell current = prev[_end];
		Path path = new Path();
		while (current != _start)
		{
			path.addCell(current);
			current = prev[current];
		}
		return path;
	}
}
