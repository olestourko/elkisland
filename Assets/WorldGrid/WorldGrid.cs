using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldGrid : MonoBehaviour {
	/*--------------------------------------------------------------------------*/
	/*Stuff for Dijkstra (to be moved)               							*/
	/*--------------------------------------------------------------------------*/
	List<Cell> Q;
	Dictionary<Cell, int> dist;
	Dictionary<Cell, Cell> prev;
	/*--------------------------------------------------------------------------*/
	
	public Cell cellPrefab;
	public int size;
	private List<List<Cell>> cells = new List<List<Cell>>();
	
	void Start () 
	{
		generate();
		generatePath();
		//generatePath();
	}
	
	void Update () {}
	
	public void generate()
	{
		int x_offset = -(size / 2);
		int z_offset = -(size / 2);
		for(int i = 0; i < size; i++)
		{
			cells.Add(new List<Cell>());
			for(int j = 0; j < size; j++)
			{
				Cell cell = Instantiate(cellPrefab) as Cell;
				cell.transform.parent = this.transform;
				cell.transform.position = new Vector3(x_offset + i, 0, z_offset + j);
				cells[i].Add(cell);
				cell.cost = Random.Range(1, 4);
				cell.name = cell.cost + "";
				//cell.name = (i * size) + j + "";
				
				float c = (float)cell.cost/3;
				cell.renderer.material.color = new Color(c, c, c);
			}
		}
		
		/*Link the cells*/
		for(int i = 0; i < size; i++)
		{
			for(int j = 0; j < size; j++)
			{
				if(i-1 >= 0) cells[i][j].next_left = cells[i-1][j];
				if(i+1 < size) cells[i][j].next_right = cells[i+1][j];
				if(j-1 >= 0) cells[i][j].next_bottom = cells[i][j-1];
				if(j+1 < size) cells[i][j].next_top = cells[i][j+1];
			}
		}
	}
	
	private void generatePath()
	{
		Cell start = cells[0][0];
		Cell end = cells[size-1][size-1];
		//Cell start = cells[Random.Range(0, size-1)][Random.Range(0, size-1)];
		//Cell end = cells[Random.Range(0, size-1)][Random.Range(0, size-1)];
		start.setType(Cell.CellType.Path);
		end.setType(Cell.CellType.Path);
		dijkstra_FindPath(start, end);	
	}
	
	private void dijkstra_FindPath(Cell _start, Cell _end)
	{
		Q = new List<Cell>();
		dist = new Dictionary<Cell, int>();
		prev = new Dictionary<Cell, Cell>();
		
		/*Reset cell distances to infinity and previous to null*/ 
		for(int i = 0; i < size; i++)
		{
			for(int j = 0; j < size; j++)
			{
				Q.Add(cells[i][j]);
				dist.Add(cells[i][j], 65536);
				prev.Add (cells[i][j], null);
			}			
		}
		dist[_start] = 0;
		prev[_start] = _start;
		
		while(Q.Count > 0)
		{
			/*Find the cell with the shortest distance in Q (this will be the starting cell at first)*/
			Cell u = null;
			foreach(Cell cell in Q)
			{
				if(u == null) u = cell;
				else if(dist[u] > dist[cell]) u = cell;
			}
			Q.Remove(u);
			//Debug.Log ("Removed " + u.name);
			
			foreach(Cell v in u.dijkstra_GetNeighbors())
			{
				if(Q.Contains(v)) 
				{
					int alt = dist[u] + v.cost;
					if(alt < dist[v]) 
					{
						dist[v] = alt;
						prev[v] = u;
					}
				}

			}
		}
		
		foreach(Cell cell in dist.Keys)
		{
			Debug.Log (cell.name + " - " + dist[cell] + " via " + prev[cell].name);
		}
		/*Generate path*/
		List<Cell> path = new List<Cell>();
		Cell current = prev[_end];
		while (current != _start)
		{
			path.Add (current);
			current = prev[current];
		}
		Debug.Log("Path generated");
		foreach(Cell cell in path)
		{
			cell.setType(Cell.CellType.Path);
		}
	}
}
