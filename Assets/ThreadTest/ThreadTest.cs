using UnityEngine;
using System.Collections;

public class ThreadTest {

	public volatile bool done = false;
	public volatile string output;
	
	public void ThreadStart()
	{
		Debug.Log ("Thread started");
		System.Threading.Thread.Sleep(3000);
		//Call back when thread is finished
		done = true;
		output = "123456789";
	}

}
