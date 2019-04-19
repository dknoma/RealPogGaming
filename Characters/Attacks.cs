using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Attacks : MonoBehaviour {

	private UnityEvent attackAEvent;
	private UnityEvent attackBEvent;
	private UnityEvent supportEvent;

	public virtual void DoAttackA() {
		
	}
	
	public virtual void DoAttackB() {
		
	}

	public virtual void DoSupport() {
		
	}
}
