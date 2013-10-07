using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Path {
	
	private List<Cell> cells = new List<Cell>();
	public Path partOf;

	// for animation
	private int c = 0;
	public Path()
	{
		
	}
	
	public void addCell(Cell _cell)
	{
		cells.Add(_cell);	
	}
	
	public Cell getStart()
	{
		return cells[0];	
	}
	
	public List<Cell> getCells()
	{
		return cells;	
	}
	
	public Cell getEnd()
	{
		return cells[cells.Count - 1];
	}
	
	public Cell getNext(Cell _cell)
	{
		if(cells.Contains(_cell))
		{
			int i = cells.IndexOf(_cell);
			if(i+1 < cells.Count)
			{
				return cells[i+1];	
			}
		}
		return null;
	}
	
	public Cell getRandomCell()
	{
		return cells[Random.Range(0, cells.Count-1)];	
	}
	
	public int getLength()
	{
		return cells.Count;	
	}
	
	//Path animation
	public void Animate()
	{
		cells[c].Deselect();
		c++;
		if(c > cells.Count-1) c = 0;
		cells[c].Select();
	}
	
	//Path can be appended at the head or a tail
	public bool Append(Path _path)
	{
		//New path is at tail
		if(this.getEnd().IsAdjacent(_path.getStart())) 
		{
			Debug.Log ("Path appended (tail | head)");
			foreach(Cell cell in _path.getCells()) cells.Add(cell);
			return true;
		}
		if(this.getEnd().IsAdjacent(_path.getEnd())) 
		{
			Debug.Log ("Path appended (tail | tail)");
			for(int i = _path.getLength() - 1; i >= 0; i--)
			{
				cells.Add(_path.getCells()[i]);
			}
			return true;
		}		
		
		//New path is at head
		if(this.getStart().IsAdjacent(_path.getEnd())) 
		{
			Debug.Log ("Path appended (head | tail)");
			List<Cell> new_cells = new List<Cell>();
			foreach(Cell cell in _path.getCells()) new_cells.Add(cell);
			foreach(Cell cell in cells) new_cells.Add(cell);
			cells = new_cells;
			return true;
		}
		return false;
	}
	//Insert a path between the start and end of this one
	public bool Insert(Path _path)
	{
		List<Cell> new_cells = new List<Cell>();
		Cell start = _path.getStart();
		Cell end = _path.getEnd();
		Cell current = null;
		return false;
	}
}
