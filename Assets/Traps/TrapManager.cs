using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrapManager : MonoBehaviour {
	
	public AudioTrap audio_trap_prefab;
	public ShadowTrap shadow_trap_prefab;
	public List<Trap> traps = new List<Trap>();
		
	public List<Trap> ActivateNearbyTraps(Vector3 _position, float _distance)
	{
		List<Trap> activated_traps = new List<Trap>();
		foreach(Trap trap in traps)
		{
			float distance = Vector3.Distance(_position, trap.transform.position);
			if(trap as ShadowTrap != null) distance *= 0.6f;
			if(distance < _distance) activated_traps.Add(trap);
		}
		foreach(Trap trap in activated_traps)
		{
			trap.Activate();
		}
		return activated_traps;
	}
	
	public void DestroyTrap(Trap _trap)
	{
		if(traps.Contains(_trap))
		{
			traps.Remove(_trap);
			Destroy(_trap.gameObject, 5.0f);
		}
	}
	
	public void GenerateTraps(Path _path)
	{
		foreach(Cell cell in _path.getCells())
		{
			int random_number = Random.Range(0, 20);
			if(random_number < 17) continue;
			
			//Chose a cell to spawn at (not path, but adjacent to one
			Cell cell_spawn = null;
			foreach(Cell cell_adjacent in cell.getNeighbors())
			{
				if(cell_adjacent.cellType == Cell.CellType.Woods) cell_spawn = cell_adjacent;		
			}
			if(cell_spawn == null) continue;
			
			//Spawn a random trap type
			random_number = Random.Range(0, 11);
			Trap trap = null;
			
			if(random_number < 7) trap = Instantiate(audio_trap_prefab) as AudioTrap;
			else trap = Instantiate(shadow_trap_prefab) as ShadowTrap;

 			
			trap.transform.position = cell_spawn.position;
			Vector3 trap_forward = Vector3.Normalize(cell.position - cell_spawn.position);
			trap.transform.forward = trap_forward;
			trap.transform.parent = this.transform;
			traps.Add(trap);
		}
	}
}
