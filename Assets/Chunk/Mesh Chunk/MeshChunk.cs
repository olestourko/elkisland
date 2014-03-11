using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeshChunk : MonoBehaviour {

	public bool isActive = true;
	public bool selected = false;
	
	private List<WeightedQuad> quads = new List<WeightedQuad>();
	
	Cell[,] cells = new Cell[5,5];
	private List<Cell> cells_list;
	private Dictionary<Cell, WeightedQuad> cell_to_quad;
	
	//Adjacent chunks
	public MeshChunk left;
	public MeshChunk top;
	public MeshChunk right;
	public MeshChunk bottom;
	
	/*Event firing when chunk is init'd*/
	public event InitHandler Init;
	public delegate void InitHandler(MeshChunk _chunk);
	
	
	/*Used by cells for instantiating paths and random models*/
	public GameObject model_turn;
	public List<GameObject> model_straight;
	public List<GameObject> models_random;
		public List<float> spawn_probablity;
		private List<Range> spawn_ranges;
	
	public Material random_model_material;
	
	//used for scaling the height of cells
	private float height_factor = -0.15f;
	
	//used for generating the chunk without a worldgrid
	public bool dev = false;
	
	public ChunkType chunkType;
	public enum ChunkType
	{
		Forest,
		Plain
	}
	
	
	// Use this for initialization
	void Start () 
	{
		/*Setup spawn probabilities*/
		spawn_ranges = new List<Range>();
		float range_last = 0.0f;
		foreach(float p in spawn_probablity)
		{
			float range_end = range_last + p;				
			spawn_ranges.Add(new Range(range_last, range_end));
			range_last = range_end;
		}
	
		Generate();
		if(dev)
		{
			SmoothMesh();
			UpdateMesh();
			GenerateRandomModels();
			UpdateModels();
		}
		if(Init != null) Init(this);
	}
	
	//--------------------------------------------------------------------------
	// Everything for generation
	//--------------------------------------------------------------------------
	
	//Generates mesh
	public void Generate()
	{
		GenerateCells();
		quads = new List<WeightedQuad>();
		cell_to_quad = new Dictionary<Cell, WeightedQuad>();
		//generate quads (each quad == 1 cell)
		for(int i = 0; i < 5; i++)
		{
			
			for(int j = 0; j < 5; j++)
			{
				Vector3 v1 = new Vector3(i, 0, j);
				Vector3 v2 = new Vector3(i+1, 0, j);
				Vector3 v3 = new Vector3(i, 0, j+1);
				Vector3 v4 = new Vector3(i+1, 0, j+1);
				WeightedQuad quad = new WeightedQuad(v1, v2, v3, v4);
				quads.Add(quad);
				
				//Associate a cell and get the y offsets for quad verts based on cel heights
				Cell cell = cells[i, j];
				quad.vertex_1_weight = quad.vertex_2_weight = quad.vertex_3_weight = quad.vertex_4_weight = cell.height;
				cell_to_quad.Add(cell, quad);
			}
		}
		UpdateMesh();		
	}
	public void SmoothMesh()
	{
		List<float> offsets = new List<float>();
		foreach(Cell cell in cells_list)
		{	
			//vert 0
			if(cell.bottom != null && cell.left != null)
			{
				if(cell.bottom.left != null)
				{
					offsets.Add(
						(cell.height + cell.bottom.height + cell.left.height + cell.bottom.left.height) / 4.0f);				}
				else if(cell.left.bottom != null)
				{
					offsets.Add(
						(cell.height + cell.bottom.height + cell.left.height + cell.left.bottom.height) / 4.0f);						
				}
				else
				{
					offsets.Add((cell.height + cell.bottom.height + cell.left.height) / 3.0f);	
				}
			} 
			else if(cell.bottom != null)
			{
				offsets.Add((cell.height + cell.bottom.height) / 2.0f);	
			}
			else if(cell.left != null)
			{
				offsets.Add((cell.height + cell.left.height) / 2.0f);	
			} 
			else offsets.Add(cell.height);
					
			//vert 1
			if(cell.bottom != null && cell.right != null)
			{
				if(cell.bottom.right != null)
				{
					offsets.Add(
						(cell.height + cell.bottom.height + cell.right.height + cell.bottom.right.height) / 4.0f);
				}
				else if(cell.right.bottom != null)
				{
					offsets.Add(
						(cell.height + cell.bottom.height + cell.right.height + cell.right.bottom.height) / 4.0f);						
				}
				else
				{
					offsets.Add((cell.height + cell.bottom.height + cell.right.height) / 3.0f);
				}
			} 
			else if(cell.bottom != null)
			{
				offsets.Add((cell.height + cell.bottom.height) / 2.0f);	
			}
			else if(cell.right != null)
			{
				offsets.Add((cell.height + cell.right.height) / 2.0f);	
			} 
			else offsets.Add(cell.height);
					
			//vert 2
			if(cell.top != null && cell.left != null)
			{
				if(cell.top.left != null)
				{
					offsets.Add(
						(cell.height + cell.top.height + cell.left.height + cell.top.left.height) / 4.0f);
				}
				else if(cell.left.top != null)
				{
					offsets.Add(
						(cell.height + cell.top.height + cell.left.height + cell.left.top.height) / 4.0f);						
				}
				else
				{
					offsets.Add((cell.height + cell.top.height + cell.left.height) / 3.0f);
				}
			} 
			else if(cell.top != null)
			{
				offsets.Add((cell.height + cell.top.height) / 2.0f);	
			}
			else if(cell.left != null)
			{
				offsets.Add((cell.height + cell.left.height) / 2.0f);	
			} 
			else offsets.Add(cell.height);
					
			//vert 3
			if(cell.top != null && cell.right != null)
			{
				if(cell.top.right != null)
				{
					offsets.Add(
						(cell.height + cell.top.height + cell.right.height + cell.top.right.height) / 4.0f);
				}
				else if(cell.left.top != null)
				{
					offsets.Add(
						(cell.height + cell.top.height + cell.left.height + cell.left.top.height) / 4.0f);						
				}
				else
				{
					offsets.Add((cell.height + cell.top.height + cell.right.height) / 3.0f);
				}
			} 
			else if(cell.top != null)
			{
				offsets.Add((cell.height + cell.top.height) / 2.0f);	
			}
			else if(cell.right != null)
			{
				offsets.Add((cell.height + cell.right.height) / 2.0f);	
			} 
			else offsets.Add(cell.height);
		}
		int quad_number = 0;
		foreach(WeightedQuad quad in quads)
		{
			quad.vertex_1_weight = offsets[quad_number + 0];
			quad.vertex_2_weight = offsets[quad_number + 1];
			quad.vertex_3_weight = offsets[quad_number + 2];
			quad.vertex_4_weight = offsets[quad_number + 3];		
			quad_number += 4;
		}
		//Apply vert weights to each quad
		foreach(WeightedQuad quad in quads)
		{
			quad.vertex_1.y = (quad.vertex_1_weight * height_factor);
			quad.vertex_2.y = (quad.vertex_2_weight * height_factor);
			quad.vertex_3.y = (quad.vertex_3_weight * height_factor);
			quad.vertex_4.y = (quad.vertex_4_weight * height_factor);
		}
	}
	public void UpdateMesh()
	{	
		//Generate basic, world-space uv map
		Vector3[] verts = GetVerts();
		Vector2[] uv = new Vector2[verts.Length]; 
		int i = 0;
		while(i < uv.Length)
		{
			uv[i] = new Vector2(verts[i].x, verts[i].z);
			i++;
		}	
		
		Mesh mesh = new Mesh();
		GetComponent<MeshFilter>().mesh = mesh;
		mesh.vertices = verts;
		mesh.triangles = GetTris();
		mesh.uv = uv;
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		GetComponent<MeshCollider>().sharedMesh = mesh;
	}
	private Vector3[] GetVerts()
	{
		List<Vector3> verts_list = new List<Vector3>();
		foreach(WeightedQuad quad in quads)
		{
			verts_list.Add(quad.vertex_1);
			verts_list.Add(quad.vertex_2);
			verts_list.Add(quad.vertex_3);
			verts_list.Add(quad.vertex_4);
		}
		Vector3[] verts_array = verts_list.ToArray();
		return verts_array;
	}	
	private int[] GetTris()
	{
		List<Triangle> tris = new List<Triangle>();
		foreach(WeightedQuad quad in quads)
		{
			tris.Add(quad.triangle_1);
			tris.Add(quad.triangle_2);
		}
		int last_triangle_index = 0;
		int vertex_offset = 0;
		int current_triangle_num = 0;
		//each triangle has 3 verts and each vert needs an index
		int[] tris_array = new int[tris.Count * 3];
		
		while(current_triangle_num < tris.Count)
		{
			//Triangle 1
			tris_array[last_triangle_index + 0] = vertex_offset + 0;
			tris_array[last_triangle_index + 1] = vertex_offset + 1;
			tris_array[last_triangle_index + 2] = vertex_offset + 2;
			
			//Triangle 2
			tris_array[last_triangle_index + 3] = vertex_offset + 2;
			tris_array[last_triangle_index + 4] = vertex_offset + 1;
			tris_array[last_triangle_index + 5] = vertex_offset + 3;
			last_triangle_index += 6;
			vertex_offset += 4;
			current_triangle_num += 2;
		}
		
		return tris_array;
	}
	private void GenerateCells()
	{
		cells_list = new List<Cell>();
		/*Create the cells*/
		for(int i = 0; i < 5; i++)
		{
			for(int j = 0; j < 5; j++)
			{
				Cell cell = cells[i, j];
				if(cell == null)
				{
					cell = new Cell();
					cells[i, j] = cell;
					cell.position = this.transform.position + new Vector3(i, 0, j);
					cells_list.Add(cell);
				}
				cell.cost = Random.Range(1, 4);
				cell.height = cell.cost - 1.0f;
				cell.setType(Cell.CellType.Woods);
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
		foreach(Cell cell in cells_list) cell.UpdateAdjacentLinks();
	}
	public void ApplyPath(Path _path)
	{
		bool onPath = false;
		foreach(Cell cell in _path.getCells())
		{
			//Is this the first cell within the chunk for this path?
			//Is the next cell in the path outside of the chunk (or non-existent?)
			if(this.hasCell(cell))
			{
				if(!onPath)
				{
					cell.setType(Cell.CellType.Path);
					onPath = true;
				}
				else if(!this.hasCell(_path.getNext(cell)))
				{
					cell.setType(Cell.CellType.Path);
					onPath = false;
				}
				else
				{
					cell.setType(Cell.CellType.Path);
				}
			}	
		}
	}
		
	//--------------------------------------------------------------------------
	// Get various sets of cells
	//--------------------------------------------------------------------------
	public List<Cell> getCells() { return cells_list; }
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
	
	public void makeActive() { gameObject.SetActive(true); }
	public void makeInactive() { gameObject.SetActive(false); }
	
	public void Select()
	{
		selected = true;
		foreach(Cell cell in getCells()) cell.Select();	
	}
	public void Deselect()
	{
		selected = false;
		foreach(Cell cell in getCells()) cell.Deselect();
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
		
		foreach(Cell cell in cells_list) cell.UpdateAdjacentLinks();
	}
	
	public Cell GetCellClosestTo(Vector3 _position)
	{
		Cell closest_cell = null;
		float closest_distance = 65536.0f;
		foreach(Cell cell in cells_list)
		{
			float distance = Vector3.Distance(cell.position, _position);
			if(distance < closest_distance)
			{
				closest_cell = cell;
				closest_distance = distance;
			}
		}
		return closest_cell;
	}
	
	//--------------------------------------------------------------------------
	// For instantiating cell related models (move later?)
	//--------------------------------------------------------------------------
	public void GenerateRandomModels()
	{
		foreach(Cell cell in cells_list)
		{
			Destroy(cell.model_random);
			if(cell.cellType != Cell.CellType.Woods) 
			{
				continue;
			}
			float r = Random.Range(0, 1000) / 1000.0f;
			foreach(Range range in spawn_ranges)
			{
				int range_index = spawn_ranges.IndexOf(range);
				//This is the last possible range
				if(range_index == spawn_ranges.Count-1 || range.WithinRange(r))
				{
					GameObject model_d = models_random[range_index];
					InstantiateRandomModel(model_d, cell);
					break;
				}
			}
		}
	}
	public void UpdateModels()
	{
		foreach(Cell cell in cells_list)
		{
			if(cell.model_random == null) continue;
			float y = cell_to_quad[cell].GetAverageWeight() * 0.115f;
			Vector3 position = new Vector3(cell.model_random.transform.position.x, y, cell.model_random.transform.position.z);
			cell.model_random.transform.position = position;
		}
	}
	
	public void GeneratePathModels()
	{
		foreach(Cell cell in cells_list)
		{
			List<Cell.CellType> valid_cell_types = new List<Cell.CellType>();
			valid_cell_types.Add(Cell.CellType.Path);
			valid_cell_types.Add(Cell.CellType.Path_Start);
			valid_cell_types.Add(Cell.CellType.Path_End);	
			if(!valid_cell_types.Contains(cell.cellType)) continue;
			Destroy(cell.model_random);

			int num_adjacent_paths = 0;
			if(cell.left != null) { if(valid_cell_types.Contains(cell.left.cellType)) num_adjacent_paths++; }
			if(cell.right != null) { if(valid_cell_types.Contains(cell.right.cellType)) num_adjacent_paths++; }
			if(cell.top != null) { if(valid_cell_types.Contains(cell.top.cellType)) num_adjacent_paths++; }
			if(cell.bottom != null) { if(valid_cell_types.Contains(cell.bottom.cellType)) num_adjacent_paths++; }
			if(num_adjacent_paths > 2) continue;
			
			GameObject model_instance = null;
			//Straights
			if(cell.left != null && cell.right != null)
			{
				if(valid_cell_types.Contains(cell.left.cellType) && valid_cell_types.Contains(cell.right.cellType))
				{
					GameObject model = model_straight[Random.Range(0, model_straight.Count)];
					model_instance = InstantiateModel(model, cell);
					continue;
				}
			}
			
			if(cell.top != null && cell.bottom != null)
			{
				if(valid_cell_types.Contains(cell.top.cellType) && valid_cell_types.Contains(cell.bottom.cellType))
				{
					GameObject model = model_straight[Random.Range(0, model_straight.Count)];
					model_instance = InstantiateModel(model, cell);
					model_instance.transform.Rotate(new Vector3(0.0f, 0.0f, (Mathf.PI / 2.0f) * 57.3f));
					//model_instance.transform.RotateAround(Vector3.up, Mathf.PI / 2.0f);
					continue;
				}
			}
			//Turns
			if(cell.top != null && cell.left != null)
			{
				if(valid_cell_types.Contains(cell.top.cellType) && valid_cell_types.Contains(cell.left.cellType))
				{
					model_instance = InstantiateModel(model_turn, cell);
					continue;
				}
			}
			if(cell.bottom != null && cell.right != null)
			{
				if(valid_cell_types.Contains(cell.bottom.cellType) && valid_cell_types.Contains(cell.right.cellType))
				{
					model_instance = InstantiateModel(model_turn, cell);
					model_instance.transform.Rotate(new Vector3(0.0f, 0.0f, (Mathf.PI) * 57.3f));
					//model_instance.transform.RotateAround(Vector3.up, Mathf.PI);
					continue;
				}
			}
			
			if(cell.top != null && cell.right != null)
			{
				if(valid_cell_types.Contains(cell.top.cellType) && valid_cell_types.Contains(cell.right.cellType))
				{
					model_instance = InstantiateModel(model_turn, cell);
					model_instance.transform.localScale = new Vector3(-1.0f, 1.0f, -1.0f);
					//model_instance.transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
					continue;
				}
			}
			if(cell.bottom != null && cell.left != null)
			{
				if(valid_cell_types.Contains(cell.bottom.cellType) && valid_cell_types.Contains(cell.left.cellType))
				{
					model_instance = InstantiateModel(model_turn, cell);
					model_instance.transform.Rotate(new Vector3(0.0f, 0.0f, (Mathf.PI) * 57.3f));
					model_instance.transform.localScale = new Vector3(-1.0f, 1.0f, -1.0f);
					//model_instance.transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
					continue;
				}
			}
			cell.model_instance = model_instance;
		}
	}
	private GameObject InstantiateModel(GameObject _model, Cell _cell)
	{
		Destroy(_cell.model_random);
		GameObject model_instance = Instantiate(_model) as GameObject;
		model_instance.transform.position = _cell.position + new Vector3(0.5f, 0, 0.5f);
		model_instance.transform.parent = this.transform;
		return model_instance;
	}
	private GameObject InstantiateRandomModel(GameObject _model, Cell _cell)
	{
		_cell.model_random = Instantiate(_model) as GameObject;
		float random_offset_x = Random.Range(-0.5f, 0.5f);
		float random_offset_z = Random.Range(-0.5f, 0.5f);
		float random_rotation = Random.Range(0, Mathf.PI * 2.0f);
		float y = cell_to_quad[_cell].GetAverageWeight() * 0.05f;
		_cell.model_random.transform.position = _cell.position + new Vector3(random_offset_x, y, random_offset_z) + new Vector3(0.5f, 0, 0.5f);
		_cell.model_random.transform.Rotate(new Vector3(0.0f, 0.0f, random_rotation * 57.3f));
		_cell.model_random.transform.parent = this.transform;	
		return _cell.model_random;
	}
	
	public List<Vector3> GetRandomObjectPositions()
	{
		List<Vector3> objects_out = new List<Vector3>();
		foreach(Cell cell in cells_list)
		{
			if(cell.model_random != null) objects_out.Add(cell.model_random.transform.position);
		}
		return objects_out;
	}
	
}
