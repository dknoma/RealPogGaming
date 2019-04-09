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

    public static bool IsNull<T>(this T obj )where T:Component {
        return EqualityComparer<T>.Default.Equals(obj,default(T));
    }
    
    public static bool IsObjectNull<T>(T obj )  {
        return EqualityComparer<T>.Default.Equals(obj,default(T));
    }

    public static bool IsReallyNull<T>(this T obj) {
        return ReferenceEquals(obj, null);
    }
}
