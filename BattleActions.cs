using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleActions : MonoBehaviour {

	public enum Action { Attack, Defend, UseItem };

	private Action currentAction;
	private int atk;
	private ElementalAffinity.Element element = ElementalAffinity.Element.None;

	public BattleActions() {
	}

	public int getAtk() {
		return this.atk;
	}

	public ElementalAffinity.Element getElement() {
		return this.element;
	}

	public void setElement(ElementalAffinity.Element element) {
		this.element = element;
	}
}
