using UnityEngine;

public class ObjectValues {

    public string name;
    public float height;
    public Vector3 position;
    public int sortingOrder;
//    public float h;
//    public float w;
//    public float topBound;
//    public float bottomBound;
//    public float rightBound;
//    public float leftBound;
//    public float baseTopBound;
//    public float baseBottomBound;
//    public float baseRightBound;
//    public float baseLeftBound;

    public ObjectValues(string name, float height, Vector3 pos, int order) {
        this.name = name;
        this.height = height;
        position = pos;
        sortingOrder = order;
//        this.h = h;
//        this.w = w;
//        topBound = position.y + h / 2;
//        bottomBound = position.y - h / 2;
//        rightBound = position.x + w / 2;
//        leftBound = position.x - w / 2;
//        baseTopBound = topBound - height;
//        baseBottomBound = bottomBound - height;
//        baseRightBound = rightBound;
//        baseLeftBound = leftBound;
    }
}
