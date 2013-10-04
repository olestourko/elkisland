using UnityEngine;
using System.Collections;

public class Kernel9x9 : Kernel {

	public Kernel9x9()
	{
		size = 9;
		array = new int[9, 9]
		{
			{0, 0, 0, 1, 1, 1, 0, 0, 0},
			{0, 0, 1, 1, 1, 1, 1, 0, 0},
			{0, 1, 1, 1, 1, 1, 1, 1, 0},
			{1, 1, 1, 1, 1, 1, 1, 1, 1},
			{1, 1, 1, 1, 1, 1, 1, 1, 1},
			{1, 1, 1, 1, 1, 1, 1, 1, 1},
			{0, 1, 1, 1, 1, 1, 1, 1, 0},
			{0, 0, 1, 1, 1, 1, 1, 0, 0},
			{0, 0, 0, 1, 1, 1, 0, 0, 0}
		};
	}
}
