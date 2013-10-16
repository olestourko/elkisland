using UnityEngine;
using System.Collections;

public class Triangle {

	public Vector3 vertex_1;
	public Vector3 vertex_2;
	public Vector3 vertex_3;
	
	public Triangle() {}
	public Triangle(Vector3 _vertex_1, Vector3 _vertex_2, Vector3 _vertex_3)
	{
		vertex_1 = _vertex_1;
		vertex_2 = _vertex_2;
		vertex_3 = _vertex_3;
	}
}
