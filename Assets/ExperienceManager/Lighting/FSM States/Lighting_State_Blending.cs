using UnityEngine;
using System.Collections;

public class Lighting_State_Blending : State {
	
	public float start = 0.0f;
	public float end = 2.0f;
	public float current = 0.0f;
	
	//normalize start and end to 0, 1. f is used to scale deltaTime accordingly.
	private float f;
	
	public override void Enter() 
	{
		f = 1.0f / end;
		Lighting.previous = Lighting.current;
	}
	public override void Execute() 
	{
		this.current += Time.deltaTime * f;
		LightingState current = Lighting.previous.Blend(Lighting.target, this.current);
		Lighting.current = current;
		Lighting.Apply(current);
		if(this.current > 1.0f)
		{
			Lighting.ChangeState(new Lighting_State_Idle());	
		}
	}
	public override void Exit()
	{
		
	}
}
