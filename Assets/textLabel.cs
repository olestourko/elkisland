using UnityEngine;
using System.Collections;

public class textLabel : MonoBehaviour {
	
	public Vector3 offset;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		Vector3 screenPoint = Camera.main.WorldToViewportPoint(transform.parent.transform.position);
		this.transform.position = screenPoint + offset;
	}
}
