using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FirstPerson : MonoBehaviour {
	
	public float factor = 0.025f;
	
	public List<AudioClip> audioClips;
	private float count = 0.0f;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void FixedUpdate()
	{
		if(Input.GetKey(KeyCode.W)) 
		{
			transform.position += transform.forward * factor;
			PlayWalkSound();
		}
		else if(Input.GetKey(KeyCode.S)) 
		{
			transform.position += -transform.forward * factor;
			PlayWalkSound();
		}
		count += Time.fixedDeltaTime;	
		if(Input.GetKey(KeyCode.Q)) transform.Rotate(-Vector3.up * 2.0f);
		else if(Input.GetKey(KeyCode.E)) transform.Rotate(Vector3.up * 2.0f);
	}
	
	private void PlayWalkSound()
	{
		count += Time.fixedDeltaTime;
		if(count > 1.0f) 
		{
			int index = Random.Range(0, audioClips.Count-1);
			audio.PlayOneShot(audioClips[index]);
			count = 0.0f;
		}
	}
}
