using UnityEngine;
using System.Collections;

public class Location : MonoBehaviour {

	public float radius = 1.0f;
	public int n = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	public bool WithinRange(Vector3 _position)
	{
		if((_position - transform.position).magnitude <= radius) return true;
		return false;
	}
}
