using UnityEngine;
using System.Collections;

public class LightingState {
	
	public Color ambient_color;
	public Color fog_color;
	public Color sky_color;
	public float sky_factor = 1.0f;
	public float fog_density;
		
	public LightingState()
	{

	}
	
	public LightingState(Color _ambient_color, Color _fog_color, Color _sky_color, float _fog_density)
	{
		ambient_color = _ambient_color;
		fog_color = _fog_color;
		sky_color = _sky_color;
		fog_density = _fog_density;
	}
		
	//blender another lighting state
	public LightingState Blend(LightingState _other, float _factor)
	{
		_factor = Mathf.Clamp01(_factor);
		float a = 1.0f - _factor;
		float b = _factor;
		
		LightingState lightingState_out = new LightingState();
		lightingState_out.ambient_color = (this.ambient_color * a) + (_other.ambient_color * b);
		lightingState_out.fog_color = (this.fog_color * a) + (_other.fog_color * b);
		lightingState_out.sky_factor = (this.sky_factor * a) + (_other.sky_factor * b);
		lightingState_out.sky_color = (this.sky_color * a) + (_other.sky_color * b);
		lightingState_out.fog_density = (this.fog_density * a) + (_other.fog_density * b);
		return lightingState_out;
	}
}
