using UnityEngine;
using System.Collections;

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
		for(int i = 0; i < 5; i++)
		{
			for(int j = 0; j < 5; j++)
			{
				Cell cell = Instantiate(cellPrefab) as Cell;
				cells[i, j] = cell;
				cell.transform.parent = this.transform;
				cell.transform.position = transform.position + new Vector3(i, 0, j);
			}			
		}
	}
}
