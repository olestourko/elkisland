using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldGrid : MonoBehaviour {
	public Cell cellPrefab;
	public int size;
	private List<List<Cell>> cells = new List<List<Cell>>();

	public float drawDistance = 30.0f;
	public Chunk chunkPrefab;
	public Chunk[,] chunks = new Chunk[100, 100];
	//These are used to speed up generation and loading
	public List<Chunk> chunks_list = new List<Chunk>();
	//public List<Cell> chunkCells = new List<Cell>();
	private List<Chunk> pendingChunks = new List<Chunk>();
	private List<Chunk> completedChunks = new List<Chunk>();
	private Region previous_region;
	public GameObject prescence;

	//GUI
	gui gui;
	
	public Kernel kernel_view = new Kernel5x5();
	public Kernel kernel_generate = new Kernel9x9();
	
	private Region selectedRegion;
	
	//Regeneration of visited areas
	private Region visible_region = new Region();
	bool visible_region_changed = false;
	private float generation_timer = 0.0f;
	
	//Paths (experimental)
	private float t = 0.0f;
	private List<Path> paths = new List<Path>();
	
	void Start () 
	{
		gui = GameObject.Find("gui").GetComponent("gui") as gui;
	}
	
	void Update()
	{
		t += Time.deltaTime;
		if(t > 0.1f)
		{
			FixedUpdate_1s();
			t = 0.0f;
		}
		/*
		generation_timer += Time.deltaTime;
		//Regenerate chunks which arent visible
		visible_region_changed = false;
		foreach(Chunk chunk in chunks_list)
		{
			Vector3 screen_position = Camera.main.WorldToViewportPoint(chunk.transform.position);
			if((screen_position.x < 0.0f || screen_position.x > 1.0f || screen_position.y < 0.0f || screen_position.y > 1.0f))
			{
				if(!visible_region.ContainsChunk(chunk))
				{
					chunk.Select();
					visible_region.AddChunk(chunk);
					visible_region_changed = true;
				}
			}
			else
			{
				chunk.Deselect();
				visible_region.RemoveChunk(chunk);
			}
			if(generation_timer > 3.0f) 
			{
				generation_timer = 0.0f;
				visible_region.Repath();
			}
		}
		*/
	}
	
	void FixedUpdate () 
	{
		/*--------------------------------------------------------------------------*/
		/*Chunk Generation					               							*/
		/*--------------------------------------------------------------------------*/
		int i = (int)Mathf.Floor(prescence.transform.position.x / 5) + 50;
		int j = (int)Mathf.Floor(prescence.transform.position.z / 5) + 50;
		
		bool generate = false;
		int size = kernel_view.GetSize();
		int[,] kernel_array = kernel_view.GetArray();
		for(int k = -size/2; k <= size/2; k++)
		{
			for(int l = -size/2; l <= size/2; l++)
			{
				if(kernel_array[k+size/2, l+size/2] == 1)
				{
					if(chunks[i + k, j + l] == null)
					{
						generate = true;
						//CreateChunk(i + k, j + l);
					}
				}
			}
		}
		
		size = kernel_generate.GetSize();
		kernel_array = kernel_generate.GetArray();
		if(generate)
		{
			for(int k = -size/2; k <= size/2; k++)
			{
				for(int l = -size/2; l <= size/2; l++)
				{
					if(kernel_array[k+size/2, l+size/2] == 1)
					{
						if(chunks[i + k, j + l] == null)
						{
							CreateChunk(i + k, j + l);
						}
					}
				}
			}
		}
		
		
		
		/*--------------------------------------------------------------------------*/
		/*Chunk set Active/Inactive				           							*/
		/*--------------------------------------------------------------------------*/
		/*
		Vector3 pos_prescence = new Vector3(prescence.transform.position.x, 0.0f, prescence.transform.position.z);
		List<Chunk> chunks_to_deactivate = new List<Chunk>();
		foreach(Chunk chunk in chunks_list)
		{
			if(Vector3.Distance(chunk.transform.position, pos_prescence) > drawDistance)
			{
				chunks_to_deactivate.Add(chunk);
			}
			else
			{
				chunk.makeActive();
				chunk.enabled = true;
			}
		}
		while(chunks_to_deactivate.Count > 0)
		{
			Chunk chunk = chunks_to_deactivate[0];
			chunks_to_deactivate.Remove(chunk);
			chunk.makeInactive();
			chunk.enabled = false;
			//chunks_list.Remove(chunk);
			//Destroy(chunk.gameObject);
		}
		*/
	}
	
	void FixedUpdate_1s()
	{
		//Animate path (experimental)
		foreach(Path path in paths)
		{
			if(path != null) path.Animate();
		}
	}
	
	public Chunk CreateChunk(int _i, int _j)
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
		
		chunk.Init += new Chunk.InitHandler(ChunkInitListener);
		pendingChunks.Add(chunk);
		return chunk;
	}
	
	public void regenerateChunk(int _i, int _j)
	{
		int i = _i;
		int j = _j;
		
		int size = kernel_view.GetSize();
		int[,] kernel_array = kernel_view.GetArray();
		
		for(int k = -size/2; k <= size/2; k++)
		{
			for(int l = -size/2; l <= size/2; l++)
			{
				if(kernel_array[k+size/2, l+size/2] == 1)
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
		
		int size = kernel_view.GetSize();
		int[,] kernel_array = kernel_view.GetArray();
		for(int k = -size/2; k <= size/2; k++)
		{
			for(int l = -size/2; l <= size/2; l++)
			{
				if(kernel_view.GetArray()[k+size/2, l+size/2] == 1)
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
	
	
	private void ChunkInitListener(Chunk _chunk)
	{
		pendingChunks.Remove(_chunk);
		completedChunks.Add(_chunk);
		if(pendingChunks.Count == 0)
		{
			if(previous_region != null) previous_region.Deselect();
			Region region = new Region(completedChunks);
			region.Select();
			completedChunks = new List<Chunk>();
			foreach(Chunk chunk in chunks_list) chunk.LinkCells();
			//Generate paths within region
			if(paths.Count == 0)
			{
				Path path = region.GeneratePath();
				if(path != null) 
				{
					paths.Add(path);
					gui.path_count = paths.Count;
				}
			}
			else
			{
				region.GeneratePaths(paths);
			}

			previous_region = region;
			//generatePath(); //Old, whole-map generation
		}
		//Update GUI
		gui.chunk_count = chunks_list.Count;
		gui.cell_count = chunks_list.Count * 5;
	}
	
	private void generatePath()
	{
		//Debug.Log ("START----------------");
		Reset();
		
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
		//Debug.Log ("END----------------");
		
		//Generate a 2nd path
		/*
		start = path.getRandomCell();
		end = cells[Random.Range(0, cells.Count-1)];
		start.setType(Cell.CellType.Path);
		end.setType(Cell.CellType.Path);
		astar = new AStar();
		path = astar.solve(cells, start, end);	
		applyPath(path);
		*/
	}
	
	//Reset all chunks
	private void Reset()
	{
		//Reset the blocks
		foreach(Cell cell in getAllChunkCells())
		{
			cell.setType(Cell.CellType.Woods);
		}
		foreach(Chunk chunk in chunks_list) chunk.ClearLocalPaths();
	
	}
	
	private void applyPath(Path _path)
	{
		foreach(Chunk chunk in chunks_list)
		{
			chunk.ApplyPath(_path);	
		}	
		
		foreach(Chunk chunk in chunks_list) chunk.ApplyModels();
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
