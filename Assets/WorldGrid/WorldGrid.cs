using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldGrid : MonoBehaviour {
	public Cell cellPrefab;
	public int size;
	private List<List<Cell>> cells = new List<List<Cell>>();
	/*--------------------------------------------------------------------------*/
	/*Chunks								           							*/
	/*--------------------------------------------------------------------------*/
	public float drawDistance = 30.0f;
	public Chunk chunkPrefab;
	public Chunk[,] chunks = new Chunk[100, 100];
	//These are used to speed up generation and loading
	public List<Chunk> chunks_list = new List<Chunk>();
	//public List<Cell> chunkCells = new List<Cell>();
	public List<Chunk> pendingChunks = new List<Chunk>();
	public GameObject prescence;

	//GUI
	gui gui;
	
	//Kernel must be odd x odd
	public int[,] kernel = 
	{
		{0, 0, 1, 0, 0},
		{0, 1, 1, 1, 0},
		{1, 1, 1, 1, 1},
		{0, 1, 1, 1, 0},
		{0, 0, 1, 0, 0}
	};
	
	private Region selectedRegion;
	
	/*--------------------------------------------------------------------------*/
	
	void Start () 
	{
		gui = GameObject.Find("gui").GetComponent("gui") as gui;
	}
	
	void FixedUpdate () 
	{
		/*--------------------------------------------------------------------------*/
		/*Chunk Generation					               							*/
		/*--------------------------------------------------------------------------*/
		int i = (int)Mathf.Floor(prescence.transform.position.x / 5) + 50;
		int j = (int)Mathf.Floor(prescence.transform.position.z / 5) + 50;

		for(int k = -2; k <= 2; k++)
		{
			for(int l = -2; l <= 2; l++)
			{
				if(kernel[k+2, l+2] == 1)
				{
					if(chunks[i + k, j + l] == null)
					{
						generateChunk(i + k, j + l);
					}
				}
			}
		}
		
		/*--------------------------------------------------------------------------*/
		/*Chunk set Active/Inactive				           							*/
		/*--------------------------------------------------------------------------*/
		Vector3 pos_prescence = new Vector3(prescence.transform.position.x, 0.0f, prescence.transform.position.z);
		foreach(Chunk chunk in chunks_list)
		{
			if(Vector3.Distance(chunk.transform.position, pos_prescence) > drawDistance)
			{
				chunk.makeInactive();
			}
			else
			{
				chunk.makeActive();	
			}
		}
	}
	
	public Chunk generateChunk(int _i, int _j)
	{
		Chunk chunk = Instantiate(chunkPrefab) as Chunk;
		chunk.transform.parent = this.transform;
		chunk.transform.position = this.transform.position;
		chunk.name = "c " + _i + ", " + _j;
		chunk.transform.position = new Vector3((_i - 50.0f)  * 5.0f, 0.0f, (_j - 50.0f) * 5.0f);
		chunks[_i, _j] = chunk;
		
		chunks_list.Add(chunk);
		
		/*Link to adjacent chunks*/
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
		
		chunk.Init += new Chunk.InitHandler(chunkInitListener);
		pendingChunks.Add(chunk);
		return chunk;
	}
	
	public void regenerateChunk(int _i, int _j)
	{
		int i = _i;
		int j = _j;
		for(int k = -2; k <= 2; k++)
		{
			for(int l = -2; l <= 2; l++)
			{
				if(kernel[k+2, l+2] == 1)
				{
					if(chunks[i + k, j + l] != null)
					{
						chunks[i + k, j + l].regenerate();
					}
				}
			}
		}
		generatePath();
	}
	
	
	/*--------------------------------------------------------------------------*/
	/*Region Actions					               							*/
	/*--------------------------------------------------------------------------*/
	public void SelectRegion(int _x, int _z)
	{
		DeselectRegion();
		Region region = new Region();
		List<Chunk> chunks_in_region = new List<Chunk>();
		int i = _x;
		int j = _z;
		for(int k = -2; k <= 2; k++)
		{
			for(int l = -2; l <= 2; l++)
			{
				if(kernel[k+2, l+2] == 1)
				{
					if(chunks[i + k, j + l] != null)
					{
						chunks_in_region.Add(chunks[i + k, j + l]);
						region.AddChunk(chunks[i + k, j + l]);
					}
				}
			}
		}
		selectedRegion = region;
		region.Select();
	}
	
	public void DeselectRegion()
	{
		if(selectedRegion == null) return;
		selectedRegion.Deselect();
	}
	
	public void RepathRegion()
	{
		if(selectedRegion == null) return;
		selectedRegion.Repath();
	}
	
	/*--------------------------------------------------------------------------*/
	
	
	private void chunkInitListener(Chunk _chunk)
	{
		//Debug.Log ("Chunk " + _chunk.name + " Init'd");
		pendingChunks.Remove(_chunk);
		//foreach(Cell cell in _chunk.getCells()) chunkCells.Add(cell);
		if(pendingChunks.Count == 0)
		{
			foreach(Chunk chunk in chunks_list) chunk.link();
			generatePath();
		}
		gui.chunk_count = chunks_list.Count;
		gui.cell_count = chunks_list.Count * 5;
	}
	
	private void generatePath()
	{
		List<Cell> cells = getAllChunkCells();
		Cell start = cells[0];
		Cell end = cells[cells.Count-1];
		//Cell start = cells[Random.Range(0, cells.Count-1)];
		//Cell end = cells[Random.Range(0, cells.Count-1)];
		start.setType(Cell.CellType.Path);
		end.setType(Cell.CellType.Path);
		
		//Dijkstra dijkstra = new Dijkstra();
		//Path path = dijkstra.solve(cells, start, end);
		AStar astar = new AStar();
		Path path = astar.solve(cells, start, end);	
		applyPath(path);
	}
		
	private void applyPath(Path _path)
	{
		//Reset the blocks
		foreach(Cell cell in getAllChunkCells())
		{
			cell.setType(Cell.CellType.Woods);
		}
		foreach(Cell cell in _path.getCells())
		{
			cell.setType(Cell.CellType.Path);
		}
		foreach(Chunk chunk in chunks_list)
		{
			chunk.applyPath(_path);	
		}
		
		/* For each chunk where in which a cell from the path is within it, 
		 * apply the path to that cell. The chunk will create its own internal
		 * path object which will be used in region regeneration.
		 */
	}
	
	/*Deprecated*/
	private List<Cell> getAllChunkCells()
	{
		List<Cell> cells_out = new List<Cell>();
		foreach(Chunk chunk in chunks_list)
		{
			foreach(Cell cell in chunk.getCells())
			{
				cells_out.Add(cell);	
			}
		}
		return cells_out;
	}
	
}
