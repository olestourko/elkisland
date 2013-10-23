using UnityEngine;
using System.Collections;

public class BaseAI : MonoBehaviour {
	
	State current_state;
	
	public Transform target;
	
	
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
	
	public void Halt()
	{
		rigidbody.velocity = Vector3.zero;	
	}
	
	public void ApproachTarget()
	{
		SteeringBehaviors steering_behaviours = new SteeringBehaviors(this.gameObject);
		Vector3 target_vector = new Vector3(target.position.x, 0.0f, target.position.z);
		Vector3 force = steering_behaviours.Seek(target_vector) * 20.0f;
		force.y = 0.0f; //cut off the y-axis
		rigidbody.AddForce(force * Time.fixedDeltaTime);
		if(rigidbody.velocity.magnitude > 0.5f) rigidbody.velocity = Vector3.Normalize(rigidbody.velocity) * 0.5f;
	}
	
	public bool TargetCanSeeMe()
	{
		Vector3 my_local_position = target.InverseTransformPoint(this.transform.position);
		if(my_local_position.z > 0.0f) return true;
		return false;
	}
	
}
