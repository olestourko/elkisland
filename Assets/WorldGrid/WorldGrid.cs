using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class WorldGrid : MonoBehaviour {
	public Cell cellPrefab;
	public int size;

	public float drawDistance = 30.0f;
	public Chunk chunkPrefab;
	public Chunk[,] chunks = new Chunk[100, 100];
	//These are used to speed up generation and loading
	public List<Chunk> chunks_list = new List<Chunk>();
	//public List<Cell> chunkCells = new List<Cell>();
	private List<Chunk> pendingChunks = new List<Chunk>();
	private List<Chunk> completedChunks = new List<Chunk>();
	public GameObject prescence;

	//GUI
	gui gui;
	
	public Kernel kernel_view = new Kernel5x5();
	public Kernel kernel_generate = new Kernel9x9();
	
	private Region selected_region;
	
	//Regeneration of visited areas
	private Region visible_region = new Region();
	bool visible_region_changed = false;
	private float generation_timer = 0.0f;
	
	//Paths (experimental)
	private float t = 0.0f;
	public List<Path> paths = new List<Path>();
	public Transform path_target;
	public Cell path_target_cell;
	
	//Threading (experimental)
	Thread thread = null;
	private List<Path> paths_threading_out;
	
	
	public bool ready = false;
	
	void Start () 
	{
		gui = GameObject.Find("gui").GetComponent("gui") as gui;
	}
	
	void Update()
	{
		t += Time.deltaTime;
		if(t > 1.0f)
		{
			//FixedUpdate_1s();
			t = 0.0f;
		}
		
		//Apply thread changes
		if(thread != null)
		{
			if(!thread.IsAlive)
			{
				selected_region.Reset();
				foreach(Path path in paths_threading_out)
				{
					path.partOf.Replace(path);
					selected_region.ApplyPath(path);
				}
				selected_region.Select();
				thread = null;
				//regen
				//RegenerateRegion_Threaded();
			}
		}
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
		//Note the check for paths length which allows the player to be moved to path start when game first loads
		while(chunks_to_deactivate.Count > 0 && paths.Count > 0)
		{
			Chunk chunk = chunks_to_deactivate[0];
			chunks_to_deactivate.Remove(chunk);
			chunk.makeInactive();
			//chunk.enabled = false;
			//chunks_list.Remove(chunk);
			//Destroy(chunk.gameObject);
		}

	}
	
	void FixedUpdate_1s()
	{
		//Animate path (experimental)
		foreach(Path path in paths)
		{
			//if(path != null) path.Animate();
		}
				
		//Regenerate chunks which arent visible (experimental)
		if(!IsRegenerating())
		{
			Region visible_region = new Region();
			Region invisible_region = new Region();
			foreach(Chunk chunk in chunks_list)
			{
				/*
				//Vector3 screen_position = Camera.main.WorldToViewportPoint(chunk.transform.position);
				Vector3 screen_position = Camera.allCameras[1].WorldToViewportPoint(chunk.transform.position);
				if((screen_position.x < -5.0f || screen_position.x > 5.0f 
				|| screen_position.y < -5.0f || screen_position.y > 5.0f 
				|| screen_position.z < -5.0f))
				{
					invisible_region.AddChunk(chunk);
				}
				*/
				Vector3 local_coordinates = Camera.allCameras[1].transform.InverseTransformPoint(chunk.transform.position);
				{
					if(local_coordinates.z < -5.0f) invisible_region.AddChunk(chunk);	
				}
				
			}
			selected_region.Deselect();
			selected_region = invisible_region;
			selected_region.Select();
			RegenerateRegion_Threaded();
		}
	}
	
	public Cell GetCellAt(Vector3 _position)
	{
		int i = (int)Mathf.Floor((_position.x + 0.5f) / 5) + 50;
		int j = (int)Mathf.Floor((_position.z + 0.5f) / 5) + 50;
		Chunk chunk = chunks[i, j];
		Cell cell = chunk.GetCellClosestTo(_position);
		return cell;
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
				if(kernel_array[k+size/2, l+size/2] == 1)
				{
					if(chunks[i + k, j + l] != null)
					{
						chunks_in_region.Add(chunks[i + k, j + l]);
						region.AddChunk(chunks[i + k, j + l]);
					}
				}
			}
		}
		selected_region = region;
		region.Select();
	}
	
	public void DeselectRegion()
	{
		if(selected_region == null) return;
		selected_region.Deselect();
	}
	
	/*
	public void RegenerateSelectedRegion()
	{
		RegenerateRegion(selected_region);
	}
	public void RegenerateRegion(Region _region)
	{
		if(_region == null) return;
		_region.Generate();
		for(int i = 0; i < paths.Count; i++)
		{
			foreach(Path generated_path in _region.GeneratePath(paths[i]))
			{
				if(generated_path == null) Debug.Log ("Generated Path is null");
				paths[i].Replace(generated_path);
				_region.ApplyPath(generated_path);
				//Debug.Log (paths[0].Replace(paths[i]).ToString());
			}
		}
	}
	*/
	
	//Threaded region regeneration(experimental)
	private bool IsRegenerating()
	{
		return thread != null;	
	}
	public void RegenerateRegion_Threaded()
	{
		if(selected_region == null) return;
		selected_region.Generate();
		thread = new Thread(new ThreadStart(FindRegionPaths_Threaded));
		thread.Start();
	}
	private void FindRegionPaths_Threaded()
	{
		//Debug.Log ("--- Path Thread Started");
		//System.Threading.Thread.Sleep(1000);
		paths_threading_out = new List<Path>();
		foreach(Path path in paths)
		{
			foreach(Path generated_path in selected_region.GeneratePath(path, null))
			{
				paths_threading_out.Add(generated_path);
			}
		}
		//Debug.Log ("--- Path Thread Complete");
	}
	
	private Path GenerateBranchPath(Path _path)
	{
		List<Cell> cells = _path.getCells();
		Region region = new Region();
		foreach(Chunk chunk in chunks_list) region.AddChunk(chunk);
		Cell start = cells[Random.Range(0, cells.Count-1)];
		Cell end = region.GetCells()[Random.Range(0, region.GetCells().Count-1)];
		return region.GeneratePath(start, end);
	}
	
	/*--------------------------------------------------------------------------*/
	
	
	private void ChunkInitListener(Chunk _chunk)
	{
		pendingChunks.Remove(_chunk);
		completedChunks.Add(_chunk);
		if(pendingChunks.Count == 0)
		{
			if(selected_region != null) selected_region.Deselect();
			Region region = new Region(completedChunks);
			completedChunks = new List<Chunk>();
			foreach(Chunk chunk in chunks_list) chunk.LinkCells();
			//region.Select();
			//Generate paths within region
			if(paths.Count == 0)
			{
				/*
				path = GenerateBranchPath(path);
				paths.Add (path);
				region.ApplyPath(path);
				*/
				
				//Get the closest cell to the target
				Chunk target_chunk = region.GetClosestChunk(path_target.position);
				Cell target_cell = target_chunk.GetCellClosestTo(path_target.position);
				target_cell.Select();
				//Find a path to that cell
				Path path = region.GeneratePath(region.GetCells()[0], target_cell);
				paths.Add(path);
				region.ApplyPath(path);
				
				//Set player position to start of path(to be changed later. also note that this will cause another region to generate.
				Vector3 player_position = new Vector3(path.getStart().cell_GameObject.transform.position.x,
					prescence.transform.position.y,
					(path.getStart().cell_GameObject.transform.position.z));
				
				prescence.transform.position = player_position;
				prescence.transform.LookAt(target_cell.cell_GameObject.transform.position);
				
				gui.path_count = paths.Count;
				
				ready = true;
			}
			else
			{
				//Get the closest cell to the target
				Chunk target_chunk = region.GetClosestChunk(path_target.position);
				Cell target_cell = target_chunk.GetCellClosestTo(path_target.position);
				target_cell.Select();
				
				//Attempt to extend all path into the region
				foreach(Path path in paths)
				{
					//There may be more than one path generaed fo
					foreach(Path generated_path in region.GeneratePath(path, target_cell))
					{
						if(generated_path != null)
						{
							path.Append(generated_path);
							region.ApplyPath(path);
							foreach(Cell cell in path.getCells()) cell.cell_GameObject.Redraw();
						}
					}
				}
			}

			selected_region = region;
		}
		//Update GUI
		gui.chunk_count = chunks_list.Count;
		gui.cell_count = chunks_list.Count * 5;
	}
		
	private void ApplyPath(Path _path)
	{
		foreach(Chunk chunk in chunks_list)
		{
			chunk.ApplyPath(_path);	
		}	
		
		foreach(Chunk chunk in chunks_list) chunk.Redraw();
	}
	
	/*Deprecated*/
	private List<Cell> GetAllChunkCells()
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
