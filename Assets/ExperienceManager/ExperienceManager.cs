using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExperienceManager : MonoBehaviour {
	
	public gui gui;
	public WorldGrid worldGrid;
	public TrapManager trapManager;
	
	
	public Transform player;
	public Player player_script;
	public BT_AI stalking_ai_prefab;
	private List<BT_AI> AIs = new List<BT_AI>();
	
	//Lighting
	private LightingState lighting_current;
	private LightingState lighting_blended;
	private LightingState lighting_to;
	
	private LightingState lighting_0;
	private LightingState lighting_1;
	private LightingState lighting_2;
	private LightingState lighting_3;
	private LightingState lighting_4;
	private MeshChunk.ChunkType last_tweened_to;
	public GameObject sky;
	
	private GameObject forest_sounds;
	private GameObject plain_sounds;
	
	//for AI spawning on interval (to be moved)
	private float count_2 = 0.0f;
	public bool development_mode;

	//for exploration
	public Exploration_State exploration_state;
	public Exploration_State exploration_state_prev = null;
	public Location last_visited_location = null;	
	public List<Location> locations_visited = new List<Location>();

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
			//0.2f
			0.025f
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
		
		
		Lighting.sky = sky;
		Lighting.previous = lighting_0;
		Lighting.current = lighting_0;
		Lighting.target = lighting_0;
		Lighting.modifier = lighting_2;
		Lighting.state = new Lighting_State_Idle();
		Lighting.SetTarget(lighting_1);
		
		if(development_mode) Lighting.SetTarget(lighting_3);
		
		//Get audio sources
		forest_sounds = GameObject.Find("ForestSounds").gameObject;
		plain_sounds = GameObject.Find("PlainSounds").gameObject;

		//Exploration states
		ChangeState(new Exploration_State_L0());
	}
	
	// Update is called once per frame
	void Update () 
	{
		Lighting.Update();
		gui.distance_travelled = player_script.distance_walked;
				
		//Generate traps
		if(worldGrid.ready)
		{
			worldGrid.ready = false;
			trapManager.GenerateTraps(worldGrid.GetAllModelPositions());
		}
		
		//Activate traps and instantiate an AI
		List<Trap> activated_traps = trapManager.ActivateNearbyTraps(player.position, 2.5f);
		foreach(Trap trap in activated_traps)
		{
			trapManager.DestroyTrap(trap);
			
			//spawn stalker AI if audiotrap
			if(trap as AudioTrap != null)
			{
				if(AIs.Count >= 1) break;
				SpawnAI();
			}
		}
		
		//Keep sky moving with player
		Vector3 sky_offset = Vector3.forward * 10.0f;
		sky.transform.position = player.transform.position + sky_offset;
		
		//Create spawn points and targets for shadow ghosts
		GetSpawnPointsInRange(8.0f, 10.0f);
		
		//Spawn shadow ghost every 1 second using random position
		count_2 += Time.deltaTime;
				
		//Modify lighting based on distance to path
		float d = worldGrid.closest_distance;
		gui.distance_to_path = d;
		
		d = d * 0.5f;
		d -= 0.25f;
		if(d > 0.9f) d = 0.9f;
		
		Lighting.f = d;
		if(last_tweened_to != MeshChunk.ChunkType.Forest) Lighting.f = 0.0f;

		foreach(BT_AI ai in AIs)
		{
			ai.min_range = d;
			ai.max_range = d + 2.0f;
		}
		
		
		//Set lighting based on the type of chunk the player is on
		switch(worldGrid.GetChunkAt(player.transform.position).chunkType)
		{
		case MeshChunk.ChunkType.Forest:
			if(last_tweened_to != MeshChunk.ChunkType.Forest)
			{
				last_tweened_to = MeshChunk.ChunkType.Forest;
				Lighting.SetTarget(lighting_1);
				plain_sounds.audio.volume = 0.0f;
				forest_sounds.audio.volume = 1.0f;
			}
			break;

		case MeshChunk.ChunkType.Plain:
			if(last_tweened_to != MeshChunk.ChunkType.Plain)
			{
				last_tweened_to = MeshChunk.ChunkType.Plain;
				Lighting.SetTarget(lighting_4);
				plain_sounds.audio.volume = 1.0f;
				forest_sounds.audio.volume = 0.0f;
			}
			break;
		}
		

		//Location triggers
		bool r = false;
		foreach(Path_Location_Pair pair in worldGrid.pairs)
		{
			if(pair.location == null) continue;
			if(pair.location.WithinRange(player.transform.position)) 
			{
				r = true;
				last_visited_location = pair.location;
				exploration_state.Execute(this);
				exploration_state_prev = exploration_state;

				if(!locations_visited.Contains(pair.location)) 
				{
					locations_visited.Add(pair.location);
				}
			}
		}
		gui.render_description = r;
	}
	
	public void SpawnAI()
	{
		BT_AI ai = Instantiate(stalking_ai_prefab) as BT_AI;
		List<Vector3> spawn_points = GetSpawnPointsInRange(8.0f, 10.0f);
		int index = Random.Range(0, spawn_points.Count);
		Vector3 new_position = new Vector3(spawn_points[index].x, ai.transform.position.y, spawn_points[index].z);
		ai.transform.position = new_position;	
		ai.obstacles = worldGrid.GetAllModelPositions();
		ai.target = player;
		AIs.Add(ai);
	}

	//Gets a lost of spawn points from the worldgrid with a range
	public List<Vector3> GetSpawnPointsInRange(float _min, float _max)
	{
		List<Vector3> positions = new List<Vector3>();
		//foreach(MeshChunk chunk in worldGrid.visible_region.GetChunks())
		foreach(MeshChunk chunk in worldGrid.chunks_list)
		{
			foreach(Vector3 position in chunk.GetRandomObjectPositions())
			{
				float distance = Vector3.Distance(player.transform.position, position);
				if(distance > _min && distance < _max) 
				{
					positions.Add(position);
					//Auxilliary.DrawPoint(position, Color.gray);
				}
			}
		}
		return positions;
	}

	public void ChangeState(State _state)
	{
		exploration_state_prev = exploration_state;
		if(exploration_state != null) exploration_state.Exit(this);
		exploration_state = _state as Exploration_State;
		exploration_state.Enter(this);
	}

}
