using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AStar 
{
	//Has extra code for counting operations (to be moved or used)
	private Cell end;
	
	public AStar()
	{
		
	}
	
	public Path solve(List<Cell> _graph, Cell _start, Cell _end)
	{		
		end = _end;
		int ops = 0;
		List<Cell> open = new List<Cell>();
		List<Cell> closed = new List<Cell>();
		_start.g = 0;
		_start.f = Distance(_start, _end);
		_start.AStar_Parent = null;
		open.Add(_start);
		
		while(open.Count > 0) 
		{
			Cell current = GetLowest(open);
			
			if(current == _end) 
			{	
				//Debug.Log (ops);
				return GeneratePath(_end);
			}
			
			open.Remove(current);
			closed.Add(current);
			ops++;
			
			foreach(Cell adjacent in current.getNeighbors())
			{
				//Elimate cells not in the input graph (but may be linked to)
				if(!_graph.Contains(adjacent)) continue;
				
				float tentative_g = current.g + adjacent.cost;
				float tentative_f = tentative_g + Distance(adjacent, _end) * 2.0f;
				
				//if(closed.Contains(adjacent) && tentative_f >= adjacent.f) continue;
				if(closed.Contains(adjacent)) continue;
				if(!open.Contains(adjacent) || tentative_f < adjacent.f)
				{
					adjacent.AStar_Parent = current;
					adjacent.g = tentative_g;
					adjacent.f = tentative_f;
					if(!open.Contains(adjacent)) open.Add(adjacent);
						
				}
			}
		}
		return null;
	}
	
	//This generates the path backwards (needs fixing)
	private Path GeneratePath(Cell _end)
	{
		Cell current = _end;
		Path path = new Path();
		while(current.AStar_Parent != null)
		{
			path.addCell(current);
			current = current.AStar_Parent;
		}
		path.addCell(current);
		return path;
	}
	
	private float Distance(Cell _start, Cell _end)
	{
		float x = Mathf.Abs(_end.cell_GameObject.transform.position.x - _start.cell_GameObject.transform.position.x);
		float z = Mathf.Abs(_end.cell_GameObject.transform.position.z - _start.cell_GameObject.transform.position.z);
		return x + z;
		
	}
	
	private Cell GetLowest(List<Cell> _set)
	{
		float lowest_F = 655360.0f;
		Cell current = null;
		foreach(Cell cell in _set)
		{
			if(cell.f <= lowest_F)
			{
				lowest_F = cell.f;
				current = cell;
			}
		}
		return current;
	}
}
