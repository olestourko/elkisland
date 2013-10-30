using UnityEngine;
using System.Collections;

public class BT_Stalker_C_SelectiveHide : BT_Selector {

	public BT_Stalker_C_SelectiveHide() : base()
	{
		BT_Sequence seq0 = new BT_Sequence();
			BT_Stalker_Hidden c0 = new BT_Stalker_Hidden();
			BT_Stalker_C_Hide a0 = new BT_Stalker_C_Hide(BT_Stalker_C_Hide.Mode.Target);
		BT_Stalker_C_HideWithTimeout a1 = new BT_Stalker_C_HideWithTimeout();
		
		this.AddChild(seq0);
			seq0.AddChild(c0);
			seq0.AddChild(a0);
		this.AddChild(a1);
	}
}
