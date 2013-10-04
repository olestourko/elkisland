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
}
