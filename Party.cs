using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Party : MonoBehaviour {

	public enum Slot { Front, Back };

	public GameObject frontUnit;
	public GameObject backUnit;

	public List<PartyMember> partyMembers = new List<PartyMember> ();
		
	// Use this for initialization
	void Start () {
		this.partyMembers.Add(new PartyMember(this.frontUnit, (int) Slot.Front));
		if(this.backUnit != null) {
			this.partyMembers.Add(new PartyMember(this.backUnit, (int) Slot.Back));
		}
	}

	public List<PartyMember> getPartyMembers() {
		return this.partyMembers;
	}
}
