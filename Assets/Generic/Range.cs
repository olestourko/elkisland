using UnityEngine;
using System.Collections;

public class Range {

	private float start;
	private float end;
	
	public Range(float _start, float _end)
	{
		if(start > end)
		{
			start = _end;
			end = _start;
		}
		else
		{
			start = _start;
			end = _end;
		}
	}
	
	public bool WithinRange(float _n)
	{
		if(_n >= start && _n <= end) return true;
		return false;
	}
	
	public override string ToString ()
	{
		return start + " to " + end;
	}
}
