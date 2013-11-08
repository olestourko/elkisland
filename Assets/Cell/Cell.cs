using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cell : Node {

	public bool selected = false;
	public CellType cellType;
	public enum CellType
	{
		None,
		Path,
		Path_Start,
		Path_End,
		Woods
	}
	
	public Cell left;
	public Cell top;
	public Cell right;
	public Cell bottom;
	public string name;
	
	 //temporary for Astar
	public Cell AStar_Parent;

	//for threading
	public Vector3 position;
	
	//Move later (models)
	public GameObject model_instance;
	public GameObject model_random;
	
	//Alternative weight used for height
	public float height;
	
	public Cell() {}
	
	public List<Cell> getNeighbors()
	{
		List<Cell> cells = new List<Cell>();
		if(left != null) cells.Add(left);
		if(top != null) cells.Add(top);
		if(right != null) cells.Add(right);
		if(bottom != null) cells.Add(bottom);
		return cells;
	}
	/*--------------------------------------------------------------------------*/
	
	public bool IsAdjacent(Cell _cell)
	{
		if(_cell == left || _cell == right || _cell == top || _cell == bottom) return true;
		return false;
	}
	
	public void setType(CellType _type)
	{
		cellType = _type;
		if(_type == CellType.Path) 
		{
			if(top != null) top.height = 0.0f;
			if(bottom != null) bottom.height = 0.0f;
			if(left != null) left.height = 0.0f;
			if(right != null) right.height = 0.0f;
			height = 0.0f;
		}
	}
	
	public void Select()
	{
		if(selected) return;
		selected = true;
	}
	
	public void Deselect()
	{
		selected = false;
	}
	
	public int CostMinusOne() { return cost - 1; }
}
