using UnityEngine;
using System.Collections;

public abstract class Kernel {

	protected int size;
	protected int[,] array;
	
	public Kernel()
	{
		
	}
	
	public int GetSize() { return size; }
	public int[,] GetArray() { return array; }
	public int GetCenter()
	{
		return Mathf.FloorToInt(size / 2) + 1;	
	}
	public Object[,] GetSubarray(Object[,] _in, int _i, int _j)
	{
		Debug.Log (_in.Length);
		return null;
	}
}
