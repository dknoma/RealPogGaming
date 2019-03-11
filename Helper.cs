using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helper {

    public static T FindComponentInChildWithTag<T>(this GameObject parent, string tag)where T:Component{
        Transform t = parent.transform;
        foreach(Transform tr in t) {
            if(tr.CompareTag(tag)) {
                return tr.GetComponent<T>();
            }
        }
        return null;
    }
    
    public static T FindComponentInSiblingsWithTag<T>(this GameObject me, string tag)where T:Component{
        Transform parent = me.transform.parent;
        foreach(Transform sib in parent) {
            if(sib.CompareTag(tag) && !sib.Equals(me.transform)) {
                return sib.GetComponent<T>();
            }
        }
        return null;
    }
}
