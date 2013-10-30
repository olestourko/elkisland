using UnityEngine;
using System.Collections;

public class BT_Stalker_C_HideWithTimeout : BT_Selector {

	public BT_Stalker_C_HideWithTimeout()
	{
		BT_Sequence seq0 = new BT_Sequence();
		BT_Timeout c0 = new BT_Timeout(1.5f);
		BT_Stalker_C_Hide a0 = new BT_Stalker_C_Hide(BT_Stalker_C_Hide.Mode.NotVisible);
		BT_Stalker_C_Hide a1 = new BT_Stalker_C_Hide(BT_Stalker_C_Hide.Mode.Target);
		
		this.AddChild(seq0);
			seq0.AddChild(c0);
			seq0.AddChild(a0);
		this.AddChild(a1);
	}
}
