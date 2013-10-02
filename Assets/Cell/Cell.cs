using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cell : MonoBehaviour {
	
	public bool selected = false;
	public enum CellType
	{
		None,
		Path,
		Path_Start,
		Path_End,
		Woods,
		Selected
	}
	
	public Cell left;
	public Cell top;
	public Cell right;
	public Cell bottom;
	public string name;
	
	 //temporary for Astar
	public Cell AStar_Parent;
	public float g = 0;
	public float f = 65536;
	public float h = 0;
	
	/*--------------------------------------------------------------------------*/
	/*For Dijkstra shortest path solver (to be moved)							*/
	/*--------------------------------------------------------------------------*/
	public int cost = 1;
	
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
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		transform.GetChild(0).guiText.text = name;
	}
	
	public void setType(CellType _type)
	{
		if(_type == CellType.Path) renderer.material.color = Color.red;
		else if(_type == CellType.Woods) renderer.material.color = Color.grey;
		else if(_type == CellType.Selected) renderer.material.color = Color.magenta;
		else if(_type == CellType.Path_Start) renderer.material.color = Color.blue;
		else if(_type == CellType.Path_End) renderer.material.color = Color.yellow;
	}
}
