using System;
using UnityEngine;

public enum Transition {
	Fade,
	Triangle,
	HorizontalDistortion,
	Bubble
}

[ExecuteInEditMode]
public class ScreenTransition : MonoBehaviour {

	public Transition transition;
	public Material triangleTransitionMaterial;
	public Material horizontalDistortionTransitionMaterial;
	[DisableInspectorEdit] public bool isTransitioningToBlack;
	[DisableInspectorEdit] public bool isTransitioningFromBlack;

	private float transitionCounter;
	private float transitionCutoff;
	private static readonly int Cutoff = Shader.PropertyToID("_Cutoff");
	private static readonly int Reverse = Shader.PropertyToID("_Reverse");

	private void OnEnable() {
		ResetMaterial(triangleTransitionMaterial);
		ResetMaterial(horizontalDistortionTransitionMaterial);
	}

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
		} else if (Input.GetButtonDown("Tab")) {
			switch (transition) {
				case Transition.Fade:
					transition = Transition.Triangle;
					break;
				case Transition.Triangle:
					transition = Transition.HorizontalDistortion;
					break;
				case Transition.HorizontalDistortion:
					transition = Transition.Bubble;
					break;
				case Transition.Bubble:
					transition = Transition.Fade;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
	
	private void TransitionScreen() {
		switch (transition) {
			case Transition.Fade:
				break;
			case Transition.Triangle:
				// Smoothly transition to black
				if (isTransitioningToBlack) {
					triangleTransitionMaterial.SetFloat(Reverse, 0);	// Sets the reverse "boolean" of the mat
					transitionCutoff = Mathf.Clamp(transitionCutoff + 0.02f, 0, 1); // Set max of cutoff to 1
					triangleTransitionMaterial.SetFloat(Cutoff, transitionCutoff); // sets cutoff value of mat
					if (Mathf.Abs(transitionCutoff - 1) < Mathf.Epsilon) {
						isTransitioningToBlack = false;	// Finish transitioning
					}
				} else {
					// Smoothly transition from black
					if (isTransitioningFromBlack) {
						triangleTransitionMaterial.SetFloat(Reverse, 1);
						transitionCutoff = Mathf.Clamp(transitionCutoff - 0.02f, 0, 1); // Set max of cutoff to 1
						triangleTransitionMaterial.SetFloat(Cutoff, transitionCutoff);
						if (transitionCutoff < Mathf.Epsilon) {
							isTransitioningFromBlack = false;
						}
					}   
				}
				break;
			case Transition.Bubble:
				break;
			case Transition.HorizontalDistortion:
				// Smoothly transition to black
				if (isTransitioningToBlack) {
					horizontalDistortionTransitionMaterial.SetFloat(Reverse, 0);
					transitionCutoff = Mathf.Clamp(transitionCutoff + 0.02f, 0, 1); // Set max of cutoff to 1
					horizontalDistortionTransitionMaterial.SetFloat(Cutoff, transitionCutoff);
					if (Mathf.Abs(transitionCutoff - 1) < Mathf.Epsilon) {
						isTransitioningToBlack = false;
					}
				} else {
					// Smoothly transition from black
					if (isTransitioningFromBlack) {
						horizontalDistortionTransitionMaterial.SetFloat(Reverse, 1);
						transitionCutoff = Mathf.Clamp(transitionCutoff - 0.02f, 0, 1); // Set max of cutoff to 1
						horizontalDistortionTransitionMaterial.SetFloat(Cutoff, transitionCutoff);
						if (transitionCutoff < Mathf.Epsilon) {
							isTransitioningFromBlack = false;
						}
					}   
				}
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	private void OnRenderImage(RenderTexture src, RenderTexture dest) {
		switch (transition) {
			case Transition.Fade:
				break;
			case Transition.Triangle:
				if (triangleTransitionMaterial != null) {
					Graphics.Blit(src, dest, triangleTransitionMaterial);
				}
				break;
			case Transition.HorizontalDistortion:
				if (horizontalDistortionTransitionMaterial != null) {
					Graphics.Blit(src, dest, horizontalDistortionTransitionMaterial);
				}
				break;
			case Transition.Bubble:
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	private void ResetMaterial(Material mat) {
		if (mat == null) return;
		mat.SetFloat(Reverse, 0);
		mat.SetFloat(Cutoff, 0);
	}
}
