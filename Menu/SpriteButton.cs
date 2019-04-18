using UnityEngine;

public class SpriteButton : MenuButton {
	
	private SpriteRenderer spriteRenderer;

	private const Transition MY_TRANSITION = Transition.SpriteSwap;

	public SpriteRenderer SpriteRenderer {
		get { return spriteRenderer; }
	}
	
	/// <summary>
	/// Override UIBehaviour Start(). Safe to override as Selectable doesn't use Start()
	/// </summary>
	protected override void Start() {
		transition = MY_TRANSITION;
		if (spriteRenderer == null) {
			spriteRenderer = GetComponent<SpriteRenderer>();
		}
	}

	protected override void DoStateTransition(SelectionState state, bool instant) {
		Sprite newSprite;
		switch (state) {
			case SelectionState.Normal:
				Debug.LogFormat("Normal {0}", name);
//				if(text == null  && GetComponentInChildren<TextMeshProUGUI>() != null) {
//					text = GetComponentInChildren<TextMeshProUGUI>();
//					Vector3 localPos = text.transform.localPosition;
//					textLocalPos = new Vector3(localPos.x + 0.5f, localPos.y, localPos.z-1);
//					Debug.LogFormat("init: {0}", textLocalPos);
//				}
//				if (text != null) {
//					text.transform.localPosition = textLocalPos;
//					Debug.LogFormat("text local pos: {0}, {1}", textLocalPos, text.transform.localPosition);
//				}
				newSprite = spriteState.disabledSprite;
				break;
			case SelectionState.Highlighted:
				Debug.LogFormat("Highlighting {0}", name);
				newSprite = spriteState.highlightedSprite;
//				if (text != null) {
//					text.transform.localPosition = new Vector3(0, 0.5f, -1);
//				}
				break;
			case SelectionState.Pressed:
				Debug.LogFormat("Pressing {0}", name);
				newSprite = spriteState.pressedSprite;
//				if (text != null) {
//					text.transform.localPosition = textLocalPos;
//				}
				break;
			case SelectionState.Disabled:
				Debug.LogFormat("Disabling {0}", name);
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
		if (spriteRenderer == null) {
			spriteRenderer = GetComponent<SpriteRenderer>();
		}
//		image.overrideSprite = newSprite;
		spriteRenderer.sprite = newSprite;
		Debug.LogFormat("spriteRenderer.sprite: {0}", spriteRenderer.sprite.name);
	}
}
