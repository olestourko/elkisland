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
	
	//Lighting
	private LightingState lighting_0;
	private LightingState lighting_1;
	private LightingState lighting_2;
	private float count = 0.0f;
	public GameObject sky;
	
	// Use this for initialization
	void Start () 
	{
		//black
		lighting_0 = new LightingState(
			new Color(0.0f, 0.0f, 0.0f),
			new Color(0.0f,  0.0f, 0.0f),
			new Color(0.0f,  0.0f, 0.0f),
			0.2f
		);
		lighting_0.sky_factor = 0.0f;
		//blue-green
		lighting_1 = new LightingState(
			new Color(0.184f, 0.199f, 0.199f),		//ambient
			new Color(0.117f,  0.293f, 0.293f),		//fog
			new Color(0.117f,  0.293f, 0.293f),		//sky
			0.2f
		);
		//dark
		lighting_2 = new LightingState(
			new Color(0.098f, 0.098f, 0.098f),
			new Color(0.059f, 0.059f, 0.059f),
			new Color(0.059f, 0.059f, 0.059f),
			//new Color(0.156f, 0.117f, 0.176f),
			0.2f
		);
		lighting_2.sky_factor = 0.2f;
		//ApplyLightingState(lighting_2);
	}
	
	// Update is called once per frame
	void Update () 
	{
		//Blend lighting
		if(count <= 1.0f)
		{
			LightingState blended = lighting_0.Blend(lighting_1, count);
			ApplyLightingState(blended);
			count += Time.deltaTime * 0.5f;
		}
		
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
				ai.transform.position = trap.transform.position + (Vector3.up * 0.11f) + (trap.transform.forward * 0.75f);
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
	
	private void ApplyLightingState(LightingState _lightingState)
	{
		RenderSettings.ambientLight = _lightingState.ambient_color;
		RenderSettings.fogColor = _lightingState.fog_color;
		RenderSettings.fogDensity = _lightingState.fog_density;
		Camera.allCameras[1].backgroundColor = _lightingState.fog_color;
		sky.renderer.material.SetColor("_Color", _lightingState.sky_color);
		sky.renderer.material.SetFloat("_Factor", _lightingState.sky_factor);
	}
}
