using UnityEngine;
using System.Collections;

public class BoltingAI : MonoBehaviour {
	
	private SteeringBehaviors steering_behaviours;
	public Vector3 target;
	
	//for shader animation
	private float ttyl = 2.0f;
	private float count = 0.0f;
	private bool maxxed = false;
	
	void Start () 
	{
		float speed = 2.0f;
		steering_behaviours = new SteeringBehaviors(this.gameObject);
		Vector3 target_vector = new Vector3(target.x, 0.0f, target.z);
		Vector3 force = steering_behaviours.Seek(target_vector) * speed;
		rigidbody.velocity = force;
		ttyl = Vector3.Distance(target, this.transform.position) / speed * 2.0f;
	}
	
	void Update()
	{
		if(maxxed) count -= Time.deltaTime;
		else count += Time.deltaTime;
		if(count >= ttyl / 2.0f) maxxed = true;
		
		//set alpha
		float alpha = count / ttyl * 2.0f;
		
		renderer.material.SetFloat("_Alpha", alpha);
		audio.volume = alpha;
		if(count < 0.0f) 
		{
			Destroy(this.gameObject);
		}
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
