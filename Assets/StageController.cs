using UnityEngine;
using System.Collections;

public class StageController : MonoBehaviour {

    private Transform floor;
    public float StageHeight = 10f;//ステージの高さ（任意に設定可）

    public Rect StageRect;//ステージの範囲をRectで設定
    public Vector3 LowerLeft;
    public Vector3 UpperLeft;
    public Vector3 LowerRight;
    public Vector3 UpperRight;

    //ステージ範囲
    void OnDrawGizmos() {
        LowerLeft = new Vector3(StageRect.xMin , StageRect.yMax , 0);
        UpperLeft = new Vector3(StageRect.xMin , StageRect.yMin , 0);
        LowerRight = new Vector3(StageRect.xMax , StageRect.yMax , 0);
        UpperRight = new Vector3(StageRect.xMax , StageRect.yMin , 0);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(LowerLeft , UpperLeft);
        Gizmos.DrawLine(UpperLeft , UpperRight);
        Gizmos.DrawLine(UpperRight , LowerRight);
        Gizmos.DrawLine(LowerRight , LowerLeft);
    }

    void Start() {
        //地面を取得
        floor = GameObject.Find("Floor").transform;
        //地面のColliderを元にRectを設定
        Bounds floorBounds = floor.GetComponent<SpriteRenderer>( ).bounds;
        StageRect.xMin = floorBounds.min.x;
        StageRect.xMax = floorBounds.max.x;
        StageRect.yMin = floorBounds.min.y;
        StageRect.yMax = floorBounds.max.y + StageHeight;
    }

    void Update() {

    }

}