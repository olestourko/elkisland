using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Chunk : MonoBehaviour {
	
	public bool isActive = true;
	
	public Cell cellPrefab;
	private GUIText label;
	
	Cell[,] cells = new Cell[5,5];
	public Chunk left;
	public Chunk top;
	public Chunk right;
	public Chunk bottom;
	
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
	void Update () {
		label.text = name;
		Color c = new Color(0.0f, 0.0f, 0.0f, 1.0f);
		if(isActive) c = new Color(0.0f, 0.0f, 0.0f, 1.0f);
		
		Debug.DrawLine(transform.position + new Vector3(0f, 0f, 0f), transform.position + new Vector3(0f, 0f, 4f), c);
		Debug.DrawLine(transform.position + new Vector3(4f, 0f, 0f), transform.position + new Vector3(4f, 0f, 4f), c);
		Debug.DrawLine(transform.position + new Vector3(0f, 0f, 4f), transform.position + new Vector3(4f, 0f, 4f), c);
		Debug.DrawLine(transform.position + new Vector3(4f, 0f, 0f), transform.position + new Vector3(0f, 0f, 0f), c);
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
				cell.cost = Random.Range(1, 3);
				//cell.cost = 1;
				cell.name = cell.cost + "";
				float c = (float)cell.cost/3;
				cell.renderer.material.color = new Color(c, c, c);
				
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
		for(int i = 0; i < 5; i++)
		{
			for(int j = 0; j < 5; j++)
			{
				Cell cell = cells[i, j];
				cell.cost = Random.Range(1, 4);
				cell.name = cell.cost + "";
				float c = (float)cell.cost/3;
				cell.renderer.material.color = new Color(c, c, c);
				
			}			
		}
	}
	
	public void applyPath(Path _path)
	{
		bool onPath = false;
		Path localPath = new Path();
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
					onPath = true;
				}
				if(!this.hasCell(_path.getNext(cell)))
				{
					cell.setType(Cell.CellType.Path_End);
					onPath = false;
					//Debug.Log ("A path runs through chunk " + name);	
				}
				localPath.addCell(cell);
			}
				
		}
	}
	
	public List<Cell> getCells()
	{
		List<Cell> cells_out = new List<Cell>();
		for(int i = 0; i < 5; i++)
		{
			for(int j = 0; j < 5; j++)
			{
				cells_out.Add (cells[i,j]);
			}			
		}
		return cells_out;
	}
	
	public bool hasCell(Cell _cell)
	{
		if(_cell == null) return false;
		for(int i = 0; i < 5; i++)
		{
			for(int j = 0; j < 5; j++)
			{
				if(cells[i, j] == _cell) return true;
			}			
		}
		return false;
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
	public void link()
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
}
