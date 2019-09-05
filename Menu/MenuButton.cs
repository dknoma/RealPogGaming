using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuButton : Selectable, ISubmitHandler, ICancelHandler {
    
    [SerializeField]
    private MenuButtonSubmitEvent m_OnClick = new MenuButtonSubmitEvent();
    [SerializeField]
    private MenuButtonCancelEvent m_OnCancel = new MenuButtonCancelEvent();
    
    /// <summary>
    ///   <para>UnityEvent that is triggered when the Button is pressed.</para>
    /// </summary>
    public MenuButtonSubmitEvent onClick {
        get {
            return m_OnClick;
        }
        set {
            m_OnClick = value;
        }
    }
	
    /// <summary>
    ///   <para>UnityEvent that is triggered when canceling the current action.</para>
    /// </summary>
    public MenuButtonCancelEvent onCancel {
        get {
            return m_OnCancel;
        }
        set {
            m_OnCancel = value;
        }
    }
    
    private void Press() {
        if (!IsActive() || !IsInteractable())
            return;
        UISystemProfilerApi.AddMarker("MenuButton.onClick", this);
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
    
    private void Cancel() {
        if (!IsActive() || !IsInteractable())
            return;
        UISystemProfilerApi.AddMarker("Button.onCancel", this);
        m_OnCancel.Invoke();
    }

    public virtual void OnCancel(BaseEventData eventData) {
//		Debug.Log("CANCELING FROM TMP BUTTON.");
        Cancel();
    }
}

/// <inheritdoc />
/// <summary>
///   <para>Function definition for a button submit event.</para>
/// </summary>
[Serializable]
public class MenuButtonSubmitEvent : UnityEvent {
}

/// <inheritdoc />
/// <summary>
///   <para>Function definition for a button cancel event.</para>
/// </summary>
[Serializable]
public class MenuButtonCancelEvent : UnityEvent {
}
