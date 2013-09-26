using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Chunk : MonoBehaviour {
	
	public Cell cellPrefab;
	private GUIText label;
	
	Cell[,] cells = new Cell[5,5];
	
	public Chunk left;
	public Chunk top;
	public Chunk right;
	public Chunk bottom;
	
	public enum Side
	{
		Left,
		Top,
		Right,
		Bottom
	}
	
	// Use this for initialization
	void Start () {
		label = this.transform.GetChild(0).gameObject.guiText;
		generate();
		link();
	}
	
	// Update is called once per frame
	void Update () {
		label.text = name;
		Debug.DrawLine(transform.position + new Vector3(0f, 0f, 0f), transform.position + new Vector3(0f, 0f, 4f), Color.red);
		Debug.DrawLine(transform.position + new Vector3(4f, 0f, 0f), transform.position + new Vector3(4f, 0f, 4f), Color.red);
		Debug.DrawLine(transform.position + new Vector3(0f, 0f, 4f), transform.position + new Vector3(4f, 0f, 4f), Color.red);
		Debug.DrawLine(transform.position + new Vector3(4f, 0f, 0f), transform.position + new Vector3(0f, 0f, 0f), Color.red);
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
				cell.name = i + ", " + j + "";
				cell.transform.parent = this.transform;
				cell.transform.position = transform.position + new Vector3(i, 0, j);
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
	
	/*Link cells between chunks*/
	public void link()
	{
		if(left != null)
		{
			Debug.Log ("Linking left");
			for(int i = 0; i <5; i++) 
			{
				cells[0, i].left = left.cells[4, i];
				left.cells[4, i].right = cells[0, i];
			}
		}
		if(right != null)
		{
			Debug.Log ("Linking right");
			for(int i = 0; i <5; i++) 
			{
				cells[4, i].right = right.cells[0, i];
				right.cells[0, i].left = cells[4, i];
			}
		}
		if(top != null)
		{
			Debug.Log ("Linking top");
			for(int i = 0; i <5; i++) 
			{
				cells[i, 4].top = top.cells[i, 0];
				top.cells[i, 0].bottom = cells[i, 4];
			}
		}
		if(bottom != null)
		{
			Debug.Log ("Linking bottom");
			for(int i = 0; i <5; i++) 
			{
				cells[i, 0].bottom = bottom.cells[i, 4];
				bottom.cells[i, 4].top = cells[i, 0];
			}
		}
	}
}
