using UnityEngine;
using System.Collections;

public class SteeringBehaviors {
	
	public GameObject owner;
	
	public SteeringBehaviors(GameObject _owner)
	{
		owner = _owner;	
	}
	
	public Vector3 Seek(Vector3 _target)
	{
		Vector3 target_local = owner.transform.InverseTransformPoint(_target);
		Vector3 desired_velocity = Vector3.Normalize(target_local);
		return desired_velocity - owner.rigidbody.velocity;
	}

}
