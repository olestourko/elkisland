using UnityEngine;
using System.Collections;

public class Topdown : MonoBehaviour {
	
	public float factor = 0.5f;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void FixedUpdate()
	{
		if(Input.GetKey(KeyCode.UpArrow)) transform.position += Vector3.forward * factor;
		else if(Input.GetKey(KeyCode.DownArrow)) transform.position += -Vector3.forward * factor;
		if(Input.GetKey(KeyCode.LeftArrow)) transform.position += -Vector3.right * factor;
		else if(Input.GetKey(KeyCode.RightArrow)) transform.position += Vector3.right * factor;
		
		if(Input.GetKey(KeyCode.PageUp) && transform.position.y >= 1.0f) transform.position -= Vector3.up * factor;
		else if(Input.GetKey(KeyCode.PageDown) && transform.position.y <= 60.0f) transform.position += Vector3.up * factor;
	}
}
