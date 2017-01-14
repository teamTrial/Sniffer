using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour {
    public Transform target;
    public float damping = 0.2f;
    public float lookAheadFactor = 0;
    public float lookAheadReturnSpeed = 0;
    public float lookAheadMoveThreshold = 0;
    public float hight = 2f;
    public GameObject background;

    private float offsetZ;
    private Vector3 lastTargetPosition;
    private Vector3 currentVelocity;
    private Vector3 lookAheadPos;

    //カメラの表示領域表示用
    private Vector3 cameraBottomLeft;
    private Vector3 cameraTopLeft;
    private Vector3 cameraBottomRight;
    private Vector3 cameraTopRight;
    public float cameraRangeWidth;
    public float cameraRangeHeight;
    private StageController stageController;

    void Start() {
        lastTargetPosition = target.position;
        offsetZ = ( transform.position - target.position ).z;
        transform.parent = null;
        stageController=GetComponent<StageController>( );
    }


    //カメラの表示領域を緑ラインで表示
    void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(cameraBottomLeft , cameraTopLeft);
        Gizmos.DrawLine(cameraTopLeft , cameraTopRight);
        Gizmos.DrawLine(cameraTopRight , cameraBottomRight);
        Gizmos.DrawLine(cameraBottomRight , cameraBottomLeft);
    }
    float distance = -10f;
    void Update() {

        cameraBottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0 , 0 , distance));
        cameraTopRight = Camera.main.ViewportToWorldPoint(new Vector3(1 , 1 , distance));
        cameraTopLeft = new Vector3(cameraBottomLeft.x , cameraTopRight.y , cameraBottomLeft.z);
        cameraBottomRight = new Vector3(cameraTopRight.x , cameraBottomLeft.y , cameraTopRight.z);
        cameraRangeWidth = Vector3.Distance(cameraBottomLeft , cameraBottomRight);
        cameraRangeHeight = Vector3.Distance(cameraBottomLeft , cameraTopLeft);

        print(cameraRangeHeight);

        float xMoveDelta = ( target.position - lastTargetPosition ).x;

        bool updateLookAheadTarget = Mathf.Abs(xMoveDelta) > lookAheadMoveThreshold;

        if ( updateLookAheadTarget ) {
            lookAheadPos = lookAheadFactor * Vector3.right * Mathf.Sign(xMoveDelta);
        } else {
            lookAheadPos = Vector3.MoveTowards(lookAheadPos , Vector3.zero , Time.deltaTime * lookAheadReturnSpeed);
        }
        Vector3 aheadTargetPos = target.position + lookAheadPos + Vector3.forward * offsetZ;
        var offset = 1f;
        if ( PlayerController.PlayerDirectoin ) {
            aheadTargetPos = new Vector3(aheadTargetPos.x+ offset , hight , aheadTargetPos.z);
        } else {
            aheadTargetPos = new Vector3(aheadTargetPos.x- offset , hight , aheadTargetPos.z);
        }
        Vector3 thisPos = new Vector3(transform.position.x , hight , transform.position.z);
        Vector3 newPos = Vector3.SmoothDamp(thisPos , aheadTargetPos , ref currentVelocity , damping);
        //カメラの稼働領域をステージ領域に制限
       var newX = Mathf.Clamp(newPos.x , stageController.StageRect.xMin + cameraRangeWidth / 2 , stageController.StageRect.xMax - cameraRangeWidth / 2);
       var newY = Mathf.Clamp(newPos.y , 0 , stageController.StageRect.yMax - cameraRangeHeight / 2);
        newPos = new Vector3(newX , newY , this.transform.position.z);
        transform.position = newPos;

        lastTargetPosition = target.position;
    }
    /// <summary>
    /// ステージの端か否かを判定する
    /// </summary>
    /// <returns>
    /// true=端;false=端ではない
    /// </returns>
    bool StageEnd() {
        print(background.GetComponent<SpriteRenderer>( ).bounds.size.y);
        print("ScreenSize" + Camera.main.ScreenToWorldPoint(new Vector3(Screen.width , Screen.height , 0)).x);
        return true;
    }
}
