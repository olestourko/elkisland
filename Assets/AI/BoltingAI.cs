using UnityEngine;
using System.Collections;

public class BoltingAI : MonoBehaviour {
	
	private SteeringBehaviors steering_behaviours;
	public Vector3 target;
	
	//for shader animation
	private float alpha = 0.0f;
	private bool maxxed = false;
	
	void Start () 
	{
		steering_behaviours = new SteeringBehaviors(this.gameObject);
		if(target == null) return;
		Vector3 target_vector = new Vector3(target.x, 0.0f, target.z);
		Vector3 force = steering_behaviours.Seek(target_vector) * 1.25f;
		rigidbody.velocity = force;
	}
	
	void Update()
	{
		if(maxxed) alpha -= Time.deltaTime * 5.0f;
		else alpha += Time.deltaTime * 5.0f;
		if(alpha >= 0.9f) maxxed = true;
		renderer.material.SetFloat("_Alpha", alpha * 2.0f);
		if(alpha < 0.0f) Destroy(this.gameObject);
	}
	
	void FixedUpdate () 
	{
		/*
		if(target == null) return;
		Vector3 target_vector = new Vector3(target.x, 0.0f, target.z);
		Vector3 force = steering_behaviours.Seek(target_vector) * 100.0f;
		rigidbody.AddForce(force * Time.fixedDeltaTime);
		*/
		//if(rigidbody.velocity.magnitude > 200.0f) rigidbody.velocity = Vector3.Normalize(rigidbody.velocity) * 200.0f;
	}
}
