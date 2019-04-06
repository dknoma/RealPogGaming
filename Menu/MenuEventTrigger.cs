using UnityEngine;
using UnityEngine.EventSystems;

public class MenuEventTrigger : MonoBehaviour {
    
    

    public virtual void OnUpdateSelected() {
        Debug.Log("OnUpdateSelected called.");
    }
}
