using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PinchinPinchOut : MonoBehaviour {

    private float scale;
    private float backDist = 0.0f;

    // デバッグ用テキスト（scaleサイズ確認用）
    public Text scaleText;

    // デフォルトサイズ
    private float defaultScale;

    void Start () {
        // 初期値取得
        RectTransform rt = this.GetComponent(typeof (RectTransform)) as RectTransform;
        defaultScale = rt.sizeDelta.x;
    }

    void Update () {

        // マルチタッチ判定
        if (Input.touchCount >= 2) {

            // タッチ開始時に初期値を取得
            RectTransform rt = this.GetComponent(typeof (RectTransform)) as RectTransform;
            defaultScale = rt.sizeDelta.x;

            // タッチしている２点を取得
            Touch touch1 = Input.GetTouch (0);
            Touch touch2 = Input.GetTouch (1);

            // 2点タッチ開始時の距離を記憶
            if (touch2.phase == TouchPhase.Began) {
                backDist = Vector2.Distance (touch1.position, touch2.position);
            } else if (touch1.phase == TouchPhase.Moved && touch2.phase == TouchPhase.Moved) {
                // タッチ位置の移動後、長さを再測し、前回の距離からの相対値を取る。
                float newDist = Vector2.Distance (touch1.position, touch2.position);
                scale = defaultScale + (newDist - backDist) / 100.0f;
                scaleText.text = "scalse = " + scale.ToString();

                // 相対値が変更した場合、オブジェクトに相対値を反映させる
                if(scale != 0) {
                    UpdateScaling(scale);  
                }
            }
        }
    }

    /// 設定された拡大率に基づいてオブジェクトの大きさを更新する
    private void UpdateScaling(float argScale) {
        RectTransform rt = this.GetComponent(typeof (RectTransform)) as RectTransform;
        rt.sizeDelta = new Vector2 (argScale, argScale);
    }
}