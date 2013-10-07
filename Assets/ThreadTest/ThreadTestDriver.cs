using UnityEngine;
using System.Collections;
using System.Threading;

public class ThreadTestDriver : MonoBehaviour {
	
	Thread thread;
	ThreadTest threadTest;
	
	// Use this for initialization
	void Start () {
		//Thread test
		threadTest = new ThreadTest();
		thread = new Thread(new ThreadStart(threadTest.ThreadStart));
		thread.Start();
		if(thread.IsAlive) Debug.Log ("Thread still running");
		else Debug.Log ("Thread finished");
	}
	
	// Update is called once per frame
	void Update () {
		if(threadTest.done) 
		{
			Debug.Log ("Thread done " + threadTest.output);
			threadTest.done = false;
		}
	}
}
