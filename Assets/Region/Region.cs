using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Region {
	
	private List<MeshChunk> chunks = new List<MeshChunk>();
		
	public Region()
	{
		
	}
	public Region(List<MeshChunk> _chunks)
	{
		foreach(MeshChunk chunk in _chunks) chunks.Add(chunk);
	}
		
	public void AddChunk(MeshChunk _chunk)
	{
		if(!chunks.Contains(_chunk)) chunks.Add(_chunk);
	}
	
	public void RemoveChunk(MeshChunk _chunk)
	{
		chunks.Remove(_chunk);	
	}
	
	//resets the entire region to woods
	public void Reset()
	{
		foreach(Cell cell in this.GetCells()) 
		{
			cell.setType(Cell.CellType.Woods);
			cell.cell_GameObject.Redraw();
		}
	}
	
	public void Select()
	{
		foreach(MeshChunk chunk in chunks) chunk.Select();
	}
	
	public void Deselect()
	{
		foreach(MeshChunk chunk in chunks) chunk.Deselect();
	}
	
	//Generates/Regenerates the region
	public void Generate()
	{
		foreach(MeshChunk chunk in chunks) chunk.Generate();
	}

	//Generates a single random path in the region
	public Path GeneratePath()
	{
		List<Cell> cells = GetCells();
		Cell start = cells[0];
		Cell end = cells[cells.Count-1];
		Path path = GeneratePath(start, end);
		return path;
	}
	public Path GeneratePathRandom()
	{		
		List<Cell> cells = GetCells();
		Cell start = cells[0];
		Cell end = cells[Random.Range(0, cells.Count-1)];
		Path path = GeneratePath(start, end);
		return path;
	}
	//Uses an existing path to generate a new path within the region.
	//Use _end if the path doesn't continue out of the region
	public List<Path> GeneratePath(Path _path, Cell _end)
	{	
		List<Path> paths_out = new List<Path>();
		List<Cell> cells = this.GetCells();
		if(this.ContainsPath(_path)) 
		{
			//Debug.Log ("1: This region contains the path");
			bool on_path = false;
			Cell start = null;
			Cell end = null;
			//Detect new paths (start/end)
			foreach(Cell cell in _path.getCells())
			{
				if(cells.Contains(cell))
				{
					if(!on_path)
					{
						on_path = true;
						start = cell;
					}
				}
				//If next in path isn't in the cells and a pah is being currently constructed
				if(!cells.Contains(_path.getNext(cell)) && on_path)
				{
					on_path = false;
					end = cell;
					Path new_path = this.GeneratePath(start, end);
					new_path.partOf = _path;
					paths_out.Add(new_path);
				}
			}			
		}
		else
		{
			Debug.Log ("2: This region doesn't contain the path");
			//Check if path is on the border of the region. If so, generate it using its last point.	
			Path path_out = null;
			Cell path_start = _path.getStart();
			Cell path_end = _path.getEnd();
			//Check start of path
			foreach(Cell adjacent_cell in path_start.getNeighbors())
			{
				if(this.ContainsCell(adjacent_cell)) 
				{
					//Debug.Log ("New region has an outgoing path");	
					path_out = this.GeneratePath(adjacent_cell, _end);
					paths_out.Add(path_out);
					break;
				}
			}
			//Check end of path
			foreach(Cell adjacent_cell in path_end.getNeighbors())
			{
				if(this.ContainsCell(adjacent_cell))
				{
					//Debug.Log ("New region has an incoming path");
					path_out = this.GeneratePath(adjacent_cell, _end);
					paths_out.Add(path_out);
					break;
				}
			}
		}
		return paths_out;
	}
	public Path GeneratePath(Cell _start, Cell _end)
	{
		List<Cell> cells = GetCells();
		AStar astar = new AStar();
		Path path = astar.solve(cells, _start, _end);		
		return path;
	}
	
	public void ApplyPath(Path _path)
	{
		foreach(MeshChunk chunk in chunks) chunk.ApplyPath(_path);
		foreach(MeshChunk chunk in chunks) chunk.GeneratePathModels();
	}
		
	//Get MeshChunks bordering this region (inside)
	public List<MeshChunk> GetBorderChunks()
	{
		List<MeshChunk> chunks_out = new List<MeshChunk>();
		foreach(MeshChunk chunk in chunks)
		{
			if(!chunks.Contains(chunk.top) || !chunks.Contains(chunk.bottom)
			|| !chunks.Contains(chunk.left) || !chunks.Contains(chunk.right))
				chunks_out.Add(chunk);
		}
		return chunks_out;
	}
	//Gets cells bordering this region (inside)
	public List<Cell> GetBorderCells()
	{
		List<Cell> cells = this.GetCells();
		List<Cell> cells_out = new List<Cell>();
		foreach(MeshChunk chunk in this.GetBorderChunks())
		{
			foreach(Cell cell in chunk.getCells())
			{
				if(!cells.Contains(cell.top) || !cells.Contains(cell.bottom)
				|| !cells.Contains(cell.left) || !cells.Contains(cell.right))
					cells_out.Add(cell);
			}
		}
		return cells_out;
	}
	//Gets all cells from MeshChunks
	public List<Cell> GetCells()
	{
		List<Cell> cells_out = new List<Cell>();
		foreach(MeshChunk chunk in chunks)
		{
			foreach(Cell cell in chunk.getCells()) cells_out.Add(cell);
		}
		return cells_out;
	}
	
	public bool ContainsCell(Cell _cell)
	{
		foreach(MeshChunk chunk in chunks)
		{
			if(chunk.hasCell(_cell)) return true;
		}
		return false;
	}
	public bool ContainsChunk(MeshChunk _chunk)
	{
		return chunks.Contains(_chunk);	
	}
	public bool ContainsPath(Path _path)
	{
		List<Cell> cells = this.GetCells();
		foreach(Cell cell in _path.getCells())
		{
			if(cells.Contains(cell)) return true;
		}
		return false;
	}
	
	public MeshChunk GetClosestChunk(Vector3 _position)
	{
		MeshChunk chunk_out = null;
		float closest_distance = 65536.0f;
		foreach(MeshChunk chunk in chunks)
		{
			float distance = (chunk.transform.position - _position).magnitude;
			if(closest_distance > distance)
			{
				chunk_out = chunk;
				closest_distance = distance;
			}
		}
		return chunk_out;
	}
}
