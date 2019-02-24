using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action : MonoBehaviour {

	private int atk;
	private ElementalAffinity.Element element = ElementalAffinity.Element.None;

	public Action() {
	}

	public int getAtk() {
		return this.atk;
	}

	public ElementalAffinity.Element getElement() {
		return this.element;
	}

	public void getElement(ElementalAffinity.Element element) {
		this.element = element;
	}
}
