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
	
	//Model
	public GameObject model_straight;
	public GameObject model_turn;
	private GameObject model_instance;
	
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
		Destroy(model_instance);
		cellType = _type;
		Color c = Color.white;
		if(_type == CellType.Path) c = new Color(0.25f, 0.0f, 0.0f, 1.0f);
		else if(_type == CellType.Woods) c = Color.white;
		else if(_type == CellType.Path_Start) c = new Color(0.25f, 0.0f, 0.0f, 1.0f);
		else if(_type == CellType.Path_End) c = new Color(0.25f, 0.0f, 0.0f, 1.0f);
		
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
	
	public void DetectModelType()
	{
		Destroy(model_instance);
		List<CellType> valid_cell_types = new List<CellType>();
		valid_cell_types.Add(CellType.Path);
		valid_cell_types.Add(CellType.Path_Start);
		valid_cell_types.Add(CellType.Path_End);	
		if(!valid_cell_types.Contains(cellType)) return;
		
		int num_adjacent_paths = 0;
		if(left != null) { if(valid_cell_types.Contains(left.cellType)) num_adjacent_paths++; }
		if(right != null) { if(valid_cell_types.Contains(right.cellType)) num_adjacent_paths++; }
		if(top != null) { if(valid_cell_types.Contains(top.cellType)) num_adjacent_paths++; }
		if(bottom != null) { if(valid_cell_types.Contains(bottom.cellType)) num_adjacent_paths++; }
		if(num_adjacent_paths > 2) return;
		
		//Straights
		if(left != null && right != null)
		{
			if(valid_cell_types.Contains(left.cellType) && valid_cell_types.Contains(right.cellType))
			{
				InstantiateModel(model_straight);
				return;
			}
		}
		
		if(top != null && bottom != null)
		{
			if(valid_cell_types.Contains(top.cellType) && valid_cell_types.Contains(bottom.cellType))
			{
				InstantiateModel(model_straight);
				model_instance.transform.RotateAround(Vector3.up, Mathf.PI / 2.0f);
				return;
			}
		}
		//Turns
		if(top != null && left != null)
		{
			if(valid_cell_types.Contains(top.cellType) && valid_cell_types.Contains(left.cellType))
			{
				InstantiateModel(model_turn);
				//model_instance.transform.RotateAround(Vector3.up, Mathf.PI / 2.0f);
				return;
			}
		}
		if(bottom != null && right != null)
		{
			if(valid_cell_types.Contains(bottom.cellType) && valid_cell_types.Contains(right.cellType))
			{
				InstantiateModel(model_turn);
				model_instance.transform.RotateAround(Vector3.up, Mathf.PI);
				return;
			}
		}
		
		if(top != null && right != null)
		{
			if(valid_cell_types.Contains(top.cellType) && valid_cell_types.Contains(right.cellType))
			{
				InstantiateModel(model_turn);
				model_instance.transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
				return;
			}
		}
		if(bottom != null && left != null)
		{
			if(valid_cell_types.Contains(bottom.cellType) && valid_cell_types.Contains(left.cellType))
			{
				InstantiateModel(model_turn);
				model_instance.transform.RotateAround(Vector3.up, Mathf.PI);
				model_instance.transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
				return;
			}
		}
	}
	
	private void InstantiateModel(GameObject _model)
	{
		model_instance = Instantiate(_model) as GameObject;
		model_instance.transform.position = this.transform.position;
		model_instance.transform.parent = this.transform;
	}
}
