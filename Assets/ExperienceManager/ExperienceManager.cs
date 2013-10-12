using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExperienceManager : MonoBehaviour {
	
	public WorldGrid worldGrid;
	public TrapManager trapManager;
	
	public Transform player;
	public StalkingAI ai_prefab;
	
	private List<StalkingAI> stalkingAIs = new List<StalkingAI>();
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		//Generate traps
		if(worldGrid.ready)
		{
			worldGrid.ready = false;	
			List<Path> paths = worldGrid.paths;
			foreach(Path path in paths)
			{
				trapManager.GenerateTraps(path);	
			}
		}
		
		//Activate traps and instantiate an AI
		List<Trap> activated_traps = trapManager.ActivateNearbyTraps(player.position, 1.5f);
		foreach(Trap trap in activated_traps)
		{
			trapManager.DestroyTrap(trap);
			StalkingAI ai = Instantiate(ai_prefab) as StalkingAI;
			ai.worldGrid = worldGrid;
			ai.target = player;
			ai.aiState = StalkingAI.AIState.Follow;
			stalkingAIs.Add(ai);
		}
		
		//remove AIs who have followed the player enough
		List<StalkingAI> ai_to_remove = new List<StalkingAI>();
		foreach(StalkingAI ai in stalkingAIs)
		{
			if(ai.distance_travelled > 15.0f) ai_to_remove.Add(ai);
		}
		
		while(ai_to_remove.Count > 0)
		{
			StalkingAI ai = ai_to_remove[0];
			ai_to_remove.Remove(ai);
			stalkingAIs.Remove(ai);
			Destroy(ai.gameObject);
		}
	}
}
