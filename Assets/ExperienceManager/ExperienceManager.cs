using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExperienceManager : MonoBehaviour {
	
	public gui gui;
	public WorldGrid worldGrid;
	public TrapManager trapManager;
	
	public Transform player;
	public StalkingAI stalking_ai_prefab;
	public BoltingAI bolting_ai_prefab;
	
	private List<StalkingAI> stalkingAIs = new List<StalkingAI>();
	
	//Lighting
	private LightingState lighting_current;
	private LightingState lighting_blended;
	private LightingState lighting_to;
	
	private LightingState lighting_0;
	private LightingState lighting_1;
	private LightingState lighting_2;
	private LightingState lighting_3;
	private LightingState lighting_4;
	private float count = 0.0f;
	private bool done_tweening = true;
	private MeshChunk.ChunkType last_tweened_to;
	public GameObject sky;
	
	private GameObject forest_sounds;
	private GameObject plain_sounds;
	
	//for AI spawning on interval (to be moved)
	private float count_2 = 0.0f;
	public bool development_mode;
	
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
		//completely visible (debug)
		lighting_3 = new LightingState(
			new Color(1.0f, 1.0f, 1.0f),
			new Color(1.0f, 1.0f, 1.0f),
			new Color(1.0f, 1.0f, 1.0f),
			0.0f
		);
		//yellow / dusk
		lighting_4 = new LightingState(
			new Color(0.549f, 0.196f, 0.294f),
			new Color(0.549f, 0.196f, 0.0f),
			new Color(0.549f, 0.196f, 0.0f),
			0.15f
		);
		
		lighting_2.sky_factor = 0.2f;
		
		lighting_current = lighting_0;
		if(development_mode) TweenLightingState(lighting_3);
		else TweenLightingState(lighting_1);
		
		//Get audio sources
		forest_sounds = transform.Find("ForestSounds").gameObject;
		plain_sounds = transform.Find("PlainSounds").gameObject;
	}
	
	// Update is called once per frame
	void Update () 
	{
		//Blend lighting
		if(count <= 1.0f)
		{
			lighting_blended = lighting_current.Blend(lighting_to, count);
			ApplyLightingState(lighting_blended);
			count += Time.deltaTime * 0.25f;
		} 
		else 
		{
			lighting_current = lighting_blended;
			done_tweening = true;
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
			
		}
		else
		{
			
		}
		*/
		
		//Keep sky moving with player
		Vector3 sky_offset = Vector3.forward * 10.0f;
		sky.transform.position = player.transform.position + sky_offset;
		
		
		
		//Create spawn points and targets for shadow ghosts
		List<Vector3> positions = new List<Vector3>();
		foreach(MeshChunk chunk in worldGrid.visible_region.GetChunks())
		{
			foreach(Vector3 position in chunk.GetRandomObjectPositions())
			{
				float distance = Vector3.Distance(player.transform.position, position);
				float min = 6;
				float max = 8;
				if(distance > min && distance < max) 
				{
					positions.Add(position);
					Debug.DrawLine(position + Vector3.right*0.5f, position - Vector3.right*0.5f, Color.green);
					Debug.DrawLine(position + Vector3.forward*0.5f, position - Vector3.forward*0.5f, Color.green);
					//Debug.DrawLine(player.transform.position, position, Color.green);	
				}
			}
		}
		//Spawn shadow ghost every 1 second using random position
		count_2 += Time.deltaTime;
		/*
		if(count_2 > 1.0f) 
		{
			count_2 = 0.0f;
			BoltingAI ai = Instantiate(bolting_ai_prefab) as BoltingAI;
			ai.transform.position = positions[Random.Range(0, positions.Count-1)] + (Vector3.up * 0.11f);
			ai.target = positions[Random.Range(0, positions.Count-1)];
		}	
		*/
		
		//Set lighting based on distance to path
		gui.distance_to_path = worldGrid.closest_distance;
		if(count >= 1.0f)
		{
			float factor = worldGrid.closest_distance / 4.0f;
			LightingState lighting_blended = lighting_1.Blend(lighting_2, factor);
			//ApplyLightingState(lighting_blended);
		}
		//Set lighting based on the type of chunk the player is on
		//Debug.Log (done_tweening + " | " + last_tweened_to);
		switch(worldGrid.GetChunkAt(player.transform.position).chunkType)
		{
		case MeshChunk.ChunkType.Forest:
			if(!done_tweening) break;
			if(last_tweened_to != MeshChunk.ChunkType.Forest)
			{
				last_tweened_to = MeshChunk.ChunkType.Forest;
				TweenLightingState(lighting_1);
				plain_sounds.audio.volume = 0.0f;
				forest_sounds.audio.volume = 1.0f;
			}
			break;
		case MeshChunk.ChunkType.Plain:
			if(!done_tweening) break;
			if(last_tweened_to != MeshChunk.ChunkType.Plain)
			{
				last_tweened_to = MeshChunk.ChunkType.Plain;
				TweenLightingState(lighting_4);
				plain_sounds.audio.volume = 1.0f;
				forest_sounds.audio.volume = 0.0f;
			}
			break;
		}
		
		//audio
		forest_sounds.transform.position = player.transform.position;
		plain_sounds.transform.position = player.transform.position;
	}
	
	//Lighting
	public void ChangeLighting(int _id)
	{
		switch(_id)
		{
		case 0:
			TweenLightingState(lighting_0);
			break;
		case 1:
			TweenLightingState(lighting_1);
			break;
		case 2:
			TweenLightingState(lighting_2);
			break;
		}
	}
	
	private void TweenLightingState(LightingState _lightingState)
	{
		done_tweening = false;
		count = 0.0f;	
		lighting_to = _lightingState;
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
