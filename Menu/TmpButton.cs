using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class TmpButton : Selectable, ISubmitHandler, ICancelHandler {

	private TextMeshProUGUI text;
	private Vector3 textLocalPos;
	
	[SerializeField]
	private Button.ButtonClickedEvent m_OnClick = new Button.ButtonClickedEvent();
	[SerializeField]
	private ButtonCancelEvent m_OnCancel = new ButtonCancelEvent();
	
	protected TmpButton()
	{
	}
	
	/// <summary>
	///   <para>UnityEvent that is triggered when the Button is pressed.</para>
	/// </summary>
	public Button.ButtonClickedEvent onClick
	{
		get
		{
			return m_OnClick;
		}
		set
		{
			m_OnClick = value;
		}
	}
	
	/// <summary>
	///   <para>UnityEvent that is triggered when canceling the current action.</para>
	/// </summary>
	public ButtonCancelEvent onCancel {
		get {
			return m_OnCancel;
		}
		set {
			m_OnCancel = value;
		}
	}

	private const Transition MY_TRANSITION = Transition.SpriteSwap;

	protected override void Start() {
		transition = MY_TRANSITION;
		if(text == null) {
			text = GetComponentInChildren<TextMeshProUGUI>();
			Vector3 localPos = text.transform.localPosition;
			textLocalPos = new Vector3(localPos.x + 0.5f, localPos.y, localPos.z-1);
			Debug.LogFormat("null: {0}", textLocalPos);
		} else {
			textLocalPos = new Vector3(0.5f, 0, -1);
			Debug.LogFormat("local: {0}", textLocalPos);
		}
	}
	
	private void Press() {
		if (!IsActive() || !IsInteractable())
			return;
		UISystemProfilerApi.AddMarker("Button.onClick", this);
		m_OnClick.Invoke();
	}
	
	/// <summary>
	///   <para>Registered IPointerClickHandler callback.</para>
	/// </summary>
	/// <param name="eventData">Data passed in (Typically by the event system).</param>
	public virtual void OnPointerClick(PointerEventData eventData) {
		if (eventData.button != PointerEventData.InputButton.Left)
			return;
		Press();
	}

	/// <summary>
	///   <para>Registered ISubmitHandler callback.</para>
	/// </summary>
	/// <param name="eventData">Data passed in (Typically by the event system).</param>
	public virtual void OnSubmit(BaseEventData eventData) {
		Press();
		if (!IsActive() || !IsInteractable())
			return;
		DoStateTransition(SelectionState.Pressed, false);
		StartCoroutine(OnFinishSubmit());
	}
	
	/// <summary>
	///   <para>Finish the buttons color fade/sprite swap/animation.</para>
	/// </summary>
	private IEnumerator OnFinishSubmit() {
		var fadeTime = colors.fadeDuration;
		var elapsedTime = 0f;

		while (elapsedTime < fadeTime) {
			elapsedTime += Time.unscaledDeltaTime;
			yield return null;
		}

		DoStateTransition(currentSelectionState, false);
	}
	
	//TODO: change state transition to let button know when to change text transform
	protected override void DoStateTransition(SelectionState state, bool instant) {
		Sprite newSprite;
		switch (state) {
			case SelectionState.Normal:
				Debug.LogFormat("Normal {0}", name);
				if(text == null) {
					text = GetComponentInChildren<TextMeshProUGUI>();
					Vector3 localPos = text.transform.localPosition;
					textLocalPos = new Vector3(localPos.x + 0.5f, localPos.y, localPos.z-1);
					Debug.LogFormat("init: {0}", textLocalPos);
				}
				text.transform.localPosition = textLocalPos;
				Debug.LogFormat("text local pos: {0}, {1}", textLocalPos, text.transform.localPosition);
				newSprite = null;
				break;
			case SelectionState.Highlighted:
				Debug.LogFormat("Highlighting {0}", name);
				newSprite = spriteState.highlightedSprite;
				text.transform.localPosition = new Vector3(0, 0.5f, -1);
				break;
			case SelectionState.Pressed:
				Debug.LogFormat("Pressing {0}", name);
				newSprite = spriteState.pressedSprite;
				text.transform.localPosition = textLocalPos;
				break;
			case SelectionState.Disabled:
				Debug.LogFormat("Disabling {0}", name);
				newSprite = spriteState.disabledSprite;
				break;
			default:
				newSprite = null;
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
		if (image == null)
			return;
		image.overrideSprite = newSprite;
	}
	
//	public virtual void OnCancelAction(BaseEventData eventData) {
////			this.Press();
////			if (!this.IsActive() || !this.IsInteractable())
////				return;
////			this.DoStateTransition(Selectable.SelectionState.Pressed, false);
////			this.StartCoroutine(this.OnFinishSubmit());
//	}

	private void Cancel() {
		if (!IsActive() || !IsInteractable())
			return;
		UISystemProfilerApi.AddMarker("Button.onCancel", this);
		m_OnCancel.Invoke();
	}

	public virtual void OnCancel(BaseEventData eventData) {
		Debug.Log("CANCELING FROM TMP BUTTON.");
		Cancel();
	}
}
/// <inheritdoc />
/// <summary>
///   <para>Function definition for a button click event.</para>
/// </summary>
[Serializable]
public class ButtonCancelEvent : UnityEvent {
}