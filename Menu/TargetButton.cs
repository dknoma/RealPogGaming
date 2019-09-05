using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TargetButton : MenuButton {

	private GameObject targetUnit;

	private UnityEvent chooseTargetEvent = new UnityEvent();
	
	public GameObject target {
		get {
			return targetUnit;
		}
		set {
			targetUnit = value;
		}
	}

	protected override void Start() {
		
	}

	public void AddChooseTargetEventListener(UnityAction<GameObject> call) {
		chooseTargetEvent.AddListener(() => call(target));
	}
	
	public void RemoveChooseTargetEventListener(UnityAction<GameObject> call) {
		chooseTargetEvent.RemoveListener(() => call(target));
	}
	
	private void Press() {
		Debug.LogFormat("Button was pressed.");
		if (!IsActive() || !IsInteractable())
			return;
		UISystemProfilerApi.AddMarker("TargetButton.onClick", this);
		chooseTargetEvent.Invoke();	// Attacks the target chosen by UIManager
	}

	public override void OnSubmit(BaseEventData eventData) {
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
}
