using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	public float distance_walked = 0.0f;
	private Vector3 last_position = Vector3.zero;
	
	void Start () {}
	
	void Update () 
	{
		distance_walked += (transform.position - last_position).magnitude;
		last_position = transform.position;
	}
}
