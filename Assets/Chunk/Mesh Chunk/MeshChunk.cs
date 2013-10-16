using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeshChunk : MonoBehaviour {

	public bool isActive = true;
	public bool selected = false;
	
	private List<WeightedQuad> quads = new List<WeightedQuad>();
	
	Cell[,] cells = new Cell[5,5];
	private List<Cell> cells_list = new List<Cell>();
	private Dictionary<Cell, WeightedQuad> cell_to_quad = new Dictionary<Cell, WeightedQuad>();
	
	//Adjacent chunks
	public MeshChunk left;
	public MeshChunk top;
	public MeshChunk right;
	public MeshChunk bottom;
	
	/*Event firing when chunk is init'd*/
	public event InitHandler Init;
	public delegate void InitHandler(Chunk _chunk);
	
	
	// Use this for initialization
	void Start () 
	{
		Generate ();
	}
	
	//--------------------------------------------------------------------------
	// Everything for generation
	//--------------------------------------------------------------------------
	
	public void Generate()
	{
		GenerateCells();
		List<float> offsets = new List<float>();
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
				quad.vertex_1_weight = quad.vertex_2_weight = quad.vertex_3_weight = quad.vertex_4_weight = cell.cost;
				cell_to_quad.Add(cell, quad);
	
				

				float top, bottom, left, right = cell.cost;
				float topleft, topright, bottomleft, bottomright = cell.cost;
				
				//vert 0
				if(cell.bottom != null && cell.left != null)
				{
					if(cell.bottom.left != null)
					{
						offsets.Add(
							(cell.cost + cell.bottom.cost + cell.left.cost + cell.bottom.left.cost) / 4.0f);
					}
					else if(cell.left.bottom != null)
					{
						offsets.Add(
							(cell.cost + cell.bottom.cost + cell.left.cost + cell.left.bottom.cost) / 4.0f);						
					}
					else
					{
						offsets.Add((cell.cost + cell.bottom.cost + cell.left.cost) / 3.0f);	
					}
				} 
				else if(cell.bottom != null)
				{
					offsets.Add((cell.cost + cell.bottom.cost) / 2.0f);	
				}
				else if(cell.left != null)
				{
					offsets.Add((cell.cost + cell.left.cost) / 2.0f);	
				} 
				else offsets.Add(cell.cost);
				
				//vert 1
				if(cell.bottom != null && cell.right != null)
				{
					if(cell.bottom.right != null)
					{
						offsets.Add(
							(cell.cost + cell.bottom.cost + cell.right.cost + cell.bottom.right.cost) / 4.0f);
					}
					else if(cell.right.bottom != null)
					{
						offsets.Add(
							(cell.cost + cell.bottom.cost + cell.right.cost + cell.right.bottom.cost) / 4.0f);						
					}
					else
					{
						offsets.Add((cell.cost + cell.bottom.cost + cell.right.cost) / 3.0f);
					}
				} 
				else if(cell.bottom != null)
				{
					offsets.Add((cell.cost + cell.bottom.cost) / 2.0f);	
				}
				else if(cell.right != null)
				{
					offsets.Add((cell.cost + cell.right.cost) / 2.0f);	
				} 
				else offsets.Add(cell.cost);
				
				//vert 2
				if(cell.top != null && cell.left != null)
				{
					if(cell.top.left != null)
					{
						offsets.Add(
							(cell.cost + cell.top.cost + cell.left.cost + cell.top.left.cost) / 4.0f);
					}
					else if(cell.left.top != null)
					{
						offsets.Add(
							(cell.cost + cell.top.cost + cell.left.cost + cell.left.top.cost) / 4.0f);						
					}
					else
					{
						offsets.Add((cell.cost + cell.top.cost + cell.left.cost) / 3.0f);
					}
				} 
				else if(cell.top != null)
				{
					offsets.Add((cell.cost + cell.top.cost) / 2.0f);	
				}
				else if(cell.left != null)
				{
					offsets.Add((cell.cost + cell.left.cost) / 2.0f);	
				} 
				else offsets.Add(cell.cost);
				
				//vert 3
				if(cell.top != null && cell.right != null)
				{
					if(cell.top.right != null)
					{
						offsets.Add(
							(cell.cost + cell.top.cost + cell.right.cost + cell.top.right.cost) / 4.0f);
					}
					else if(cell.left.top != null)
					{
						offsets.Add(
							(cell.cost + cell.top.cost + cell.right.cost + cell.right.top.cost) / 4.0f);						
					}
					else
					{
						offsets.Add((cell.cost + cell.top.cost + cell.right.cost) / 3.0f);
					}
				} 
				else if(cell.top != null)
				{
					offsets.Add((cell.cost + cell.top.cost) / 2.0f);	
				}
				else if(cell.right != null)
				{
					offsets.Add((cell.cost + cell.right.cost) / 2.0f);	
				} 
				else offsets.Add(cell.cost);
			}
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
			quad.vertex_1.y = quad.vertex_1_weight * -0.5f;
			quad.vertex_2.y = quad.vertex_2_weight * -0.5f;
			quad.vertex_3.y = quad.vertex_3_weight * -0.5f;
			quad.vertex_4.y = quad.vertex_4_weight * -0.5f;
		}
		UpdateMesh();		
	}
	
	private void UpdateMesh()
	{
		Mesh mesh = new Mesh();
		GetComponent<MeshFilter>().mesh = mesh;
		mesh.vertices = GetVerts();
		mesh.triangles = GetTris();
		mesh.RecalculateNormals();	
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
		/*Create the cells*/
		for(int i = 0; i < 5; i++)
		{
			for(int j = 0; j < 5; j++)
			{
				Cell cell = cells[i, j];
				if(cell == null)
				{
					cell = new Cell(null);
					cells[i, j] = cell;
					cell.position = this.transform.position;
					cells_list.Add(cell);
				}
				cell.cost = Random.Range(1, 4);
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
	}
	
	
	
	public void ApplyPath(Path _path)
	{
		//Debug.Log ("Chunk " + name + " applied path");
		bool onPath = false;
		foreach(Cell cell in _path.getCells())
		{
			//Is this the first cell within the chunk for this path?
			//Is the next cell in the path outside of the chunk (or non-existent?)
			if(this.hasCell(cell))
			{
				if(!onPath)
				{
					cell.setType(Cell.CellType.Path_Start);
					onPath = true;
				}
				else if(!this.hasCell(_path.getNext(cell)))
				{
					cell.setType(Cell.CellType.Path_End);
					onPath = false;
					//Debug.Log ("A path runs through chunk " + name);	
				}
				else
				{
					cell.setType(Cell.CellType.Path);
				}
			}		
		}
	}
	
	public void Redraw()
	{
		foreach(Cell cell in cells_list) cell.cell_GameObject.Redraw();	
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
	}
	
	public Cell GetCellClosestTo(Vector3 _position)
	{
		Cell closest_cell = null;
		float closest_distance = 65536.0f;
		foreach(Cell cell in cells_list)
		{
			float distance = Vector3.Distance(cell.cell_GameObject.transform.position, _position);
			if(distance < closest_distance)
			{
				closest_cell = cell;
				closest_distance = distance;
			}
		}
		return closest_cell;
	}
	
}
