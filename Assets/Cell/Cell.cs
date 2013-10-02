using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cell : MonoBehaviour {
	
	public bool selected = false;
	private Color originalColor = Color.white;
	
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
		cellType = _type;
		Color c = Color.white;
		if(_type == CellType.Path) c = Color.red;
		else if(_type == CellType.Woods) c = Color.white;
		else if(_type == CellType.Path_Start) c = Color.blue;
		else if(_type == CellType.Path_End) c = Color.yellow;
		
		if(cellType == CellType.Woods)
		{
			float w = (float)cost/3;
			c.r *= w;
			c.g *= w;
			c.b *= w;
		}
		
		originalColor = c;
		if(selected)
		{
			c.r *= 0.5f;
			c.g *= 0.5f;
			c.b *= 0.5f;
			c.g += 0.25f;
		}
		renderer.material.color = c;
	}
	
	public void Select()
	{
		if(selected) return;
		selected = true;
		originalColor = renderer.material.color;
		Color c = renderer.material.color;
		c.r *= 0.5f;
		c.g *= 0.5f;
		c.b *= 0.5f;
		c.g += 0.25f;
		renderer.material.color = c;
	}
	
	public void Deselect()
	{
		selected = false;
		renderer.material.color = originalColor;	
	}
}
