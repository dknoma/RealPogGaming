using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyMember : MonoBehaviour {

	private GameObject member;
	private int partySlot;

	public PartyMember(GameObject member, int slot) {
		this.member = member;
		this.partySlot = slot;
	}

	public GameObject getPartyMember() {
		return this.member;
	}

	public int getPartySlot() {
		return this.partySlot;
	}
}
