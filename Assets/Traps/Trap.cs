using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Trap : MonoBehaviour {
	
	public List<AudioClip> audioClips;
	
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void Activate()
	{
		int index = Random.Range(0, audioClips.Count-1);
		audio.PlayOneShot(audioClips[index]);
	}
}
