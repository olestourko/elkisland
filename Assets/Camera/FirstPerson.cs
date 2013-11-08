using UnityEngine;
using System.Collections;

public class FirstPerson : MonoBehaviour {
		
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
}
