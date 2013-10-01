using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Path {
	
	private List<Cell> cells = new List<Cell>();
	// Use this for initialization
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
	
	public int getLength()
	{
		return cells.Count;	
	}
}
