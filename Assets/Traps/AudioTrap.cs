using UnityEngine;
using System.Collections;

public class AudioTrap : Trap {
	
	public override void Activate()
	{
		Debug.Log ("AudioTrap Activated");
		int index = Random.Range(0, audioClips.Count-1);
		audio.PlayOneShot(audioClips[index]);
	}
}
