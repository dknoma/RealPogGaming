using System;
using UnityEngine;

public enum Transition {
	Fade,
	Triangle,
	Bubble
}

[ExecuteInEditMode]
public class ScreenTransition : MonoBehaviour {

	public Transition transition;
	public Material triangleTransitionMaterial;
	[DisableInspectorEdit] public bool isTransitioningToBlack;
	[DisableInspectorEdit] public bool isTransitioningFromBlack;

	private float transitionCounter;
	private float transitionCutoff;
	private static readonly int Cutoff = Shader.PropertyToID("_Cutoff");
	private static readonly int Reverse = Shader.PropertyToID("_Reverse");

	private void Update() {
		TestButtons();
	}

	private void FixedUpdate() {
		TransitionScreen();
	}

	private void TestButtons() {
		if (Input.GetButtonDown("Fire2")) {
			isTransitioningToBlack = true;
		} else if (Input.GetButtonDown("Fire3")) {
			isTransitioningFromBlack = true;
		}
	}
	
	private void TransitionScreen() {
		switch (transition) {
			case Transition.Fade:
				break;
			case Transition.Triangle:
				if (isTransitioningToBlack) {
					triangleTransitionMaterial.SetFloat(Reverse, 0);
					transitionCutoff = Mathf.Clamp(transitionCutoff + 0.03f, 0, 1); // Set max of cutoff to 1
					triangleTransitionMaterial.SetFloat(Cutoff, transitionCutoff);
					if (Mathf.Abs(transitionCutoff - 1) < Mathf.Epsilon) {
						isTransitioningToBlack = false;
					}
				} else {
					if (isTransitioningFromBlack) {
						triangleTransitionMaterial.SetFloat(Reverse, 1);
						transitionCutoff = Mathf.Clamp(transitionCutoff - 0.03f, 0, 1); // Set max of cutoff to 1
						triangleTransitionMaterial.SetFloat(Cutoff, transitionCutoff);
						if (transitionCutoff < Mathf.Epsilon) {
							isTransitioningFromBlack = false;
						}
					}   
				}
				break;
			case Transition.Bubble:
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	private void OnRenderImage(RenderTexture src, RenderTexture dest) {
		if (triangleTransitionMaterial != null) {
			Graphics.Blit(src, dest, triangleTransitionMaterial);
		}
	}
}
