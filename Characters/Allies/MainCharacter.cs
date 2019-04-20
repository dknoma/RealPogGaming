using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCharacter : Player {

	private const int ATTACK_B_HITS = 2;
	
	public override void DoAttackA(GameObject target) {
		// TODO: Create sub classes for each unit. maybe even make one for enemies (obviously would do attacks automatically)
		//		 	Maybe move attack names and stuff to this class as well.
		//			Attacks need target(s)
		//			Support skills need target(s) as well
		//				targets probably chosen from Buttons during battle
		//				Perform action on target, invoke action event to listeners (battle manager in this case)
		Debug.LogFormat("{0} is attacking.", name);
		int damage = CalculateSingleHitDamage(target);
		TryDamage(damage, target);
		attackAEvent.Invoke();
		actionEvent.Invoke();
	}
	
	public override void DoAttackB(GameObject target) {
		// 2-hit attack
		int damage = CalculateDamage(target);
		// Divide damage per hit so that damage isnt reduced during calculation
		// Want total damage to be about the same rather than being reduced due to defense calculations
		int dmgPerHit = damage / ATTACK_B_HITS;	
		// Do damage ATACK_B_HITS times
		for (int i = 0; i < ATTACK_B_HITS; i++) {
			TryDamage(dmgPerHit, target);
		}
		attackBEvent.Invoke();
		actionEvent.Invoke();
	}

	public override void DoSupport(GameObject target) {
		Support();
		supportEvent.Invoke();
		actionEvent.Invoke();
	}

	protected override void SwordA() {
		
	}
	
	protected override void SwordB() {
		
	}

	protected override void Support() {
		AtkBuff();
	}

	private void AtkBuff() {
		Debug.LogFormat("Buffing attack");
		TryStatChange(StatChange.AtkUp, 3);
	}
}
