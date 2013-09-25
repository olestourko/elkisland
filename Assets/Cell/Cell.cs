using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cell : MonoBehaviour {
	
	public enum CellType
	{
		None,
		Path,
		Woods,
		Selected
	}
	
	public Cell next_left;
	public Cell next_top;
	public Cell next_right;
	public Cell next_bottom;
	public string name;
	
	/*--------------------------------------------------------------------------*/
	/*For Dijkstra shortest path solver (to be moved)							*/
	/*--------------------------------------------------------------------------*/
	public int cost = 1;
	
	public List<Cell> dijkstra_GetNeighbors()
	{
		List<Cell> cells = new List<Cell>();
		if(next_left != null) cells.Add(next_left);
		if(next_top != null) cells.Add(next_top);
		if(next_right != null) cells.Add(next_right);
		if(next_bottom != null) cells.Add(next_bottom);
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
	}
}
