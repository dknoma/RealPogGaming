using System.Collections;
using System.Collections.Generic;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.Events;

public class Attacks : MonoBehaviour {

	public IntValue attackBManaCost;
	public IntValue supportManaCost;

	private UnityEvent attackAEvent;
	private UnityEvent attackBEvent;
	private UnityEvent supportEvent;

	public virtual void DoAttackA() {
		// TODO: Create sub classes for each unit. maybe even make one for enemies (obviously would do attacks automatically)
		//		 	Maybe move attack names and stuff to this class as well.
		//			Attacks need target(s)
		//			Support skills need target(s) as well
		//				targets probably chosen from Buttons during battle
		//				Perform action on target, invoke action event to listeners (battle manager in this case)
	}
	
	public virtual void DoAttackB() {
		
	}

	public virtual void DoSupport() {
		
	}

	protected virtual void SwordA() {
		
	}
	
	protected virtual void SwordB() {
		
	}

	protected virtual void Support() {
		
	}
}
