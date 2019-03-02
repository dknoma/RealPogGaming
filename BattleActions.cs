using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleActions : MonoBehaviour {

	public enum Action { Attack, Defend, UseItem };

	// action menu that loops around to beginning

	private Action currentAction;
	private int atk;
	private Element element = Element.None;

	public BattleActions() {
	}

	public int getAtk() {
		return atk;
	}

	public Element getElement() {
		return element;
	}

	public void setElement(Element element) {
		this.element = element;
	}
}
