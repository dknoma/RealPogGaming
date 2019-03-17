using UnityEngine;

public class ObjectValues {

    public string name;
    public float height;
    public bool determinePriortyWithHeight;
    public bool fixedHeight;
    public bool fixedSorting;
    public Vector3 position;
    public float h;
    public float w;
    public float topBound;
    public float bottomBound;
    public float rightBound;
    public float leftBound;
    public float baseTopBound;
    public float baseBottomBound;
    public float baseRightBound;
    public float baseLeftBound;
//    public float distance;
    public int sortingOrder;

    public ObjectValues(Vector3 pos, float h, float w, float height, int order, string name) {
        position = pos;
        this.h = h;
        this.w = w;
        this.height = height;
        topBound = position.y + h / 2;
        bottomBound = position.y - h / 2;
        rightBound = position.x + w / 2;
        leftBound = position.x - w / 2;
        sortingOrder = order;
        this.name = name;
        baseTopBound = topBound - height;
        baseBottomBound = bottomBound - height;
        baseRightBound = rightBound;
        baseLeftBound = leftBound;
    }
}
