using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrapManager : MonoBehaviour {
	
	public Trap trap_prefab;
	public List<Trap> traps = new List<Trap>();
		
	public List<Trap> ActivateNearbyTraps(Vector3 _position, float _distance)
	{
		List<Trap> activated_traps = new List<Trap>();
		foreach(Trap trap in traps)
		{
			if(Vector3.Distance(_position, trap.transform.position) < _distance) activated_traps.Add(trap);
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
			if(random_number < 19) continue;
			
			random_number = Random.Range(0, 2);
			Cell cell_spawn = cell.getNeighbors()[random_number];
			if(cell_spawn == null) continue;
			
			Trap trap = Instantiate(trap_prefab) as Trap;
			trap.transform.position = cell_spawn.cell_GameObject.transform.position;
			trap.transform.parent = this.transform;
			traps.Add(trap);
		}
	}
}
