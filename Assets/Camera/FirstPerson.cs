using UnityEngine;
using System.Collections;

public class FirstPerson : MonoBehaviour {
	
	public float factor = 0.025f;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void FixedUpdate()
	{
		if(Input.GetKey(KeyCode.W)) transform.position += transform.forward * factor;
		else if(Input.GetKey(KeyCode.S)) transform.position += -transform.forward * factor;
		
		if(Input.GetKey(KeyCode.Q)) transform.Rotate(-Vector3.up * 2.0f);
		else if(Input.GetKey(KeyCode.E)) transform.Rotate(Vector3.up * 2.0f);
	}
}
