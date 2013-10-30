using UnityEngine;
using System.Collections;

public class State {
	
	public virtual void Enter(BaseAI _ai) {}
	public virtual void Execute(BaseAI _ai) {}
	public virtual void Exit(BaseAI _ai) {}
}
