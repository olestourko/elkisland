using UnityEngine;
using System.Collections;

public class Lighting {
	
	public static GameObject sky;
	
	public static LightingState target;
	public static LightingState previous;
	public static LightingState current;
	
	public static LightingState modifier;
	public static float f = 0.0f;
	
	
	public static State state;
	
	
	
	public static void SetTarget(LightingState _lightingState)
	{
		target = _lightingState;
		ChangeState(new Lighting_State_Blending());
	}
	
	public static void Update()
	{
		if(state != null) state.Execute();
		
		//Apply factor
		LightingState absolute = current.Blend(modifier, f);
		Apply(absolute);
	}
	
	public static void Apply(LightingState _lightingState)
	{
		RenderSettings.ambientLight = _lightingState.ambient_color;
		RenderSettings.fogColor = _lightingState.fog_color;
		RenderSettings.fogDensity = _lightingState.fog_density;
		Camera.allCameras[0].backgroundColor = _lightingState.fog_color;
		sky.renderer.material.SetColor("_Color", _lightingState.sky_color);
		sky.renderer.material.SetFloat("_Factor", _lightingState.sky_factor);
	}
		
	//For FSM
	public static void ChangeState(State _state)
	{
		state.Exit();
		state = _state;
		state.Enter();
	}
	
}
