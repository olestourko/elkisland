using UnityEngine;
using System.Collections;

public class Kernel5x5 : Kernel {

	public Kernel5x5()
	{
		size = 5;
		array = new int[5, 5]
		{
			{0, 0, 1, 0, 0},
			{0, 1, 1, 1, 0},
			{1, 1, 1, 1, 1},
			{0, 1, 1, 1, 0},
			{0, 0, 1, 0, 0}
		};
	}
}
