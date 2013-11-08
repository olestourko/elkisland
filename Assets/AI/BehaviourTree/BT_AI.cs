using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BT_AI : MonoBehaviour {
	
	public Transform target;
	public List<Vector3> obstacles;
	public float min_range;
	public float max_range;
	
	public BT_Node bt;
	private BT_Stalker_Blackboard blackboard;
	
	/*Audio*/
	public List<AudioClip> audioClips;
	private float count = 0.0f;
	private AudioClip last_played;
	private float wait_time = 0.0f;
	
	
	// Use this for initialization
	void Start () 
	{
		renderer.material.SetFloat("_Alpha", 1.0f);
		blackboard = new BT_Stalker_Blackboard();
		blackboard.ai = this;
		blackboard.obstacles = this.obstacles;
		blackboard.min_range = 2.0f;
		blackboard.max_range = 4.0f;
				
		bt = new BT_Selector(blackboard);
		
		BT_Timeout t0 = new BT_Timeout(4.0f);
		BT_Sequence seq0 = new BT_Sequence();
			BT_Stalker_CanSeeMe c0 = new BT_Stalker_CanSeeMe();
			BT_Stalker_C_FullHide a0 = new BT_Stalker_C_FullHide();
			BT_SetValue a1 = new BT_SetValue("hidding", 0);
		BT_Sequence seq1 = new BT_Sequence();
			BT_GetValue c1 = new BT_GetValue("hidding");
			BT_Stalker_C_FullHide a2 = new BT_Stalker_C_FullHide();
			BT_Selector sel0 = new BT_Selector();
				BT_Sequence seq2 = new BT_Sequence();
					//timeout
					BT_SetValue a3 = new BT_SetValue("hidding", -1);
				BT_SetValue a4 = new BT_SetValue("hidding", 0);	
		
		
		BT_Stalker_Follow a5 = new BT_Stalker_Follow();
		
		
		
		bt.AddChild(seq0);
			seq0.AddChild(c0);
			seq0.AddChild(a0);
			seq0.AddChild(a1);
			seq0.AddChild(t0);
		bt.AddChild(seq1);
			seq1.AddChild(c1);
			seq1.AddChild(a2);
			seq1.AddChild(sel0);
				sel0.AddChild(seq2);
					seq2.AddChild(t0);
					seq2.AddChild(a3);
				sel0.AddChild(a4);
		bt.AddChild(a5);	
		/*
		BT_SetValue a0 = new BT_SetValue("hidden", 0);
		BT_GetValue c0 = new BT_GetValue("hidden");
		bt.AddChild(c0);
		bt.AddChild(new BT_Stalker_Follow());
		*/
	}
	
	// Update is called once per frame
	void Update () 
	{
		bt.Execute();
		blackboard.elapsed_time += Time.deltaTime;
		blackboard.min_range = min_range;
		blackboard.max_range = max_range;
		
		/*Audio*/
		count += Time.deltaTime;
		if(rigidbody.velocity.magnitude >= 0.1f) PlayWalkSound();
		//blackboard.min_range -= Time.deltaTime * 0.1f;
		//blackboard.max_range -= Time.deltaTime * 0.1f;
	}
	
	
	//--------------------------------------------------------------------------------------------
	//---	Basic AI actions
	//--------------------------------------------------------------------------------------------
	public AudioClip attack_sound;
	public float max_speed = 0.5f;
	public float attack_range = 1.5f;
	
	public void Halt()
	{
		rigidbody.velocity = Vector3.zero;	
	}
	
	public void ApproachTarget(Vector3 _target)
	{
		SteeringBehaviors steering_behaviours = new SteeringBehaviors(this.gameObject);
		Vector3 target_vector = new Vector3(_target.x, 0, _target.z);
		Vector3 force = steering_behaviours.Seek(target_vector) * 400.0f * max_speed;
		force.y = 0.0f; //cut off the y-axis
		rigidbody.AddForce(force * Time.fixedDeltaTime);
		if(rigidbody.velocity.magnitude > max_speed) rigidbody.velocity = Vector3.Normalize(rigidbody.velocity) * max_speed;
	}
	
	public bool TargetCanSeeMe()
	{
		//return renderer.isVisible;
		float x = Camera.main.WorldToViewportPoint(transform.position).x;
		//Debug.Log(x);
		Vector3 my_local_position = target.InverseTransformPoint(this.transform.position);
		if(x > 0.0f && x < 1.0f) 
		{
			if(my_local_position.z >= 0.0f) return true;
		}
		return false;
	}
	
	
	private void PlayWalkSound()
	{
		if(count > wait_time) 
		{
			List<AudioClip> valid_clips = new List<AudioClip>();
			foreach(AudioClip clip in audioClips) valid_clips.Add(clip);
			valid_clips.Remove(last_played);
			
			int index = Random.Range(0, valid_clips.Count);
			audio.PlayOneShot(valid_clips[index]);
			count = 0.0f;
			last_played = valid_clips[index];
			wait_time = last_played.length + 0.8f;
			if(wait_time > 0.4f) wait_time = 0.4f;
		}
	}
}
