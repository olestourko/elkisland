using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Schema_MainPath : Schema {

	private Path path;
	//Restriction: all points must unique
	private List<int> branch_points_open = new List<int>();
	private List<int> branch_points_closed = new List<int>();


	public Schema_MainPath(Path _path)
	{
		path = _path;
		branch_points_open.Add(10);
		branch_points_open.Add(25);
		branch_points_open.Add(50);
	}

	//Returns a list of branch points (path cell indices) that need to be generated
	public List<int> Run()
	{
		List<int> todo = new List<int>();
		for(int i = 0; i < branch_points_open.Count; i++)
		{
			int point = branch_points_open[i];
			if(point < path.getLength())
			{
				i--;
				branch_points_open.Remove(point);
				branch_points_closed.Add(point);
				todo.Add(point);
			}
		}
		return todo;
	}
}
