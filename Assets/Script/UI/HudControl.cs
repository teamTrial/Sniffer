using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// /// uGUIで3D空間のオブジェクト上に追従するHUD制御用コンポーネント
/// </summary>
public class HudControl : MonoBehaviour {
    public Transform targetTrans;
    public Vector3 offset = Vector3.zero;

    RectTransform myRectTrans;
    RectTransform parentRectTrans;
    Camera uiCamera;

    void Awake () {
        myRectTrans = GetComponent<RectTransform> ();
        parentRectTrans = (RectTransform) myRectTrans.parent;

        var canvasArr = GetComponentsInParent<Canvas> ();
        for (int i = 0; i < canvasArr.Length; i++) {
            if (canvasArr[i].isRootCanvas) {
                uiCamera = canvasArr[i].worldCamera;
            }
        }
    }

    void Update () {
        if (targetTrans != null && myRectTrans != null && parentRectTrans != null && uiCamera != null) {
            UpdateUiLocalPosFromTargetPos ();
        }
    }

    void UpdateUiLocalPosFromTargetPos () {
        var screenPos = Camera.main.WorldToScreenPoint (targetTrans.position + offset);
        var localPos = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle (parentRectTrans, screenPos, uiCamera, out localPos);
        myRectTrans.localPosition = localPos;
    }
}