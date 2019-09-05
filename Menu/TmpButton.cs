using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Debug = UnityEngine.Debug;

public class TmpButton : MenuButton {

	public bool usingImage;
	
	private SpriteRenderer spriteRenderer;
	
	private TextMeshProUGUI text;
	private Vector3 textLocalPos;

	private const Transition MY_TRANSITION = Transition.SpriteSwap;
	
	public SpriteRenderer SpriteRenderer {
		get {
			if (spriteRenderer == null && GetComponent<SpriteRenderer>() != null) {
				spriteRenderer = GetComponent<SpriteRenderer>();
			}
			return spriteRenderer;
		}
	}
	
	protected override void Start() {
		transition = MY_TRANSITION;
		if(text == null && GetComponentInChildren<TextMeshProUGUI>() != null) {
			text = GetComponentInChildren<TextMeshProUGUI>();
			Vector3 localPos = text.transform.localPosition;
			textLocalPos = new Vector3(localPos.x - 0.06175f, localPos.y, localPos.z-1);
//			Debug.LogFormat("null: {0}", textLocalPos);
		} else {
			textLocalPos = new Vector3(-0.06175f, 0, -1);
//			Debug.LogFormat("local: {0}", textLocalPos);
		}
		if (spriteRenderer == null && GetComponent<SpriteRenderer>() != null) {
			spriteRenderer = GetComponent<SpriteRenderer>();
		}
	}
	
	/// <summary>
	/// Position changes by 0.5 / ppu
	/// </summary>
	/// <param name="state"></param>
	/// <param name="instant"></param>
	protected override void DoStateTransition(SelectionState state, bool instant) {
		Sprite newSprite;
		switch (state) {
			case SelectionState.Normal:
//				Debug.LogFormat("Normal {0}", name);
				if(text == null  && GetComponentInChildren<TextMeshProUGUI>() != null) {
					text = GetComponentInChildren<TextMeshProUGUI>();
					Vector3 localPos = text.transform.localPosition;
					textLocalPos = new Vector3(localPos.x + 0.18675f, localPos.y, localPos.z-1);
//					Debug.LogFormat("init: {0}", textLocalPos);
				}
				if (text != null) {
					text.transform.localPosition = textLocalPos;
//					Debug.LogFormat("text local pos: {0}, {1}", textLocalPos, text.transform.localPosition);
				}
				newSprite = spriteState.disabledSprite;
				break;
			case SelectionState.Highlighted:
//				Debug.LogFormat("Highlighting {0}", name);
				newSprite = spriteState.highlightedSprite;
				if (text != null) {
					text.transform.localPosition = new Vector3(- 0.18675f, 0.125f, -1);
				}
				break;
			case SelectionState.Pressed:
//				Debug.LogFormat("Pressing {0}", name);
				newSprite = spriteState.pressedSprite;
				if (text != null) {
					text.transform.localPosition = textLocalPos;
				}
				break;
			case SelectionState.Disabled:
//				Debug.LogFormat("Disabling {0}", name);
				newSprite = spriteState.disabledSprite;
				break;
			default:
				newSprite = spriteState.disabledSprite;
				break;
		}
		if (!gameObject.activeInHierarchy)
			return;
		switch (transition) {
			case Transition.SpriteSwap:
				DoSpriteSwap(newSprite);
				break;
		}
	}

	private void DoSpriteSwap(Sprite newSprite) {
		if (image != null && usingImage) { // If has image, change image
			image.overrideSprite = newSprite;
		} 
		if (spriteRenderer == null) return; // If has render, change sprite
		spriteRenderer.sprite = newSprite;
	}
}
