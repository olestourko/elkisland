using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {

	public float factor;
	
	public float distance_walked = 0.0f;
	private Vector3 last_position = Vector3.zero;
	
	//Audio
	public List<AudioClip> audioClips;
	private float count = 0.0f;
	
	private AudioClip last_played;
	private float wait_time = 0.0f;
	
	
	void Start () 
	{
		last_played = audioClips[0];
	}
	
	void Update () 
	{
		//Update stats
		distance_walked += (transform.position - last_position).magnitude;
		last_position = transform.position;
		
		
		if(Input.GetKey(KeyCode.W)) 
		{
			transform.position += new Vector3(transform.forward.x, 0.0f, transform.forward.z).normalized * factor * Time.deltaTime;
			PlayWalkSound();
		}
		else if(Input.GetKey(KeyCode.S)) 
		{
			transform.position -= new Vector3(transform.forward.x, 0.0f, transform.forward.z).normalized * factor * Time.deltaTime;
			PlayWalkSound();
		}
		if(Input.GetKey(KeyCode.D)) 
		{
			transform.position += new Vector3(transform.right.x, 0.0f, transform.right.z).normalized * factor * Time.deltaTime;
			PlayWalkSound();
		}
		else if(Input.GetKey(KeyCode.A)) 
		{
			transform.position -= new Vector3(transform.right.x, 0.0f, transform.right.z).normalized * factor * Time.deltaTime;
			PlayWalkSound();
		}
	}
	
	private void PlayWalkSound()
	{
		count += Time.fixedDeltaTime;
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
			if(wait_time > 0.75f) wait_time = 0.75f;
		}
	}
}
