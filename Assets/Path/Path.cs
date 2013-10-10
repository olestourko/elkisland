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
	
	//Replace a section of the path with a subpath
	public bool Replace(Path _path)
	{
		Cell start = _path.getStart();
		Cell end = _path.getEnd();
		if(!(cells.Contains(start) && cells.Contains(end))) return false;
		int start_index = cells.IndexOf(start);
		int end_index = cells.IndexOf(end);
		
		List<Cell> first = new List<Cell>();
		List<Cell> middle = _path.getCells();
		List<Cell> last = new List<Cell>();
		//swap start and end if they are out of order
		if(start_index > end_index)
		{
			start_index = cells.IndexOf(end);
			end_index = cells.IndexOf(start);
			middle = new List<Cell>();
			for(int i = _path.getCells().Count-1; i >= 0; i--) middle.Add(_path.getCells()[i]);
		}
		//Debug.Log (start_index + " - " + end_index);
		
		for(int i = 0; i < start_index; i++) first.Add(cells[i]);
		for(int i = end_index+1; i < cells.Count; i++) last.Add(cells[i]);
		
		List<Cell> new_cells = new List<Cell>();
		foreach(Cell cell in first) new_cells.Add(cell);
		foreach(Cell cell in middle) new_cells.Add(cell);
		foreach(Cell cell in last) new_cells.Add(cell);
		cells = new_cells;
		return true;
	}
	
	public string ToString()
	{
		return "Path | start: " + this.getStart().cell_GameObject.name + ", end: " + this.getEnd().cell_GameObject.name;	
	}

}
