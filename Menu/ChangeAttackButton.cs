using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChangeAttackButton : EventTrigger {

//    private GameObject originalOption;
//    private TextMeshProUGUI text;
//    private Vector3 origTextPos;
    private EventSystem eventSystem;

    private TmpButton button;
//    private EventTrigger asd;

//    private void OnEnable() {
//        eventSystem = EventSystem.current;
////        originalOption = eventSystem.firstSelectedGameObject;
////        origTextPos = originalOption.GetComponentInChildren<TextMeshProUGUI>().transform.position;
//////        OnCurrentEventObjectChange.
////        Debug.LogFormat("\t\torig SET {0}", originalOption);
//    }
//
//    private void Update() {
////        Debug.LogFormat("orig:\t\t\t{0}", originalOption.name);
//        Debug.LogFormat("currentSelectedGameObject:\t{0}", eventSystem.currentSelectedGameObject);
////        Debug.LogFormat("equal?:\t{0}", eventSystem.currentSelectedGameObject == originalOption);
////        if (eventSystem.currentSelectedGameObject != originalOption && OnCurrentEventObjectChange != null) {
////            GameObject newObj = eventSystem.currentSelectedGameObject;
////            originalOption = newObj;
////            OnCurrentEventObjectChange(newObj);
////            Debug.LogFormat("\t\tChanging option to {0}", newObj.name);
////        }
//    }
//
//    public void CurrentObjectChange() {
//        Debug.LogFormat("\t\tChanging option to {0}", eventSystem.currentSelectedGameObject);
////        if (eventSystem.currentSelectedGameObject != originalOption) {
////            GameObject newObj = eventSystem.currentSelectedGameObject;
////            originalOption = newObj;
////            Debug.LogFormat("\t\tChanging option to {0}", newObj.name);
////        }
//    }
//    public override void OnUpdateSelected(BaseEventData eventData) {
//        Debug.Log("Updating selected...");
//    }

//    public override void OnMove(AxisEventData data) {
//        Debug.Log("OnMove called.");
//    }
//    
//    public override void OnSelect(BaseEventData data)
//    {
//        Debug.Log("OnSelect called.");
//    }
//
//    public override void OnSubmit(BaseEventData data)
//    {
//        Debug.Log("OnSubmit called.");
//    }

    private void Awake() {
        button = GetComponent<TmpButton>();
//        text = GetComponentInChildren<TextMeshProUGUI>();
//        origTextPos = text.transform.position;
//        Debug.LogFormat("Text pos : {0}", origTextPos);
    }

    public override void OnBeginDrag(PointerEventData data) {
        Debug.Log("OnBeginDrag called.");
    }

    public override void OnCancel(BaseEventData data) {
        Debug.Log("OnCancel called.");
    }


    public override void OnDrag(PointerEventData data) {
        Debug.Log("OnDrag called.");
    }

    public override void OnDrop(PointerEventData data) {
        Debug.Log("OnDrop called.");
    }

    public override void OnEndDrag(PointerEventData data) {
        Debug.Log("OnEndDrag called.");
    }

    public override void OnInitializePotentialDrag(PointerEventData data) {
        Debug.Log("OnInitializePotentialDrag called.");
    }

    public override void OnMove(AxisEventData data) {
        Debug.Log("OnMove called.");
    }

    public override void OnPointerClick(PointerEventData data) {
        Debug.Log("OnPointerClick called.");
    }

    public override void OnPointerDown(PointerEventData data) {
        Debug.Log("OnPointerDown called.");
    }

    public override void OnPointerEnter(PointerEventData data) {
        Debug.Log("OnPointerEnter called.");
    }

    public override void OnPointerExit(PointerEventData data) {
        Debug.Log("OnPointerExit called.");
    }

    public override void OnPointerUp(PointerEventData data) {
        Debug.Log("OnPointerUp called.");
    }

    public override void OnScroll(PointerEventData data) {
        Debug.Log("OnScroll called.");
    }

    public override void OnSelect(BaseEventData data) {
//        Debug.Log("OnSelect called.");
//        Vector3 pos = text.transform.position;
//        text.transform.position = new Vector3(pos.x - 0.25f, pos.y + 0.25f, pos.z);
//        Debug.LogFormat("OnSelect: {0} text pos : {1}", name, text.transform.position);
    }

    public override void OnDeselect(BaseEventData data) {
//        Debug.Log("OnDeselect called.");
//        Vector3 pos = text.transform.position;
//        text.transform.position = new Vector3(pos.x + 0.25f, pos.y - 0.25f, pos.z);
//        text.transform.position = origTextPos;
//        Debug.LogFormat("OnDeselect: {0} text pos: {1}", name, text.transform.position);
    }
    
    public override void OnSubmit(BaseEventData data) {
        Debug.LogFormat("OnSubmit: {0}", name);;
        
//        gameObject.SetActive(false);
//        StartCoroutine(MoveText());
//        data.currentInputModule.
    }

//    private IEnumerator MoveText() {
////        Image buttonImage = GetComponent<Image>();
////        Button button = GetComponent<Button>();
//        Vector3 position = text.transform.position;
//        Vector3 pos = position;
////        text.transform.position = new Vector3(pos.x + 0.25f, pos.y - 0.25f, pos.z);
////        yield return new WaitForSeconds(0.45f);
//////        yield return new WaitUntil(() => buttonImage.sprite == button.spriteState.highlightedSprite);
////        pos = text.transform.position;
////        text.transform.position = new Vector3(pos.x - 0.25f, pos.y + 0.25f, pos.z);
////        text.transform.position = position;
//    }

//    protected void UpdateSelectionState (BaseEventData eventData) {
//        var beforeState = currentSelectionState;
//        base.UpdateSelectionState(eventData);
//        if (currentSelectionState != beforeState)
//            DoCallback ();
//    }
    
    public override void OnUpdateSelected(BaseEventData data) {
//        Debug.Log("OnUpdateSelected called.");
//        Debug.LogFormat("OnUpdateSelected called: {0}", "");
    }
}