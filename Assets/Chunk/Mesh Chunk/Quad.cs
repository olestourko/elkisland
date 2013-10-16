using UnityEngine;
using System.Collections;

public class Quad {

	public Triangle triangle_1;
	public Triangle triangle_2;
	
	public Vector3 vertex_1;
	public Vector3 vertex_2;
	public Vector3 vertex_3;
	public Vector3 vertex_4;
	
	public Quad(Vector3 _vertex_1, Vector3 _vertex_2, Vector3 _vertex_3, Vector3 _vertex_4)
	{
		vertex_1 = _vertex_1;
		vertex_2 = _vertex_2;
		vertex_3 = _vertex_3;
		vertex_4 = _vertex_4;
		triangle_1 = new Triangle(vertex_1, vertex_2, vertex_3);
		triangle_2 = new Triangle(vertex_3, vertex_1, vertex_4);
	}
}
