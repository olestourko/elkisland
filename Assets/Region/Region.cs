using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Region {
	
	private List<Chunk> chunks = new List<Chunk>();
	public Region()
	{
		
	}
	public Region(List<Chunk> _chunks)
	{
		foreach(Chunk chunk in _chunks) chunks.Add(chunk);
	}
	
	public bool ContainsChunk(Chunk _chunk)
	{
		return chunks.Contains(_chunk);	
	}
	
	public void AddChunk(Chunk _chunk)
	{
		if(!chunks.Contains(_chunk)) chunks.Add(_chunk);
	}
	
	public void RemoveChunk(Chunk _chunk)
	{
		chunks.Remove(_chunk);	
	}
	
	public void Select()
	{
		foreach(Chunk chunk in chunks) chunk.Select();
	}
	
	public void Deselect()
	{
		foreach(Chunk chunk in chunks) chunk.Deselect();
	}
	
	//Generates a single path in the region
	public Path GeneratePath()
	{
		List<Cell> cells = GetCells();
		Cell start = cells[0];
		Cell end = cells[cells.Count-1];
		//Cell start = cells[Random.Range(0, cells.Count-1)];
		//Cell end = cells[Random.Range(0, cells.Count-1)];
		start.setType(Cell.CellType.Path);
		end.setType(Cell.CellType.Path);
		AStar astar = new AStar();
		Path path = astar.solve(cells, start, end);	
		if(path != null) ApplyPath(path);
		
		//ExistsIncomingPath();
		return path;
	}
	//Takes in a list of existing paths and figures our which are in the region, and continues them
	public List<Path> GeneratePaths(List<Path> _paths)
	{
		Debug.Log("Paths input: " + _paths.Count);
		return null;
	}
	
	//Detects if there is a path coming into the region
	private bool ExistsIncomingPath()
	{
		List<Cell> cells = this.GetCells();
		List<Cell.CellType> path_types = new List<Cell.CellType>();
		path_types.Add(Cell.CellType.Path_Start);
		path_types.Add(Cell.CellType.Path_End);
		
		foreach(Cell cell_inner in this.GetBorderCells()) 
		{	
			foreach(Cell cell_adjacent in cell_inner.getNeighbors())
			{
				if(!cells.Contains(cell_adjacent))
				{
					if(path_types.Contains(cell_adjacent.cellType))
					{
						Debug.Log("An adjacent path exists @ " + cell_inner.name);	
					}
				}
			}
		}
		return false;
	}
	
	public void Repath()
	{
		List<Path> world_paths = new List<Path>();
		//get all the paths in the region (world paths)
		foreach(Chunk chunk in chunks)
		{
			foreach(Path path in chunk.GetWorldPaths())
			{
				if(!world_paths.Contains(path) && path != null) world_paths.Add(path);
			}
		}	
		
		//regenerate the chunk weights and clear their paths
		foreach(Chunk chunk in chunks) 
		{	
			chunk.regenerate();
			chunk.ClearLocalPaths();
		}
		
		//Split world paths if necessary
		List<Path> local_paths = new List<Path>();
		Path new_local_path = null;
		foreach(Path path in world_paths)
		{
			Cell start = null;
			Cell end = null;
			foreach(Cell cell in path.getCells())
			{
				if(this.ContainsCell(cell))
				{
					if(start == null) 
					{
						new_local_path = new Path();
						new_local_path.partOf = path;
						start = cell;
					}
					else if(!this.ContainsCell(path.getNext(cell))) end = cell;
					new_local_path.addCell(cell);
				}
				
				if(start != null && end != null && new_local_path != null)
				{
					local_paths.Add(new_local_path);
					start = null;
					end = null;
					new_local_path = null;
				}
			}
		}
		
		//Apply the paths
		foreach(Path path in local_paths)
		{
			/*
			Cell start = null;
			Cell end = null;
			//Find the start and end of the path
			foreach(Cell cell in path.getCells())
			{
				if(this.ContainsCell(cell))
				{
					if(start == null) start = cell;
					else if(!this.ContainsCell(path.getNext(cell))) end = cell;
				}
			}
			//Apply the path, using the start and end
			if(start != null && end != null)
			{
				AStar astar = new AStar();
				Path newPath = astar.solve(GetCells(), start, end);
				if(newPath != null) ApplyPath(newPath);
			}
			 */
			AStar astar = new AStar();
			Path newPath = astar.solve(GetCells(), path.getStart(), path.getEnd());
			ApplyPath(newPath);
		}
		Debug.Log (local_paths.Count + " new local paths generated for " + world_paths.Count + " world paths.");
		foreach(Path path in world_paths) Debug.Log("\t" + path.getLength());
	}
	
	public void ApplyPath(Path _path)
	{
		foreach(Chunk chunk in chunks) chunk.ApplyPath(_path);
		foreach(Chunk chunk in chunks) chunk.ApplyModels();
	}
		
	//Get chunks bordering this region (inside)
	public List<Chunk> GetBorderChunks()
	{
		List<Chunk> chunks_out = new List<Chunk>();
		foreach(Chunk chunk in chunks)
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
		foreach(Chunk chunk in this.GetBorderChunks())
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
	//Gets all cells from chunks
	public List<Cell> GetCells()
	{
		List<Cell> cells_out = new List<Cell>();
		foreach(Chunk chunk in chunks)
		{
			foreach(Cell cell in chunk.getCells()) cells_out.Add(cell);
		}
		return cells_out;
	}
	
	public bool ContainsCell(Cell _cell)
	{
		foreach(Chunk chunk in chunks)
		{
			if(chunk.hasCell(_cell)) return true;
		}
		return false;
	}
}
