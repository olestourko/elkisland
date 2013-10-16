using UnityEngine;
using System.Collections;

public class WeightedQuad : Quad {
	
	//Vertex weights which can be used for auxiliary purposes
	public float vertex_1_weight = 1.0f;
	public float vertex_2_weight = 1.0f;
	public float vertex_3_weight = 1.0f;
	public float vertex_4_weight = 1.0f;
	
	public WeightedQuad(Vector3 _vertex_1, Vector3 _vertex_2, Vector3 _vertex_3, Vector3 _vertex_4)
	: base(_vertex_1, _vertex_2, _vertex_3, _vertex_4)
	{
		
	}
}
