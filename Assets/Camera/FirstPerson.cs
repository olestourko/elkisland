using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FirstPerson : MonoBehaviour {
	
	public float factor = 0.025f;
	
	public List<AudioClip> audioClips;
	private float count = 0.0f;

	// Use this for initialization
	void Start () 
	{
		Screen.lockCursor = true;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetKeyDown(KeyCode.Q)) Screen.lockCursor = !Screen.lockCursor;
		if(Screen.lockCursor)
		{
			float delta_x = Input.GetAxis("Mouse X");
			float delta_y = Input.GetAxis("Mouse Y");
			transform.Rotate(Vector3.up * delta_x * 50.0f * Time.deltaTime, Space.World);
			transform.Rotate(transform.right * -delta_y * 50.0f * Time.deltaTime, Space.World);
		}
	}
	
	void FixedUpdate()
	{	
		if(Input.GetKey(KeyCode.W)) 
		{
			transform.position += new Vector3(transform.forward.x, 0.0f, transform.forward.z).normalized * factor;
			PlayWalkSound();
		}
		else if(Input.GetKey(KeyCode.S)) 
		{
			transform.position -= new Vector3(transform.forward.x, 0.0f, transform.forward.z).normalized * factor;
			PlayWalkSound();
		}
		if(Input.GetKey(KeyCode.D)) 
		{
			transform.position += new Vector3(transform.right.x, 0.0f, transform.right.z).normalized * factor;
			PlayWalkSound();
		}
		else if(Input.GetKey(KeyCode.A)) 
		{
			transform.position -= new Vector3(transform.right.x, 0.0f, transform.right.z).normalized * factor;
			PlayWalkSound();
		}
		
		count += Time.fixedDeltaTime;	
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
