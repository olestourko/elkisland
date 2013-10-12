using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StalkingAI : MonoBehaviour {
	
	private SteeringBehaviors steering_behaviours;
	public Transform target;
	
	public enum AIState
	{
		Idle,
		Follow,
		Approach
	}
	public AIState aiState = AIState.Idle;
	
	//Audio
	public List<AudioClip> audioClips;
	private float count = 0.0f;
	
	public WorldGrid worldGrid;
	public float distance_travelled = 0.0f;
	
	// Use this for initialization
	void Start () 
	{
		steering_behaviours = new SteeringBehaviors(this.gameObject);
	}
		
	void FixedUpdate()
	{
		switch (aiState)
		{
		case AIState.Idle:
			Idle();
			break;
		case AIState.Follow:
			Follow();
			break;
		case AIState.Approach:
			Approach();
			break;
		}
	}
	
	public bool TargetCanSeeMe()
	{
		Vector3 my_local_position = target.transform.InverseTransformPoint(this.transform.position);
		if(my_local_position.z > 0.0f) return true;
		return false;
	}
	
	private void Idle()
	{
		
	}
	
	private void Follow()
	{
		if(TargetCanSeeMe()) 
		{	
			rigidbody.velocity = Vector3.zero;
			return;
		}
		
		
		
		float volume = rigidbody.velocity.magnitude / 2.0f;
		if(volume < 0.1f) volume = 0.0f;
		audio.volume = volume;
		float l = 1.0f/rigidbody.velocity.magnitude * 0.15f;
		count += Time.fixedDeltaTime;
		if(count > l) 
		{
			int index = Random.Range(0, audioClips.Count-1);
			audio.PlayOneShot(audioClips[index]);
			count = 0.0f;
		}
				
		Cell cell = worldGrid.GetCellAt(target.position);
		//cell.Select();
		
		
		List<Cell.CellType> valid_cell_types = new List<Cell.CellType>();
		valid_cell_types.Add(Cell.CellType.Path);
		valid_cell_types.Add(Cell.CellType.Path_Start);
		valid_cell_types.Add(Cell.CellType.Path_End);	
		
		Vector3 seek_target = target.position;
		//Target not on path
		if(!valid_cell_types.Contains(cell.cellType))
		{
			
		}
		//Target on path
		else
		{
			Cell cell_target = null;
			foreach(Cell cell_adjacent in cell.getNeighbors())
			{
				if(!valid_cell_types.Contains(cell_adjacent.cellType))
				{
					cell_target = cell_adjacent;
				}
			}
			if(cell_target != null) seek_target = cell_target.cell_GameObject.transform.position;
			else rigidbody.velocity = Vector3.zero;	
		}
		
		Vector3 target_vector = new Vector3(seek_target.x, 0.0f, seek_target.z);
		Vector3 force = steering_behaviours.Seek(target_vector) * 20.0f;
		rigidbody.AddForce(force * Time.fixedDeltaTime);
		if(rigidbody.velocity.magnitude > 0.5f) rigidbody.velocity = Vector3.Normalize(rigidbody.velocity) * 0.5f;
		distance_travelled += rigidbody.velocity.magnitude * Time.fixedDeltaTime;
	}
	
	public void Approach()
	{
		
	}
}
