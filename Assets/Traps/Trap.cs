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
		Debug.DrawRay(transform.position, transform.forward, Color.green);
	}
	
	//Fill in in subclasses
	public virtual void Activate()
	{

	}
}
