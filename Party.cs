using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Party : MonoBehaviour {

	public enum Slot { Front, Back };

	public GameObject frontUnit;
	public GameObject backUnit;

	private List<GameObject> partyMembers;
		
	// Use this for initialization
	void Awake () {
		this.partyMembers = new List<GameObject>();
//		this.partyMembers.Add(new PartyMember(this.frontUnit, (int) Slot.Front));
//		if(this.backUnit != null) {
//			this.partyMembers.Add(new PartyMember(this.backUnit, (int) Slot.Back));
//		}
	}

	public void initPartyMembers() {
		this.frontUnit.GetComponent<Character> ().setPartySlot ((int) Slot.Front);
		this.partyMembers.Add(this.frontUnit);
		if(this.backUnit != null) {
			this.backUnit.GetComponent<Character> ().setPartySlot ((int) Slot.Back);
			this.partyMembers.Add(this.backUnit);
		}
		Debug.Log ("Party length: " + this.partyMembers.Count);
	}

	public void changePartyMember(GameObject newMember) {
		
	}

	public List<GameObject> getPartyMembers() {
		return this.partyMembers;
	}
}
