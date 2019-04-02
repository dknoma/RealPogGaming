using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// water-electric = 0-3, light-dark = 4,5, none = 6
public enum Element { 
	Water, 
	Fire, 
	Earth, 
	Electric, 
	Light, 
	Dark,
	None 
};

public class ElementalAffinity : MonoBehaviour {


	private static readonly int LastNormalEle = (int)Element.Electric;

	public static float CalcElementalDamage(Element source, Element target) {
		switch(source) {
		case Element.None:
			return 1.0f;
		case Element.Water:
		case Element.Fire:
		case Element.Earth:
		case Element.Electric:
			return CalcNormal(source, target);
		case Element.Light:
		case Element.Dark:
			return CalcLd(source, target);
		default:
			return 1.0f;
		}
	}

	// Normal elemental damage calculations
	private static float CalcNormal(Element source, Element target) {
		if ((int)target == ((int)source + 1) % (LastNormalEle+1)) {
			return 1.5f;
		} else if ((int)target == ((int)source + LastNormalEle) % (LastNormalEle+1)) {
			return 0.5f;
		}
		return 1.0f;
	}

	// Light-dark elemental damage calculations
	private static float CalcLd(Element source, Element target) {
		if((int) target > LastNormalEle && ((int) target == ((int) source+1) % 6 || (int) target == ((int) source+5) % 6)) {
			return 1.5f;
		}
		return 1.0f;
	}
}
