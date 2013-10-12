using UnityEngine;
using System.Collections;

public class BoltingAI : MonoBehaviour {
	
	private SteeringBehaviors steering_behaviours;
	public Vector3 target;
	
	void Start () 
	{
		steering_behaviours = new SteeringBehaviors(this.gameObject);
		if(target == null) return;
		Vector3 target_vector = new Vector3(target.x, 0.0f, target.z);
		Vector3 force = steering_behaviours.Seek(target_vector) * 5.0f;
		rigidbody.velocity = force;
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
