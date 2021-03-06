﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class gui : MonoBehaviour {
	
	private string pre = "> ";
	public string terminalString = "";
	
	public bool minimal;
	
	public int chunk;
	public int cell_count;
	public int path_count;
	public float distance_to_path;
	public float distance_travelled;
	public string version_number;


	public static string location_description;
	public bool render_description = false;

	private WorldGrid worldGrid;
	private ExperienceManager experienceManager;
	
	void Start()
	{
		worldGrid = GameObject.Find("WorldGrid").GetComponent("WorldGrid") as WorldGrid;
		experienceManager = GameObject.Find("ExperienceManager").GetComponent("ExperienceManager") as ExperienceManager;
	}
	
	void OnGUI() 
	{
		//Draw the Terminal
		if(Event.current.keyCode == KeyCode.Return && Event.current.type == EventType.KeyDown)
		{
			processInput();
			terminalString = "";
		}
		GUI.SetNextControlName("terminal");
		terminalString = GUI.TextField (new Rect (10, 10, 200, 22), pre + terminalString, 25);
		if(terminalString.Length > 1) terminalString = terminalString.Substring(2, terminalString.Length - 2);
		
		//Draw the information fields		
		Texture2D texture = new Texture2D(1, 1);
    	texture.SetPixel(0,0,Color.black);
    	texture.Apply();
		
		GUIStyle text = new GUIStyle();
		text.fontSize = 16;
		text.normal.textColor = Color.white;
		
		GUI.skin.box.normal.background = texture;
		if(!minimal)
		{
			//GUI.Box(new Rect (0, Screen.height - 125, 130, 25), GUIContent.none);
			GUI.Box(new Rect (0, Screen.height - 100, 130, 25), GUIContent.none);
			GUI.Box(new Rect (0, Screen.height - 75, 130, 25), GUIContent.none);
			GUI.Box(new Rect (0, Screen.height - 50, 100, 25), GUIContent.none);
			GUI.Box(new Rect (0, Screen.height - 25, 200, 25), GUIContent.none);
			
			//GUI.Label(new Rect (10, Screen.height - 125 + 4, 100, 20), "d to cottage: " + System.Math.Round(distance_to_cottage, 1), text);
			GUI.Label(new Rect (10, Screen.height - 100 + 4, 100, 20), "d travelled: " + System.Math.Round(distance_travelled, 1), text);
			GUI.Label(new Rect (10, Screen.height - 75 + 4, 100, 20), "d to path: " + System.Math.Round(distance_to_path, 1), text);
			GUI.Label(new Rect (10, Screen.height - 50 + 4, 100, 20), "cells: " + cell_count, text);
			GUI.Label(new Rect (10, Screen.height - 25 + 4, 100, 20), "chunks: " + chunk, text);
			GUI.Label(new Rect (110, Screen.height - 25 + 4, 100, 20), "paths: " + path_count, text);
		}
		//Version number
		GUI.Box(new Rect (Screen.width - 150, Screen.height - 25, 150, 30), GUIContent.none);
		GUI.Label(new Rect(Screen.width - 140, Screen.height - 25 + 4, 150, 30), version_number, text);

		//Overlay for location descriptions
		if(render_description)
		{
			Rect overlay = new Rect (0 + 50, 0 + 50 + 200, Screen.width - 100, Screen.height - 100 - 200);
			Rect overlay_text = new Rect (0 + 54, 0 + 54 + 200, Screen.width - 108, Screen.height - 108 - 200);
			GUI.Box(overlay, GUIContent.none);
			GUI.Label(overlay_text, location_description);
		}
	}
	
	private void processInput()
	{
		Match match = Regex.Match(terminalString, @"[A-Za-z0-9][A-Za-z0-9]*");
		
		string command = match.Groups[0].ToString();
		string remaining = terminalString.Substring(command.Length, terminalString.Length - command.Length);
			
		MatchCollection args_match = Regex.Matches(remaining, @"[A-Za-z0-9][A-Za-z0-9_]*");
		int i = 0;
		
		List<string> args = new List<string>();
		foreach(Match arg in args_match)
		{
			args.Add(arg.ToString());
			//Debug.Log("\t arg[" + i + "]: " + arg.ToString());
			i++;
		}
		
		/*--------------------------------------------------------------------------*/
		/*Process the various commands			           							*/
		/*--------------------------------------------------------------------------*/
		if(command.Equals("regen"))
		{
			if(args.Count != 0) 
			{
				Debug.Log ("Usage: regen");
				Debug.Log ("Regenerates the selected region");
			}
			else
			{
				worldGrid.RegenerateRegion_Threaded();
				//worldGrid.RegenerateSelectedRegion();
			}
		}
		
		else if(command.Equals("select"))
		{
			if(args.Count != 2)
			{
				Debug.Log ("Usage: select <x> <z>");
				Debug.Log ("Selects a region with the MeshChunk <x> <z> at the center.");
			}
			else
			{
				int x = int.Parse(args[0]);
				int z = int.Parse(args[1]);
				worldGrid.SelectRegion(x, z);
			}
		}
		
		else if(command.Equals("deselect"))
		{
			if(args.Count != 0)
			{
				Debug.Log ("Usage: deselect");
				Debug.Log ("Deselect the selected region.");
			}
			else
			{
				worldGrid.DeselectRegion();
			}
		}
		
		else if(command.Equals("quit"))
		{
			if(args.Count != 0)
			{
				Debug.Log ("Usage: quit");
				Debug.Log ("Quits the application.");
			}
			else
			{
				Application.Quit();
			}
		}
		
		/*
		else if(command.Equals("lighting"))
		{
			if(args.Count != 1)
			{
				Debug.Log ("Usage: lighting <lighting state id>");
				Debug.Log ("Changes the enviornmental lighting");
			}
			else
			{
				int c = int.Parse(args[0]);
				experienceManager.ChangeLighting(c);
			}
		}
		*/
		
		else if(command.Equals("spawn"))
		{
			if(args.Count != 1)
			{
				Debug.Log ("Usage: spawn <entity name>");
			}
			else
			{
				switch(args[0])
				{
				case "ai_stalker":
					experienceManager.SpawnAI();
					break;
				case "ai_pather":
					
					break;
				default:
					Debug.Log ("Entity not found");
					break;
					
				}
			}
		}
	}
}
