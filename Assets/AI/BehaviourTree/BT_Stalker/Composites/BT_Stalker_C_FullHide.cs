using UnityEngine;
using System.Collections;

public class BT_Stalker_C_FullHide : BT_Selector {

	public BT_Stalker_C_FullHide()
	{
		BT_Sequence seq0 = new BT_Sequence();
			BT_Stalker_Hidden c0 = new BT_Stalker_Hidden();
			BT_Stalker_PlayerInRange c1 = new BT_Stalker_PlayerInRange(0.0f, 1.0f);
			BT_Stalker_C_HideWithTimeout a0 = new BT_Stalker_C_HideWithTimeout();
		BT_Stalker_C_SelectiveHide comp0 = new BT_Stalker_C_SelectiveHide();
		
		this.AddChild(seq0);
			seq0.AddChild(c0);
			seq0.AddChild(c1);
			seq0.AddChild(a0);
		this.AddChild(comp0);	
	}
}
