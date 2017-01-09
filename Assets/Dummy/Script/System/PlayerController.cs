using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

    private Vector2 getpovit;
    public GameObject Player;
	void Start () {
        getpovit = GetPovit( );
        GetDistancefromPovittoFinger( );
    }
    /// <summary>
    /// 中心地から指の距離を毎フレーム算出する
    /// </summary>
    void GetDistancefromPovittoFinger() {
        this.UpdateAsObservable( )
            .Where(distance=>Input.GetMouseButton(0))
            .Select(distance=>FingerPos(Input.mousePosition))
            .Subscribe(distance => {
                Vector2 dis = distance-getpovit;
                Player.transform.Translate(dis*Time.deltaTime);
            });
    }
    /// <summary>
    /// 画像の中心地を取得する
    /// 初回のみ取得
    /// </summary>
    /// <returns>
    /// 画像のワールド座標(Povit)
    /// </returns>
    Vector2 GetPovit() {
        var getpovit = this.transform.position;
        return getpovit;
    }
    /// <summary>
    /// 指の位置を算出する
    /// </summary>
    /// <param name="fingerpos">
    /// タッチしたスクリーン座標
    /// </param>
    /// <returns>
    /// 指のワールド座標(Vector2)
    /// </returns>
    Vector2 FingerPos(Vector2 fingerpos) {
        fingerpos = Camera.main.ScreenToWorldPoint(fingerpos);
        return fingerpos;
    }
}
