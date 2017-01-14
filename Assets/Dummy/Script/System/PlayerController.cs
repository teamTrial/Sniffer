using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

    private Vector2 Center;
    public GameObject Player;
    /// <summary>
    /// 右向きtrue;左向きfalse
    /// </summary>
    public static bool PlayerDirectoin { get; private set; }
	void Start () {
        PlayerDirectoin = true;
        GetDistancefromPovittoFinger( );
    }
    /// <summary>
    /// 中心地から指の距離を毎フレーム算出する
    /// </summary>
    void GetDistancefromPovittoFinger() {
        this.UpdateAsObservable( )
            .Where(distance=>Input.GetMouseButton(0))
            .Select(distance=> {
                Center = GetPovit( );
                return FingerPos(Input.mousePosition);
                })
            .Subscribe(distance => {
                Vector2 dis = new Vector2(distance.x-Center.x,0);
                Direction(dis);
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
    /// 1.指の位置がUI/Controllerオブジェクトの高さ以上だった場合現在のCenterをreturnする
    /// </summary>
    /// <param name="fingerpos">
    /// タッチしたスクリーン座標
    /// </param>
    /// <returns>
    /// 指のワールド座標(Vector2)
    /// </returns>
    Vector2 FingerPos(Vector2 fingerpos) {
        fingerpos = Camera.main.ScreenToWorldPoint(fingerpos);
        //1-------------------------------------------------------------
        var Height = GetComponent<RectTransform>( ).sizeDelta.y;
        if ( Height < Input.mousePosition.y ) 
            return Center;
        //--------------------------------------------------------------
        return fingerpos;
    }
    /// <summary>
    /// Characterの向きを反転させる
    /// </summary>
    /// <param name="dis">
    /// 指と中央の差分
    /// </param>
    void Direction(Vector2 dis) {
        var x = Player.transform.localScale.x;
        //右向き
        if ( 0<dis.x  ) {
            PlayerDirectoin = true;
            x = Mathf.Abs(Player.transform.localScale.x);
        }
        //左向き
        else {
            PlayerDirectoin = false;
            x = -Mathf.Abs(Player.transform.localScale.x);
        }
        Player.transform.localScale = new Vector3(
            x,
            Player.transform.localScale.y,
            Player.transform.localScale.z);
    }
}
