using UnityEngine;
using System.Collections;


public class gui : MonoBehaviour {
	
	private string pre = "> ";
	public string terminalString = "";
	
	private WorldGrid worldGrid;
	
	void Start()
	{
		worldGrid = GameObject.Find("WorldGrid").GetComponent("WorldGrid") as WorldGrid;
	}
	
	void OnGUI() 
	{
		if(Event.current.keyCode == KeyCode.Return && Event.current.type == EventType.KeyDown)
		{
			processInput();
			terminalString = "";
		}
		GUI.SetNextControlName("terminal");
		terminalString = GUI.TextField (new Rect (10, 10, 200, 22), pre + terminalString, 25);
		if(terminalString.Length > 1) terminalString = terminalString.Substring(2, terminalString.Length - 2);
	}
	
	private void processInput()
	{
		if(terminalString.Length >= 5)
		{
			//Space after command isn't taken into account
			string content = terminalString.Substring(5, terminalString.Length - 5);
			//Debug.Log("Command: " + terminalString.Substring(0, 5));
			//Debug.Log("Content: " + content);
			content = content.Substring(1, content.Length - 1);
			string xString = "";
			string zString= "";
			int i = 0;
			while(true)
			{
				if(i > content.Length-1) break;
				if(content[i] == ',') break;
				xString += content[i];
				i++;
			}
			i++;
			while(true)
			{
				if(i > content.Length-1) break;
				zString += content[i];
				i++;
			}
			int x = int.Parse(xString);
			int z = int.Parse(zString);
			Debug.Log(x);
			Debug.Log(z);
			worldGrid.regenerateChunk(x, z);
		}
	}
}
