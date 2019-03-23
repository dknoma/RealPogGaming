using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Party : MonoBehaviour {

	public enum Slot { Front, Back };

	public GameObject frontUnit;
	public GameObject backUnit;

	private List<GameObject> partyMembers = new List<GameObject>();
		
	// Use this for initialization
	private void Awake () {
//		this.partyMembers.Add(new PartyMember(this.frontUnit, (int) Slot.Front));
//		if(this.backUnit != null) {
//			this.partyMembers.Add(new PartyMember(this.backUnit, (int) Slot.Back));
//		}
	}

	public void InitPartyMembers() {
		frontUnit.GetComponent<Character> ().setPartySlot ((int) Slot.Front);
		partyMembers.Add(frontUnit);
		if(backUnit != null) {
			backUnit.GetComponent<Character> ().setPartySlot ((int) Slot.Back);
			partyMembers.Add(backUnit);
		}
		Debug.Log ("Party length: " + partyMembers.Count);
	}

	public void ChangePartyMember(GameObject newMember) {
		
	}

	public List<GameObject> GetPartyMembers() {
		return partyMembers;
	}
}
