using UnityEngine;
using System.Collections;

public class Auxilliary {

	public static void DrawPoint(Vector3 _point, Color _color)
	{
		Debug.DrawLine(_point + Vector3.right*0.5f, _point - Vector3.right*0.5f, _color);
		Debug.DrawLine(_point + Vector3.forward*0.5f, _point - Vector3.forward*0.5f, _color);
	}
}
