using System;
using UnityEditor;
using UnityEngine.Serialization;

namespace UnityEngine.EventSystems {
	/// <summary>
	///   <para>A BaseInputModule designed for keyboard  controller input.</para>
	/// </summary>
	[AddComponentMenu("Event/Standalone Input Module")]
	public class KeyInputManager : PointerInputModule {
		private int m_ConsecutiveMoveCount = 0;
		[SerializeField]
		private string m_HorizontalAxis = "Horizontal";
		[SerializeField]
		private string m_VerticalAxis = "Vertical";
		[SerializeField]
		private string m_SubmitButton = "Submit";
		[SerializeField]
		private string m_CancelButton = "Cancel";
		[SerializeField]
		private float m_InputActionsPerSecond = 10f;
		[SerializeField]
		private float m_RepeatDelay = 0.5f;
		private float m_PrevActionTime;
		private Vector2 m_LastMoveVector;
		private Vector2 m_LastMousePosition;
		private Vector2 m_MousePosition;
		private GameObject m_CurrentFocusedGameObject;
		private PointerEventData m_InputPointerEvent;
		[SerializeField]
		[FormerlySerializedAs("m_AllowActivationOnMobileDevice")]
		private bool m_ForceModuleActive;

		protected KeyInputManager() {
		}

		[Obsolete("Mode is no longer needed on input module as it handles both mouse and keyboard simultaneously.", false)]
		public InputMode inputMode {
			get {
				return InputMode.Mouse;
			}
		}

		/// <summary>
		///   <para>Is this module allowed to be activated if you are on mobile.</para>
		/// </summary>
		[Obsolete("allowActivationOnMobileDevice has been deprecated. Use forceModuleActive instead (UnityUpgradable) -> forceModuleActive")]
		public bool allowActivationOnMobileDevice {
			get {
				return m_ForceModuleActive;
			}
			set {
				m_ForceModuleActive = value;
			}
		}

		/// <summary>
		///   <para>Force this module to be active.</para>
		/// </summary>
		public bool forceModuleActive {
			get {
				return m_ForceModuleActive;
			}
			set {
				m_ForceModuleActive = value;
			}
		}

		/// <summary>
		///   <para>Number of keyboard / controller inputs allowed per second.</para>
		/// </summary>
		public float inputActionsPerSecond {
			get {
				return m_InputActionsPerSecond;
			}
			set {
				m_InputActionsPerSecond = value;
			}
		}

		/// <summary>
		///   <para>Delay in seconds before the input actions per second repeat rate takes effect.</para>
		/// </summary>
		public float repeatDelay {
			get {
				return m_RepeatDelay;
			}
			set {
				m_RepeatDelay = value;
			}
		}

		/// <summary>
		///   <para>Input manager name for the horizontal axis button.</para>
		/// </summary>
		public string horizontalAxis {
			get  {
				return m_HorizontalAxis;
			}
			set {
				m_HorizontalAxis = value;
			}
		}

		/// <summary>
		///   <para>Input manager name for the vertical axis.</para>
		/// </summary>
		public string verticalAxis {
			get {
				return m_VerticalAxis;
			}
			set {
				m_VerticalAxis = value;
			}
		}

		/// <summary>
		///   <para>Maximum number of input events handled per second.</para>
		/// </summary>
		public string submitButton {
			get {
				return m_SubmitButton;
			}
			set {
				m_SubmitButton = value;
			}
		}

		/// <summary>
		///   <para>Input manager name for the 'cancel' button.</para>
		/// </summary>
		public string cancelButton {
			get {
				return m_CancelButton;
			}
			set {
				m_CancelButton = value;
			}
		}

		private bool ShouldIgnoreEventsOnNoFocus() {
			switch (SystemInfo.operatingSystemFamily) {
				case OperatingSystemFamily.MacOSX:
				case OperatingSystemFamily.Windows:
				case OperatingSystemFamily.Linux:
					return !EditorApplication.isRemoteConnected;
				default:
					return false;
			}
		}

		/// <summary>
		///   <para>See BaseInputModule.</para>
		/// </summary>
		public override void UpdateModule() {
			if (!eventSystem.isFocused && ShouldIgnoreEventsOnNoFocus()) {
				if (m_InputPointerEvent != null && m_InputPointerEvent.pointerDrag != null && m_InputPointerEvent.dragging)
					ExecuteEvents.Execute<IEndDragHandler>(m_InputPointerEvent.pointerDrag, m_InputPointerEvent, ExecuteEvents.endDragHandler);
				m_InputPointerEvent = null;
			} else {
				m_LastMousePosition = m_MousePosition;
				m_MousePosition = input.mousePosition;
			}
		}

		/// <summary>
		///   <para>See BaseInputModule.</para>
		/// </summary>
		/// <returns>
		///   <para>Supported.</para>
		/// </returns>
		public override bool IsModuleSupported() {
			return m_ForceModuleActive || input.mousePresent || input.touchSupported;
		}

		/// <summary>
		///   <para>See BaseInputModule.</para>
		/// </summary>
		/// <returns>
		///   <para>Should activate.</para>
		/// </returns>
		public override bool ShouldActivateModule() {
			if (!base.ShouldActivateModule())
				return false;
			bool flag = m_ForceModuleActive | input.GetButtonDown(m_SubmitButton) | input.GetButtonDown(m_CancelButton) | !Mathf.Approximately(input.GetAxisRaw(m_HorizontalAxis), 0.0f) | !Mathf.Approximately(input.GetAxisRaw(m_VerticalAxis), 0.0f) | (m_MousePosition - m_LastMousePosition).sqrMagnitude > 0.0 | input.GetMouseButtonDown(0);
			if (input.touchCount > 0)
				flag = true;
			return flag;
		}

		/// <summary>
		///   <para>See BaseInputModule.</para>
		/// </summary>
		public override void ActivateModule() {
			if (!eventSystem.isFocused && ShouldIgnoreEventsOnNoFocus())
				return;
			base.ActivateModule();
			m_MousePosition = input.mousePosition;
			m_LastMousePosition = input.mousePosition;
			GameObject selectedGameObject = eventSystem.currentSelectedGameObject;
			if (selectedGameObject == null)
				selectedGameObject = eventSystem.firstSelectedGameObject;
			eventSystem.SetSelectedGameObject(selectedGameObject, GetBaseEventData());
		}

		/// <summary>
		///   <para>See BaseInputModule.</para>
		/// </summary>
		public override void DeactivateModule() {
			base.DeactivateModule();
			ClearSelection();
		}

		/// <summary>
		///   <para>See BaseInputModule.</para>
		/// </summary>
		public override void Process() {
			if (!eventSystem.isFocused && ShouldIgnoreEventsOnNoFocus())
				return;
			bool selectedObject = SendUpdateEventToSelectedObject();
			if (eventSystem.sendNavigationEvents) {
				if (!selectedObject)
					selectedObject |= SendMoveEventToSelectedObject();
				if (!selectedObject)
					SendSubmitEventToSelectedObject();
			}
			if (ProcessTouchEvents() || !input.mousePresent)
				return;
			ProcessMouseEvent();
		}

		private bool ProcessTouchEvents() {
			for (int index = 0; index < input.touchCount; ++index) {
				Touch touch = input.GetTouch(index);
				if (touch.type != TouchType.Indirect) {
					bool pressed;
					bool released;
					PointerEventData pointerEventData = GetTouchPointerEventData(touch, out pressed, out released);
					ProcessTouchPress(pointerEventData, pressed, released);
					if (!released) {
						ProcessMove(pointerEventData);
						ProcessDrag(pointerEventData);
					}
					else
						RemovePointerData(pointerEventData);
				}
			}
			return input.touchCount > 0;
		}

		/// <summary>
		///   <para>This method is called by Unity whenever a touch event is processed. Override this method with a custom implementation to process touch events yourself.</para>
		/// </summary>
		/// <param name="pointerEvent">Event data relating to the touch event, such as position and ID to be passed to the touch event destination object.</param>
		/// <param name="pressed">This is true for the first frame of a touch event, and false thereafter. This can therefore be used to determine the instant a touch event occurred.</param>
		/// <param name="released">This is true only for the last frame of a touch event.</param>
		protected void ProcessTouchPress(PointerEventData pointerEvent, bool pressed, bool released) {
			GameObject gameObject1 = pointerEvent.pointerCurrentRaycast.gameObject;
			if (pressed) {
				pointerEvent.eligibleForClick = true;
				pointerEvent.delta = Vector2.zero;
				pointerEvent.dragging = false;
				pointerEvent.useDragThreshold = true;
				pointerEvent.pressPosition = pointerEvent.position;
				pointerEvent.pointerPressRaycast = pointerEvent.pointerCurrentRaycast;
//				DeselectIfSelectionChanged(gameObject1, pointerEvent);
				if (pointerEvent.pointerEnter != gameObject1) {
					HandlePointerExitAndEnter(pointerEvent, gameObject1);
					pointerEvent.pointerEnter = gameObject1;
				}
				GameObject gameObject2 = ExecuteEvents.ExecuteHierarchy<IPointerDownHandler>(gameObject1, pointerEvent, ExecuteEvents.pointerDownHandler);
				if (gameObject2 == null)
					gameObject2 = ExecuteEvents.GetEventHandler<IPointerClickHandler>(gameObject1);
				float unscaledTime = Time.unscaledTime;
				if (gameObject2 == pointerEvent.lastPress) {
					if (unscaledTime - pointerEvent.clickTime < 0.300000011920929)
						++pointerEvent.clickCount;
					else
						pointerEvent.clickCount = 1;
					pointerEvent.clickTime = unscaledTime;
				}
				else
					pointerEvent.clickCount = 1;
				pointerEvent.pointerPress = gameObject2;
				pointerEvent.rawPointerPress = gameObject1;
				pointerEvent.clickTime = unscaledTime;
				pointerEvent.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(gameObject1);
				if (pointerEvent.pointerDrag != null)
					ExecuteEvents.Execute<IInitializePotentialDragHandler>(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.initializePotentialDrag);
				m_InputPointerEvent = pointerEvent;
			}
			if (!released)
				return;
			ExecuteEvents.Execute<IPointerUpHandler>(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerUpHandler);
			GameObject eventHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(gameObject1);
			if (pointerEvent.pointerPress == eventHandler && pointerEvent.eligibleForClick)
				ExecuteEvents.Execute<IPointerClickHandler>(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerClickHandler);
			else if (pointerEvent.pointerDrag != null && pointerEvent.dragging)
				ExecuteEvents.ExecuteHierarchy<IDropHandler>(gameObject1, pointerEvent, ExecuteEvents.dropHandler);
			pointerEvent.eligibleForClick = false;
			pointerEvent.pointerPress = null;
			pointerEvent.rawPointerPress = null;
			if (pointerEvent.pointerDrag != null && pointerEvent.dragging)
				ExecuteEvents.Execute<IEndDragHandler>(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.endDragHandler);
			pointerEvent.dragging = false;
			pointerEvent.pointerDrag = null;
			ExecuteEvents.ExecuteHierarchy<IPointerExitHandler>(pointerEvent.pointerEnter, pointerEvent, ExecuteEvents.pointerExitHandler);
			pointerEvent.pointerEnter = null;
			m_InputPointerEvent = pointerEvent;
		}

		/// <summary>
		///   <para>Calculate and send a submit event to the current selected object.</para>
		/// </summary>
		/// <returns>
		///   <para>If the submit event was used by the selected object.</para>
		/// </returns>
		protected bool SendSubmitEventToSelectedObject() {
			if (eventSystem.currentSelectedGameObject == null)
				return false;
			BaseEventData baseEventData = GetBaseEventData();
			if (input.GetButtonDown(m_SubmitButton))
				ExecuteEvents.Execute<ISubmitHandler>(eventSystem.currentSelectedGameObject, baseEventData, ExecuteEvents.submitHandler);
			if (input.GetButtonDown(m_CancelButton))
				ExecuteEvents.Execute<ICancelHandler>(eventSystem.currentSelectedGameObject, baseEventData, ExecuteEvents.cancelHandler);
			return baseEventData.used;
		}

		private Vector2 GetRawMoveVector() {
			Vector2 zero = Vector2.zero;
			zero.x = input.GetAxisRaw(m_HorizontalAxis);
			zero.y = input.GetAxisRaw(m_VerticalAxis);
			if (input.GetButtonDown(m_HorizontalAxis)) {
				if (zero.x < 0.0)
					zero.x = -1f;
				if (zero.x > 0.0)
					zero.x = 1f;
			}
			if (input.GetButtonDown(m_VerticalAxis)) {
				if (zero.y < 0.0)
					zero.y = -1f;
				if (zero.y > 0.0)
					zero.y = 1f;
			}
			return zero;
		}

		/// <summary>
		///   <para>Calculate and send a move event to the current selected object.</para>
		/// </summary>
		/// <returns>
		///   <para>If the move event was used by the selected object.</para>
		/// </returns>
		protected bool SendMoveEventToSelectedObject() {
			float unscaledTime = Time.unscaledTime;
			Vector2 rawMoveVector = GetRawMoveVector();
			if (Mathf.Approximately(rawMoveVector.x, 0.0f) && Mathf.Approximately(rawMoveVector.y, 0.0f)) {
				m_ConsecutiveMoveCount = 0;
				return false;
			}
			bool flag1 = input.GetButtonDown(m_HorizontalAxis) || input.GetButtonDown(m_VerticalAxis);
			bool flag2 = Vector2.Dot(rawMoveVector, m_LastMoveVector) > 0.0;
			if (!flag1)
				flag1 = !flag2 || m_ConsecutiveMoveCount != 1 ? unscaledTime > m_PrevActionTime + 1.0 / m_InputActionsPerSecond : unscaledTime > m_PrevActionTime + (double) m_RepeatDelay;
			if (!flag1)
				return false;
			AxisEventData axisEventData = GetAxisEventData(rawMoveVector.x, rawMoveVector.y, 0.6f);
			if (axisEventData.moveDir != MoveDirection.None) {
				ExecuteEvents.Execute<IMoveHandler>(eventSystem.currentSelectedGameObject, axisEventData, ExecuteEvents.moveHandler);
				if (!flag2)
					m_ConsecutiveMoveCount = 0;
				++m_ConsecutiveMoveCount;
				m_PrevActionTime = unscaledTime;
				m_LastMoveVector = rawMoveVector;
			}
			else
				m_ConsecutiveMoveCount = 0;
			return axisEventData.used;
		}

		/// <summary>
		///   <para>Iterate through all the different mouse events.</para>
		/// </summary>
		/// <param name="id">The mouse pointer Event data id to get.</param>
		protected void ProcessMouseEvent() {
			ProcessMouseEvent(0);
		}

		[Obsolete("This method is no longer checked, overriding it with return true does nothing!")]
		protected virtual bool ForceAutoSelect() {
			return false;
		}

		/// <summary>
		///   <para>Iterate through all the different mouse events.</para>
		/// </summary>
		/// <param name="id">The mouse pointer Event data id to get.</param>
		protected void ProcessMouseEvent(int id) {
			MouseState pointerEventData = GetMousePointerEventData(id);
			MouseButtonEventData eventData = pointerEventData.GetButtonState(PointerEventData.InputButton.Left).eventData;
			m_CurrentFocusedGameObject = eventData.buttonData.pointerCurrentRaycast.gameObject;
			ProcessMousePress(eventData);
			ProcessMove(eventData.buttonData);
			ProcessDrag(eventData.buttonData);
			ProcessMousePress(pointerEventData.GetButtonState(PointerEventData.InputButton.Right).eventData);
			ProcessDrag(pointerEventData.GetButtonState(PointerEventData.InputButton.Right).eventData.buttonData);
			ProcessMousePress(pointerEventData.GetButtonState(PointerEventData.InputButton.Middle).eventData);
			ProcessDrag(pointerEventData.GetButtonState(PointerEventData.InputButton.Middle).eventData.buttonData);
			if (Mathf.Approximately(eventData.buttonData.scrollDelta.sqrMagnitude, 0.0f))
				return;
			ExecuteEvents.ExecuteHierarchy<IScrollHandler>(ExecuteEvents.GetEventHandler<IScrollHandler>(eventData.buttonData.pointerCurrentRaycast.gameObject), eventData.buttonData, ExecuteEvents.scrollHandler);
		}

		/// <summary>
		///   <para>Send a update event to the currently selected object.</para>
		/// </summary>
		/// <returns>
		///   <para>If the update event was used by the selected object.</para>
		/// </returns>
		protected bool SendUpdateEventToSelectedObject() {
			if (eventSystem.currentSelectedGameObject == null)
				return false;
			BaseEventData baseEventData = GetBaseEventData();
			ExecuteEvents.Execute<IUpdateSelectedHandler>(eventSystem.currentSelectedGameObject, baseEventData, ExecuteEvents.updateSelectedHandler);
			return baseEventData.used;
		}

		protected void ProcessMousePress(MouseButtonEventData data) {
			PointerEventData buttonData = data.buttonData;
			GameObject gameObject1 = buttonData.pointerCurrentRaycast.gameObject;
			if (data.PressedThisFrame())  {
				buttonData.eligibleForClick = true;
				buttonData.delta = Vector2.zero;
				buttonData.dragging = false;
				buttonData.useDragThreshold = true;
				buttonData.pressPosition = buttonData.position;
				buttonData.pointerPressRaycast = buttonData.pointerCurrentRaycast;
//				DeselectIfSelectionChanged(gameObject1, buttonData);
				GameObject gameObject2 = ExecuteEvents.ExecuteHierarchy<IPointerDownHandler>(gameObject1, buttonData, ExecuteEvents.pointerDownHandler);
				if (gameObject2 == null)
					gameObject2 = ExecuteEvents.GetEventHandler<IPointerClickHandler>(gameObject1);
				float unscaledTime = Time.unscaledTime;
				if (gameObject2 == buttonData.lastPress) {
					if (unscaledTime - buttonData.clickTime < 0.300000011920929)
						++buttonData.clickCount;
					else
						buttonData.clickCount = 1;
					buttonData.clickTime = unscaledTime;
				}
				else
					buttonData.clickCount = 1;
				buttonData.pointerPress = gameObject2;
				buttonData.rawPointerPress = gameObject1;
				buttonData.clickTime = unscaledTime;
				buttonData.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(gameObject1);
				if (buttonData.pointerDrag != null)
					ExecuteEvents.Execute<IInitializePotentialDragHandler>(buttonData.pointerDrag, buttonData, ExecuteEvents.initializePotentialDrag);
				m_InputPointerEvent = buttonData;
			}
			if (!data.ReleasedThisFrame())
				return;
			ExecuteEvents.Execute<IPointerUpHandler>(buttonData.pointerPress, buttonData, ExecuteEvents.pointerUpHandler);
			GameObject eventHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(gameObject1);
			if (buttonData.pointerPress == eventHandler && buttonData.eligibleForClick)
				ExecuteEvents.Execute<IPointerClickHandler>(buttonData.pointerPress, buttonData, ExecuteEvents.pointerClickHandler);
			else if (buttonData.pointerDrag != null && buttonData.dragging)
				ExecuteEvents.ExecuteHierarchy<IDropHandler>(gameObject1, buttonData, ExecuteEvents.dropHandler);
			buttonData.eligibleForClick = false;
			buttonData.pointerPress = null;
			buttonData.rawPointerPress = null;
			if (buttonData.pointerDrag != null && buttonData.dragging)
				ExecuteEvents.Execute<IEndDragHandler>(buttonData.pointerDrag, buttonData, ExecuteEvents.endDragHandler);
			buttonData.dragging = false;
			buttonData.pointerDrag = null;
			if (gameObject1 != buttonData.pointerEnter)  {
				HandlePointerExitAndEnter(buttonData, null);
				HandlePointerExitAndEnter(buttonData, gameObject1);
			}
			m_InputPointerEvent = buttonData;
		}

		protected GameObject GetCurrentFocusedGameObject()  {
			return m_CurrentFocusedGameObject;
		}

		[Obsolete("Mode is no longer needed on input module as it handles both mouse and keyboard simultaneously.", false)]
		public enum InputMode {
			Mouse,
			Buttons,
		}
	}
}
