using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UnityEngine.UI;

public class Camera2DFollow : MonoBehaviour {
    public Transform target;
    public float damping = 0.2f;
    public float lookAheadFactor = 3;
    public float lookAheadReturnSpeed = 0.5f;
    public float lookAheadMoveThreshold = 0.1f;
    public float hight = 2f;

    private float offsetZ;
    private Vector3 lastTargetPosition;
    private Vector3 currentVelocity;
    private Vector3 lookAheadPos;

    void Start() {
        lastTargetPosition = target.position;
        offsetZ = ( transform.position - target.position ).z;
        transform.parent = null;
    }

    void Update() {

        float xMoveDelta = ( target.position - lastTargetPosition ).x;

        bool updateLookAheadTarget = Mathf.Abs(xMoveDelta) > lookAheadMoveThreshold;

        if ( updateLookAheadTarget ) {
            lookAheadPos = lookAheadFactor * Vector3.right * Mathf.Sign(xMoveDelta);
        } else {
            lookAheadPos = Vector3.MoveTowards(lookAheadPos , Vector3.zero , Time.deltaTime * lookAheadReturnSpeed);
        }
        //PlayerController.PlayerDirectoin;
        Vector3 aheadTargetPos = target.position + lookAheadPos + Vector3.forward * offsetZ;
        aheadTargetPos = new Vector3(aheadTargetPos.x , hight , aheadTargetPos.z);
        Vector3 thisPos = new Vector3(transform.position.x , hight , transform.position.z);
        Vector3 newPos = Vector3.SmoothDamp(thisPos, aheadTargetPos , ref currentVelocity , damping);
        
        transform.position = newPos;

        lastTargetPosition = target.position;
    }
}
