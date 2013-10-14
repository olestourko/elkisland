using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExperienceManager : MonoBehaviour {
	
	public WorldGrid worldGrid;
	public TrapManager trapManager;
	
	public Transform player;
	public StalkingAI stalking_ai_prefab;
	public BoltingAI bolting_ai_prefab;
	
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
			
			//spawn stalker AI if audiotrap
			if(trap as AudioTrap != null)
			{
				if(stalkingAIs.Count >= 1) break;
				StalkingAI ai = Instantiate(stalking_ai_prefab) as StalkingAI;
				ai.worldGrid = worldGrid;
				ai.target = player;
				ai.aiState = StalkingAI.AIState.Follow;
				stalkingAIs.Add(ai);
			}
			//
			else
			{
				BoltingAI ai = Instantiate(bolting_ai_prefab) as BoltingAI;
				ai.transform.position = trap.transform.position + Vector3.up * 0.15f;
				ai.target = trap.transform.position + trap.transform.forward * 10.0f;
			}
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
		
		
		//Change lighting color if player off path
		/*
		if(worldGrid.GetCellAt(player.position).cellType == Cell.CellType.Woods)
		{
			RenderSettings.fogEndDistance = 1.5f;
			RenderSettings.ambientLight = new Color(0.25f, 0.25f, 0.25f);
			RenderSettings.fogColor = new Color(0.0312f, 0.0312f, 0.0468f);
			Camera.allCameras[1].backgroundColor = RenderSettings.fogColor;
		}
		else
		{
			RenderSettings.fogEndDistance = 2.5f;
			RenderSettings.ambientLight = new Color(1.0f, 1.0f, 1.0f);
			RenderSettings.fogColor = new Color(0.0624f, 0.0624f, 0.0936f);
			Camera.allCameras[1].backgroundColor = RenderSettings.fogColor;
		}
		*/
	}
}
