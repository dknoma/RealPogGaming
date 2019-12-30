using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum MenuEventType {
	OnUpdateSelected,
	Select,
	Deselect,
	Submit
}

public class MenuEventSystem : MonoBehaviour {

//	private static List<MenuEventSystem> m_MenuEventSystems = new List<MenuEventSystem>();

	private Dictionary <MenuEventType, UnityEvent> eventDictionary;
	
	public GameObject firstSelectedGameObject;
	private GameObject currentSelectedGameObject;
	private Selectable[] buttons;

	public static MenuEventSystem eventSystem;
	
	/// <summary>
	///   <para>Return the current MenuEventSystem.</para>
	/// </summary>
	public static MenuEventSystem instance {
		get {
			if (!eventSystem) {
				eventSystem = FindObjectOfType<MenuEventSystem>();
				if (!eventSystem) {
					Debug.LogError("There needs to be one active MenuEventSystem in the scene.");
				} else {
					eventSystem.Init();
				}
			}

			return eventSystem;
//			return m_MenuEventSystems.Count <= 0 ? null : m_MenuEventSystems[0];
		}
		set {
//			int index = m_MenuEventSystems.IndexOf(value);
//			if (index < 0)
//				return;
//			m_MenuEventSystems.RemoveAt(index);
//			m_MenuEventSystems.Insert(0, value);
		}
	}

	private void Start() {
		buttons = Selectable.allSelectablesArray;
//		Debug.LogFormat("buttons: {0}", buttons.Count);
//		foreach (Selectable button in buttons) {
//			Debug.LogFormat("Button: {0}", button.name);
//		}
////		Debug.LogFormat("Button: {0}", firstSelectedGameObject.name);
//		Button but = firstSelectedGameObject.GetComponent<Button>();
//		Debug.LogFormat("new: {0}", but.FindSelectableOnDown().name);
//		Debug.LogFormat("new: {0}", but.FindSelectableOnDown().name);
//		Debug.LogFormat("new: {0}", but.FindSelectableOnDown().name);
	}

	private void Init() {
		if (eventDictionary == null) {
			eventDictionary = new Dictionary<MenuEventType, UnityEvent>();
		}
	}
	
	public static void StartListening (MenuEventType eventType, UnityAction listener) {
		UnityEvent thisEvent = null;
		if (instance.eventDictionary.TryGetValue (eventType, out thisEvent)) {
			thisEvent.AddListener (listener);
		}  else {
			thisEvent = new UnityEvent ();
			thisEvent.AddListener (listener);
			instance.eventDictionary.Add (eventType, thisEvent);
		}
	}
	
	public static void StopListening (MenuEventType eventType, UnityAction listener) {
		if (eventSystem == null) return;
		UnityEvent thisEvent = null;
		if (instance.eventDictionary.TryGetValue (eventType, out thisEvent)) {
			thisEvent.RemoveListener (listener);
		}
	}
	
	public static void TriggerEvent (MenuEventType eventType) {
		UnityEvent thisEvent = null;
		if (instance.eventDictionary.TryGetValue (eventType, out thisEvent)) {
			thisEvent.Invoke ();
		}
	}
}
