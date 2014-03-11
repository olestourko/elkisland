using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class WorldGrid : MonoBehaviour {
	
	//for access outside of worldgrid
	public static WorldGrid instance;
	
	public Cell cellPrefab;
	public int size;

	public float drawDistance = 30.0f;
	public MeshChunk chunk_prefab_forest;
	public MeshChunk chunk_prefab_plain;
	public int forests_generated = 0;
	public int plains_generated = 0;
	
	public MeshChunk[,] chunks = new MeshChunk[100, 100];
	//These are used to speed up generation and loading
	public List<MeshChunk> chunks_list = new List<MeshChunk>();
	public List<MeshChunk> pending_chunks = new List<MeshChunk>();
	private List<MeshChunk> completed_chunks = new List<MeshChunk>();
	public GameObject prescence;
	
	public GameObject cottage_prefab;
	public GameObject cottage;
	
	public float closest_distance;
	
	//GUI
	gui gui;
	
	public Kernel kernel_view = new Kernel5x5();
	public Kernel kernel_generate = new Kernel9x9();
	
	private Region selected_region;
	
	//Regeneration of visited areas
	public Region visible_region = new Region();
	private float generation_timer = 0.0f;
	
	//Paths
	private float t = 0.0f;
	public List<Path> paths = new List<Path>();
	public List<Schema> path_scemas = new List<Schema>();
	public Transform path_start;
	public Transform path_end;
	
	//Path-Locations
	public List<Path_Location_Pair> pairs = new List<Path_Location_Pair>();
	private int last_location_n = 0;

	//Threading (experimental)
	Thread thread = null;
	private List<Path> paths_threading_out;
	
	public bool ready = false;
	
	void Start () 
	{
		instance = this;
		gui = GameObject.Find("gui").GetComponent("gui") as gui;

		prescence.transform.position = path_start.position + Vector3.up * 0.1f;
		GenerateRegion(path_start.position);
		GenerateRegion(path_end.position);
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
		
		//Find distance to closest path cell
		closest_distance = 65536.0f;
		foreach(Path path in paths)
		{
			foreach(Cell cell in path.getCells())
			{
				float distance = Vector3.Distance(cell.position, prescence.transform.position);
				if(distance < closest_distance) closest_distance = distance;
			}
		}
		foreach(Path_Location_Pair pair in pairs)
		{
			foreach(Cell cell in pair.path.getCells())
			{
				float distance = Vector3.Distance(cell.position, prescence.transform.position);
				if(distance < closest_distance) closest_distance = distance;
			}
		}

		
		//Draw perpendicular vectors for path cells
		//foreach(Path path in paths) path.DrawPerpVector();
		
	}
	void FixedUpdate () 
	{
		/*--------------------------------------------------------------------------*/
		/*MeshChunk Generation					               						*/
		/*--------------------------------------------------------------------------*/
		GenerateRegion(prescence.transform.position);
		
		/*--------------------------------------------------------------------------*/
		/*Initial MeshChunk Generation (For paths					               	*/
		/*--------------------------------------------------------------------------*/
		if(pending_chunks.Count == 0 && paths.Count == 1)
		{
			if (!paths[0].IsComplete())
			{
				GenerateRegion(paths[0].getEnd().position);
			}
		}

		/*--------------------------------------------------------------------------*/
		/*MeshChunk set Active/Inactive				           						*/
		/*--------------------------------------------------------------------------*/
		/*
		Vector3 pos_prescence = new Vector3(prescence.transform.position.x, 0.0f, prescence.transform.position.z);
		List<MeshChunk> chunks_to_deactivate = new List<MeshChunk>();
		foreach(MeshChunk chunk in chunks_list)
		{
			if(Vector3.Distance(chunk.transform.position, pos_prescence) > drawDistance && !pending_chunks.Contains(chunk))
			{
				chunks_to_deactivate.Add(chunk);
			}
			else
			{
				if(!visible_region.ContainsChunk(chunk)) visible_region.AddChunk(chunk);
				chunk.makeActive();
				chunk.enabled = true;
			}
		}
		//Note the check for paths length which allows the player to be moved to path start when game first loads
		while(chunks_to_deactivate.Count > 0 && paths.Count > 0)
		{
			MeshChunk chunk = chunks_to_deactivate[0];
			visible_region.RemoveChunk(chunk);
			chunks_to_deactivate.Remove(chunk);
			chunk.makeInactive();
		}
		*/
	}
	
	void FixedUpdate_1s()
	{				
		//Regenerate MeshChunks which arent visible (experimental)
		if(!IsRegenerating())
		{
			Region visible_region = new Region();
			Region invisible_region = new Region();
			foreach(MeshChunk MeshChunk in chunks_list)
			{
				/*
				//Vector3 screen_position = Camera.main.WorldToViewportPoint(MeshChunk.transform.position);
				Vector3 screen_position = Camera.allCameras[1].WorldToViewportPoint(MeshChunk.transform.position);
				if((screen_position.x < -5.0f || screen_position.x > 5.0f 
				|| screen_position.y < -5.0f || screen_position.y > 5.0f 
				|| screen_position.z < -5.0f))
				{
					invisible_region.AddMeshChunk(MeshChunk);
				}
				*/
				Vector3 local_coordinates = Camera.allCameras[1].transform.InverseTransformPoint(MeshChunk.transform.position);
				{
					if(local_coordinates.z < -5.0f) invisible_region.AddChunk(MeshChunk);	
				}
				
			}
			selected_region.Deselect();
			selected_region = invisible_region;
			selected_region.Select();
			RegenerateRegion_Threaded();
		}
	}
	
	public List<Vector3> GetAllModelPositions()
	{
		List<Vector3> positions = new List<Vector3>();
		foreach(MeshChunk chunk in chunks_list)
		{
			foreach(Vector3 position in chunk.GetRandomObjectPositions()) positions.Add(position);
		}
		return positions;
	}
	
	
	public MeshChunk GetChunkAt(Vector3 _position)
	{
		int i = (int)Mathf.Floor((_position.x + 0.5f) / 5) + 50;
		int j = (int)Mathf.Floor((_position.z + 0.5f) / 5) + 50;
		return chunks[i, j];
	}
	
	public Cell GetCellAt(Vector3 _position)
	{
		MeshChunk chunk = GetChunkAt(_position);
		Cell cell = chunk.GetCellClosestTo(_position);
		return cell;
	}
	
	//Generates a region around a position, using the specified kernel.
	public void GenerateRegion(Vector3 _position)
	{
		int i = (int)Mathf.Floor(_position.x / 5) + 50;
		int j = (int)Mathf.Floor(_position.z / 5) + 50;
		
		bool generate = false;
		int size = kernel_view.GetSize();
		int[,] kernel_array = kernel_view.GetArray();
		
		for(int k = -size/2; k <= size/2; k++)
		{
			for(int l = -size/2; l <= size/2; l++)
			{
				if(kernel_array[k+size/2, l+size/2] == 1)
				{
					if(chunks[i + k, j + l] == null) generate = true;
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
							generate = true;
						}
					}
				}
			}
		}	
	}	
	public MeshChunk CreateChunk(int _i, int _j)
	{
		forests_generated++;
		
		MeshChunk chunk_to_instantiate = chunk_prefab_forest;
		//if(forests_generated < 72) chunk_to_instantiate = chunk_prefab_forest;
		
		MeshChunk chunk = Instantiate(chunk_to_instantiate) as MeshChunk;
		chunk.transform.parent = this.transform;
		chunk.transform.position = this.transform.position;
		chunk.name = "c " + _i + ", " + _j;
		chunk.transform.position = new Vector3((_i - 50.0f)  * 5.0f, 0.0f, (_j - 50.0f) * 5.0f);
		chunks[_i, _j] = chunk;
		chunks_list.Add(chunk);
		
		/*Link to adjacent MeshChunks*/
		//Right MeshChunk
		if(chunks[_i+1, _j] != null)
		{
			chunk.right = chunks[_i+1, _j];
			chunks[_i+1, _j].left = chunk;
		}
		//Left MeshChunk
		if(chunks[_i-1, _j] != null)
		{
			chunk.left = chunks[_i-1, _j];
			chunks[_i-1, _j].right = chunk;
		}
		//Bottom MeshChunk
		if(chunks[_i, _j-1] != null)
		{
			chunk.bottom = chunks[_i, _j-1];
			chunks[_i, _j-1].top = chunk;
		}
		//Top MeshChunk
		if(chunks[_i, _j+1] != null)
		{
			chunk.top = chunks[_i, _j+1];
			chunks[_i, _j+1].bottom = chunk;
		}
		
		chunk.Init += new MeshChunk.InitHandler(ChunkInitListener);
		pending_chunks.Add(chunk);
		return chunk;
	}
		
	/*--------------------------------------------------------------------------*/
	/*Region Actions					               							*/
	/*--------------------------------------------------------------------------*/
	public void SelectRegion(int _x, int _z)
	{
		DeselectRegion();
		Region region = new Region();
		List<MeshChunk> MeshChunks_in_region = new List<MeshChunk>();
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
						MeshChunks_in_region.Add(chunks[i + k, j + l]);
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
		
	/*--------------------------------------------------------------------------*/
	
	
	private void ChunkInitListener(MeshChunk _chunk)
	{
		//Debug.Log (pending_chunks.Count + " - " + completed_chunks.Count);
		pending_chunks.Remove(_chunk);
		completed_chunks.Add(_chunk);
		if(pending_chunks.Count == 0)
		{
			if(selected_region != null) selected_region.Deselect();
			Region region = new Region(completed_chunks);
			foreach(MeshChunk chunk in chunks_list) chunk.LinkCells();
			completed_chunks = new List<MeshChunk>();

			//Get all cells accessible from path start
			Fill fill = new Fill();
			List<Node> filled_nodes = fill.Solve(new Region(chunks_list).GetClosestChunk(path_start.position).GetCellClosestTo(path_start.position));
			List<MeshChunk> filled_chunks = new List<MeshChunk>();
			foreach(Node node in filled_nodes)
			{
				foreach(MeshChunk chunk in chunks_list)
				{
					if(chunk.hasCell(node as Cell) && !filled_chunks.Contains(chunk))
						filled_chunks.Add(chunk);
				}
			}
			Region everything = new Region(filled_chunks);

			//Create initial path
			if(paths.Count == 0)
			{				
				//Create a new path and set its targets
				Cell start_cell = everything.GetClosestCell(path_start.position);
				Cell end_cell = everything.GetClosestCell(path_end.position);

				Path path = everything.GeneratePath(start_cell, end_cell);
				path.target = path_end.position;
				path.target_cell = new Region(chunks_list).GetClosestCell(path_end.position);
				paths.Add(path);
					path_scemas.Add(new Schema_MainPath(path));
				region.ApplyPath(path);
			}
			//Paths exist!
			else
			{					
				//Attempt to extend all path into the region
				foreach(Path path in paths)
				{
					Cell target_cell = everything.GetClosestCell(path.target);
					//There may be more than one path generated for
					/*
					foreach(Path generated_path in everything.GeneratePath(path, target_cell))
					{
						if(generated_path != null)
						{
							path.Append(generated_path);
							everything.ApplyPath(path);
						}
					}
					*/
					Path generated_path = everything.GeneratePath(path.getEnd(), target_cell);
					if(generated_path != null)
					{
						path.Append(generated_path);
						everything.ApplyPath(generated_path);
					}
				}
			}
			selected_region = region;


			//Run the path schema and generate branching paths where needed
			foreach(Schema schema in path_scemas)
			{
				List<int> branch_points = (schema as Schema_MainPath).Run();
				foreach(int point in branch_points)
				{
					Cell path_cell = paths[0].getCells()[point];
					Vector3 target = path_cell.position + (paths[0].GetPerpendicular(path_cell) * 8.0f);
					Cell target_cell = everything.GetClosestCell(target);
					Path path = everything.GeneratePath(path_cell, target_cell);
					path.target = target;
					path.target_cell = target_cell;

					Path_Location_Pair pair = new Path_Location_Pair();
					pair.path = path;
					pairs.Add(pair);
					everything.ApplyPath(path);
				}
			}

			//Attempt to extend each paith in path-location pairs into the region
			/*
			foreach(Path_Location_Pair pair in pairs)
			{
				Cell target_cell = everything.GetClosestCell(pair.path.target);
				foreach(Path generated_path in everything.GeneratePath(pair.path, target_cell))
				{
					if(generated_path != null)
					{
						pair.path.Append(generated_path);
						everything.ApplyPath(pair.path);
					}
				}	
			}
			*/

			//Spawn end location for each branching path if its complete
			foreach(Path_Location_Pair pair in pairs)
			{
				if(pair.path.IsComplete() && pair.location == null)
				{
					GameObject location = Instantiate(cottage_prefab) as GameObject;

					float random_rotation = Random.Range(0, Mathf.PI * 2.0f);

					location.transform.position = pair.path.getEnd().position + new Vector3(0.5f, 0, 1.5f);
					location.transform.Rotate(new Vector3(0.0f, 0.0f, random_rotation * 57.3f));
					pair.location = location.GetComponent("Location") as Location;
					pair.location.n = last_location_n;
					last_location_n++;

				}
			}

			//Update mesh height and models
			foreach(MeshChunk chunk in chunks_list) 
			{
				chunk.SmoothMesh();
				chunk.UpdateMesh();
				chunk.GenerateRandomModels();
				chunk.GeneratePathModels();
				chunk.UpdateModels();
			}	

		}

		//Update GUI
		gui.chunk = forests_generated;
		gui.cell_count = forests_generated * 25;
	}
		
	private void ApplyPath(Path _path)
	{
		foreach(MeshChunk chunk in chunks_list)
		{
			chunk.ApplyPath(_path);	
		}	
	}
	
}
