using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BaseAI : MonoBehaviour {
	
	State current_state;
	public Transform target;
	public List<Vector3> obstacles;
		
	// Use this for initialization
	void Start () 
	{
		current_state = new State_Approach();
		renderer.material.SetFloat("_Alpha", 1.0f);
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(current_state == null) return;
		current_state.Execute(this);
	}
	
	public void ChangeState(State _new_state)
	{
		this.current_state.Exit(this);
		this.current_state = _new_state;
		this.current_state.Enter(this);
	}
	
	//--------------------------------------------------------------------------------------------
	//---	Move to subclass when refactoring
	//--------------------------------------------------------------------------------------------
	public AudioClip attack_sound;
	public float max_speed = 0.5f;
	public float attack_range = 1.5f;
	
	public void Halt()
	{
		rigidbody.velocity = Vector3.zero;	
	}
	
	public void ApproachTarget(Vector3 _target)
	{
		SteeringBehaviors steering_behaviours = new SteeringBehaviors(this.gameObject);
		Vector3 target_vector = new Vector3(_target.x, 0, _target.z);
		Vector3 force = steering_behaviours.Seek(target_vector) * 100.0f * max_speed;
		force.y = 0.0f; //cut off the y-axis
		rigidbody.AddForce(force * Time.fixedDeltaTime);
		if(rigidbody.velocity.magnitude > max_speed) rigidbody.velocity = Vector3.Normalize(rigidbody.velocity) * max_speed;
	}
	
	public bool TargetCanSeeMe()
	{
		//return renderer.isVisible;
		float x = Camera.main.WorldToViewportPoint(transform.position).x;
		//Debug.Log(x);
		Vector3 my_local_position = target.InverseTransformPoint(this.transform.position);
		if(x > 0.0f && x < 1.0f) 
		{
			if(my_local_position.z > 1.0f) return true;
		}
		return false;
	}
	
	public void PlayAttackSound()
	{
		audio.PlayOneShot(attack_sound);	
	}
}
