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
	/*--------------------------------------------------------------------------*/
	/*Chunks								           							*/
	/*--------------------------------------------------------------------------*/
	public Chunk chunkPrefab;
	public Chunk[,] chunks = new Chunk[100, 100];
	public GameObject prescence;
	/*--------------------------------------------------------------------------*/
	
	void Start () 
	{
		//generate();
		//generatePath();
	}
	
	void FixedUpdate () 
	{
		int i = (int)Mathf.Floor(prescence.transform.position.x / 5) + 50;
		int j = (int)Mathf.Floor(prescence.transform.position.z / 5) + 50;
		if(chunks[i, j] == null) 
		{
			Chunk chunk = generateChunk(i, j);
			generatePath();
		}
	}
	
	public Chunk generateChunk()
	{
		Chunk chunk = Instantiate(chunkPrefab) as Chunk;
		chunk.transform.parent = this.transform;
		chunk.transform.position = this.transform.position;
		chunk.name = "chunk";
		return chunk;
	}
	public Chunk generateChunk(int _i, int _j)
	{
		Chunk chunk = generateChunk();
		chunk.transform.position = new Vector3((_i - 50.0f)  * 5.0f, 0.0f, (_j - 50.0f) * 5.0f);
		chunks[_i, _j] = chunk;
		
		/*Setup chunk relations*/
		//Right chunk
		if(chunks[_i+1, _j] != null)
		{
			chunk.right = chunks[_i+1, _j];
			chunks[_i+1, _j].left = chunk;
		}
		//Left chunk
		if(chunks[_i-1, _j] != null)
		{
			chunk.left = chunks[_i-1, _j];
			chunks[_i-1, _j].right = chunk;
		}
		//Bottom chunk
		if(chunks[_i, _j-1] != null)
		{
			chunk.bottom = chunks[_i, _j-1];
			chunks[_i, _j-1].top = chunk;
		}
		//Top chunk
		if(chunks[_i, _j+1] != null)
		{
			chunk.top = chunks[_i, _j+1];
			chunks[_i, _j+1].bottom = chunk;
		}
		return chunk;
	}
	
	/*--------------------------------------------------------------------------*/
	/*Generate fixed region (testing)               							*/
	/*--------------------------------------------------------------------------*/	
	public void generate_Fixed()
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
				float c = (float)cell.cost/3;
				cell.renderer.material.color = new Color(c, c, c);
			}
		}
		
		/*Link the cells*/
		for(int i = 0; i < size; i++)
		{
			for(int j = 0; j < size; j++)
			{
				if(i-1 >= 0) cells[i][j].left = cells[i-1][j];
				if(i+1 < size) cells[i][j].right = cells[i+1][j];
				if(j-1 >= 0) cells[i][j].bottom = cells[i][j-1];
				if(j+1 < size) cells[i][j].top = cells[i][j+1];
			}
		}
	}
	/*--------------------------------------------------------------------------*/	
	
	
	private void generatePath()
	{
		//Cell start = cells[0][0];
		//Cell end = cells[size-1][size-1];
		//Cell start = cells[Random.Range(0, size-1)][Random.Range(0, size-1)];
		//Cell end = cells[Random.Range(0, size-1)][Random.Range(0, size-1)];
		List<Cell> cells = getAllChunkCells();
		List<Cell> removeCells = new List<Cell>();
		foreach(Cell cell in cells)
		{
			if(cell == null) removeCells.Add(cell);	
		}
		foreach(Cell cell in removeCells) cells.Remove(cell);
	
		Cell start = cells[0];
		Cell end = cells[cells.Count - 1];
		
		if(start != null && end != null)
		{
			start.setType(Cell.CellType.Path);
			end.setType(Cell.CellType.Path);
			dijkstra_FindPath(cells, start, end);
		}
	}
	
	private void dijkstra_FindPath(List<Cell> _graph, Cell _start, Cell _end)
	{
		Q = new List<Cell>();
		foreach(Cell cell in _graph)
		{
			Q.Add(cell);	
		}
		
		dist = new Dictionary<Cell, int>();
		prev = new Dictionary<Cell, Cell>();
		
		/*Reset cell distances to infinity and previous to null*/ 
		/*
		for(int i = 0; i < size; i++)
		{
			for(int j = 0; j < size; j++)
			{
				Q.Add(cells[i][j]);
				dist.Add(cells[i][j], 65536);
				prev.Add (cells[i][j], null);
			}			
		}
		*/	
		foreach(Cell cell in Q)
		{
				dist.Add (cell, 65536);
				prev.Add (cell, null);
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
		/*
		foreach(Cell cell in dist.Keys)
		{
			Debug.Log (cell.name + " - " + dist[cell] + " via " + prev[cell].name);
		}
		*/
		/*Generate path*/
		List<Cell> path = new List<Cell>();
		Cell current = prev[_end];
		while (current != _start)
		{
			path.Add (current);
			current = prev[current];
		}
		
		//Reset all blocks		
		foreach(Cell cell in _graph)
		{
			float c = (float)cell.cost/3;
			cell.renderer.material.color = new Color(c, c, c);	
		}

		foreach(Cell cell in path)
		{
			cell.setType(Cell.CellType.Path);
		}
	}
	
	private List<Cell> getAllChunkCells()
	{
		List<Cell> cells = new List<Cell>();
		for(int i = 0; i < 100; i++)
		{
			for(int j = 0; j < 100; j++)
			{
				if(chunks[i, j] != null)
				{
					foreach(Cell cell in chunks[i,j].getCells())
					{
						cells.Add(cell);	
					}
				}
			}			
		}
		return cells;
	}
}
