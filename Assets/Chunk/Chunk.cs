using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Chunk : MonoBehaviour {
	
	public bool isActive = true;
	public bool selected = false;
	public bool regened = false; //temporary (used for camera redrawing)
	
	public Cell cellPrefab;
	private GUIText label;
	
	Cell[,] cells = new Cell[5,5];
	List<Cell> cells_list = new List<Cell>();
	public Chunk left;
	public Chunk top;
	public Chunk right;
	public Chunk bottom;
	private List<Path> paths = new List<Path>();
	
	/*Event firing when chunk is init'd*/
	public event InitHandler Init;
	public delegate void InitHandler(Chunk _chunk);
	
	
	// Use this for initialization
	void Start () {
		label = this.transform.GetChild(0).gameObject.guiText;
		generate();
		//link();		//Call moved to WorldGrid (since other chunks may not have loaded
		Init(this);
	}
	
	// Update is called once per frame
	void Update ()
	{	
		label.text = name;
		/*
		bool visible = false;
		Vector3 screen_position = Camera.main.WorldToViewportPoint(transform.position);
		if(screen_position.x < 0.0f || screen_position.x > 1.0f || screen_position.y < 0.0f || screen_position.y > 1.0f) visible = true;
		if(!visible)
		{
			Color c = new Color(0.0f, 1.0f, 0.0f, 1.0f);
			Debug.DrawLine(transform.position + new Vector3(0f, 0f, 0f), transform.position + new Vector3(0f, 0f, 4f), c);
			Debug.DrawLine(transform.position + new Vector3(4f, 0f, 0f), transform.position + new Vector3(4f, 0f, 4f), c);
			Debug.DrawLine(transform.position + new Vector3(0f, 0f, 4f), transform.position + new Vector3(4f, 0f, 4f), c);
			Debug.DrawLine(transform.position + new Vector3(4f, 0f, 0f), transform.position + new Vector3(0f, 0f, 0f), c);
		}
		*/
	}
	
	public void Select()
	{
		selected = true;
		foreach(Cell cell in getCells())
		{
			cell.Select();	
		}
	}
	
	public void Deselect()
	{
		selected = false;
		foreach(Cell cell in getCells())
		{
			cell.Deselect();
		}
	}
	
	public void generate()
	{
		/*Create the cells*/
		for(int i = 0; i < 5; i++)
		{
			for(int j = 0; j < 5; j++)
			{
				Cell cell = Instantiate(cellPrefab) as Cell;
				cells[i, j] = cell;
				cell.transform.parent = this.transform;
				cell.transform.position = transform.position + new Vector3(i, 0, j);
				cell.cost = Random.Range(1, 4);
				//cell.name = (i*5 + j).ToString();
				cell.name = cell.cost + "";
				float c = (float)cell.cost/3;
				cell.renderer.material.color = new Color(c, c, c);
				cells_list.Add(cell);
			}			
		}
		
		/*Link the cells*/
		for(int i = 0; i < 5; i++)
		{
			for(int j = 0; j < 5; j++)
			{
				if(i-1 >= 0) cells[i, j].left = cells[i-1, j];
				if(i+1 < 5) cells[i, j].right = cells[i+1, j];
				if(j-1 >= 0) cells[i, j].bottom = cells[i, j-1];
				if(j+1 < 5) cells[i, j].top = cells[i, j+1];
			}
		}
	}
	
	//To be merged with generate()
	public void regenerate()
	{
		regened = true;
		paths = new List<Path>();
		for(int i = 0; i < 5; i++)
		{
			for(int j = 0; j < 5; j++)
			{
				Cell cell = cells[i, j];
				cell.cost = Random.Range(1, 4);
				cell.name = cell.cost + "";
				cell.setType(Cell.CellType.Woods);
			}			
		}
	}
	
	public void ClearLocalPaths()
	{
		paths = new List<Path>();	
	}
	
	public void ApplyPath(Path _path)
	{
		//Debug.Log ("Chunk " + name + " applied path");
		//paths = new List<Path>();
		bool onPath = false;
		Path localPath = null;
		foreach(Cell cell in _path.getCells())
		{
			//Is this the first cell within the chunk for this path?
			//Is the next cell in the path outside of the chunk (or non-existent?)
			if(this.hasCell(cell))
			{
				if(!onPath)
				{
					cell.setType(Cell.CellType.Path_Start);
					localPath = new Path();
					localPath.partOf = _path;
					onPath = true;
				}
				else if(!this.hasCell(_path.getNext(cell)))
				{
					cell.setType(Cell.CellType.Path_End);
					onPath = false;
					//Debug.Log ("A path runs through chunk " + name);	
				}
				else
				{
					cell.setType(Cell.CellType.Path);
				}
				localPath.addCell(cell);
				//Debug.Log("Chunk " + name + " added local path");
			}		
		}
		if(localPath != null)
		{
			if(localPath.getLength() >= 2) 
			{
				//Debug.Log ("Chunk " + name + " added local path");
				paths.Add(localPath);
				//Debug.Log (localPath.partOf.getLength());
			}
		}
		
		GetWorldPaths();
	}
	
	public List<Path> GetLocalPaths()
	{
		return paths;
	}
	
	public List<Path> GetWorldPaths()
	{
		List<Path> worldPaths = new List<Path>();
		foreach(Path path in paths)
		{
			if(!worldPaths.Contains(path.partOf)) 
			{
				//Debug.Log (path.partOf.getLength());
				worldPaths.Add(path.partOf);
			}
		}
		//Debug.Log ("Chunk " + name + " worldPaths: " + worldPaths.Count);
		return worldPaths;
	}
	
	public void ApplyModels()
	{
		foreach(Cell cell in cells_list)
		{
			cell.DetectModelType();	
		}
	}
	
	//--------------------------------------------------------------------------
	// Get various sets of cells
	//--------------------------------------------------------------------------
	public List<Cell> getCells()
	{
		return cells_list;
	}
	public List<Cell> GetCells_Border()
	{
		List<Cell> cells_out = new List<Cell>();
		for(int i = 0; i < 5; i++) cells_out.Add(cells[0, i]);
		for(int i = 0; i < 5; i++) cells_out.Add(cells[4, i]);
		for(int i = 1; i < 4; i++) cells_out.Add(cells[i, 0]);
		for(int i = 1; i < 4; i++) cells_out.Add(cells[i, 4]);
		return cells_out;
	}
	//--------------------------------------------------------------------------
	
	public bool hasCell(Cell _cell)
	{
		if(_cell == null) return false;
		return cells_list.Contains(_cell);
	}
	
	public void makeActive()
	{
		gameObject.SetActive(true);
	}
	public void makeInactive()
	{
		gameObject.SetActive(false);
	}
	
	/*Link cells between chunks*/
	public void LinkCells()
	{
		if(left != null)
		{
			for(int i = 0; i <5; i++) 
			{
				cells[0, i].left = left.cells[4, i];
				left.cells[4, i].right = cells[0, i];
			}
		}
		if(right != null)
		{
			for(int i = 0; i <5; i++) 
			{
				cells[4, i].right = right.cells[0, i];
				right.cells[0, i].left = cells[4, i];
			}
		}
		if(top != null)
		{
			for(int i = 0; i <5; i++) 
			{
				cells[i, 4].top = top.cells[i, 0];
				top.cells[i, 0].bottom = cells[i, 4];
			}
		}
		if(bottom != null)
		{
			for(int i = 0; i <5; i++) 
			{
				cells[i, 0].bottom = bottom.cells[i, 4];
				bottom.cells[i, 4].top = cells[i, 0];
			}
		}
	}
	
	//Experimental
	/*
	public void Destroy()
	{
		List<Cell> cells_list = new List<Cell>();
		foreach(Cell cell in getCells())
		{
			cells_list.Add(cell);	
		}
		while(cells_list.Count > 0)
		{
			Cell cell = cells_list[0];
			cells_list.Remove(cell);
			GameObject.Destroy(cell);
		}
	}
	*/
}
