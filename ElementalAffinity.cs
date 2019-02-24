﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementalAffinity : MonoBehaviour {

	// water-electric = 0-3, light-dark = 4,5, none = 6
	public enum Element { Water, Fire, Earth, Electric, Light, Dark, None };

	public static float calcElementalDamage(Element source, Element target) {
		switch(source) {
		case Element.None:
			return 1.0f;
		case Element.Water:
		case Element.Fire:
		case Element.Earth:
		case Element.Electric:
			return calcNormal(source, target);
		case Element.Light:
		case Element.Dark:
			return calcLD(source, target);
		default:
			return 1.0f;
		}
	}

	// Normal elemental damage calculations
	private static float calcNormal(Element source, Element target) {
		if((int) target == ((int) source+1) % 4) {
			return 1.5f;
		} else if((int) target == ((int) source+3) % 4) {
			return 0.5f;
		}
		return 1.0f;
	}

	// Light-dark elemental damage calculations
	private static float calcLD(Element source, Element target) {
		if((int) target > 3 && ((int) target == ((int) source+1) % 6 || (int) target == ((int) source+5) % 6)) {
			return 1.5f;
		}
		return 1.0f;
	}
}
