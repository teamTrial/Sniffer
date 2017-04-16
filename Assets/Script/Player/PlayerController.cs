using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;

public class PlayerController : MonoBehaviour {

    private Vector2 Center;
    public GameObject Player;
    public static bool CenterFlag { get; private set; }
    /// <summary>
    /// 右向きtrue;左向きfalse
    /// </summary>
    public static bool PlayerDirectoin { get; private set; }
    public static bool BattleFlag { get; private set; }
    public static bool attackFlag { get; private set; }
    public static bool SnifferActionFlag { get; private set; }

    private float SnifferActionTime = 5;
    Vector2 dis;

    public float controllerThreshold = 1.6f;

    private Button center;
    float LongTap = 0;
    Animator anim;
    float OldCameraSize;
    void Start () {
        center = GameObject.Find ("UI/center").GetComponent<Button> ();
        OldCameraSize = Camera.main.orthographicSize;
        Attack ();
        PlayerDirectoin = true;
        CenterFlag = false;
        anim = Player.GetComponent<Animator> ();
#if UNITY_EDITOR
        // SnifferActionTime=100;
        // SnifferAction();
#endif
        GetDistancefromPovittoFinger ();

    }
    public void EndBattle (int cooltime = 20) {
        if (BattleFlag) {
            BattleFlag = false;
            PlayerDB.Instance.Heel (cooltime);
        }
    }
    /// <summary>
    /// 移動処理
    /// 中心地から指の距離を毎フレーム算出する
    /// </summary>
    void GetDistancefromPovittoFinger () {
        this.UpdateAsObservable ()
            .TakeWhile (clearFlag => !StageClear.ClearFlag||!police.PoliceWinFlag)
            .Where (distance => Input.GetMouseButton (0))
            .Where(PoliceButtle=>!police.BattleFlag)
            .Select (distance => {
                Center = GetPovit ();
                return FingerPos (Input.mousePosition);
            })
            .Subscribe (distance => {
                dis = new Vector2 (distance.x - Center.x, 0);
                float NewCameraSize = Camera.main.orthographicSize;
                //コントローラーのしきい値
                if (!(-controllerThreshold * (NewCameraSize / OldCameraSize) < dis.x && dis.x < controllerThreshold * (NewCameraSize / OldCameraSize)) && !Actoin.attackFlag) {
                    Direction (dis);
                    Walk (dis * Time.deltaTime);
                    CenterFlag = false;
                    EndBattle ();
                    return;
                } else if (-1.6f < dis.x && dis.x < 1.6f) {
                    CenterFlag = true;
                    setAnimation (0);
                }
            });
    }
    /// <summary>
    /// 画像の中心地を取得する
    /// 初回のみ取得
    /// </summary>
    /// <returns>
    /// 画像のワールド座標(Povit)
    /// </returns>
    Vector2 GetPovit () {
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
    Vector2 FingerPos (Vector2 fingerpos) {
        fingerpos = Camera.main.ScreenToWorldPoint (fingerpos);
        //1-------------------------------------------------------------
        var Height = GetComponent<RectTransform> ().sizeDelta.y;
        if (Height < Input.mousePosition.y) {
            return Center;
        }
        //--------------------------------------------------------------
        return fingerpos;
    }
    /// <summary>
    /// Characterの向きを反転させる
    /// </summary>
    /// <param name="dis">
    /// 指と中央の差分
    /// </param>
    void Direction (Vector2 dis) {
        var x = Player.transform.localScale.x;
        //右向き
        if (0 < dis.x) {
            PlayerDirectoin = true;
            x = Mathf.Abs (Player.transform.localScale.x);
        }
        //左向き
        else if (dis.x < 0) {
            PlayerDirectoin = false;
            x = -Mathf.Abs (Player.transform.localScale.x);
        }
        Player.transform.localScale = new Vector3 (
            x,
            Player.transform.localScale.y,
            Player.transform.localScale.z
        );
    }
    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        if(Input.GetMouseButtonUp (0)){
            setAnimation(0);
        }
    }
    public void StartBattle () {
        BattleFlag = true;
    }
    void Walk (Vector3 walk) {
        if(StageClear.ClearFlag){
            return;
        }
        Player.transform.Translate (walk);
        setAnimation (1);
    }
    void Attack () {
        var doubleclick = center.onClick.AsObservable ();
        doubleclick
            .Where (attack => !PlayerController.BattleFlag)
            .Where(PoliceButtle=>!police.BattleFlag)
            // 0.2秒以内のメッセージをまとめる
            .Buffer (doubleclick.Throttle (TimeSpan.FromMilliseconds (300)))
            // タップ回数が2回以上だったら処理する
            .Where (tap => tap.Count >= 2)
            .Subscribe (tap => {
                attackFlag = true;
                setAnimation (3);
            });

        var longtap = this.UpdateAsObservable ();
        longtap
            .Where (Center => PlayerController.CenterFlag)
            .Where (SnifferFlag => !SnifferActionFlag)
            .Where (attack => !attackFlag)
            .Where(PoliceButtle=>!police.BattleFlag)
            .Subscribe (_ => {
                if (Input.GetMouseButton (0)) {
                    LongTap += Time.deltaTime;
                }
                if (Input.GetMouseButtonUp (0)) {
                    if (LongTap > 1.0f) {
                        print ("Snifferアクション");
                        setAnimation (2);
                        SnifferAction ();
                    }
                    LongTap = 0;
                    if (anim.GetInteger ("Anim") < 2) {
                        setAnimation (0);
                    }
                }
            });
    }
    /// <summary>
    /// アニメーションの設定
    /// </summary>
    /// <param name="animnum">0=止まる,1=歩く,2=sniff,3=attack</param>
    void setAnimation (int animnum = 0) {
        anim = Player.GetComponent<Animator> ();
        anim.SetInteger ("Anim", animnum);
    }
    /// <summary>
    /// 敵の状態を可視化できるようにする
    /// </summary>
    void SnifferAction () {
        SnifferActionFlag = true;
        var MainCamera = GameObject.Find ("Main Camera").GetComponent<Grayscale> ();
        MainCamera.enabled = true;
        Observable.Timer (
                TimeSpan.FromSeconds (SnifferActionTime))
            .Subscribe (_ => {
                SnifferActionFlag = false;
                MainCamera.enabled = false;
            });
    }
    void LateUpdate () {
        checkAnim ();
    }
    void checkAnim () {
        anim = Player.GetComponent<Animator> ();
        AnimatorStateInfo animInfo = anim.GetCurrentAnimatorStateInfo (0);
        if (1.0f < animInfo.normalizedTime) {
            //アニメーションが止まると移動以外だった場合
            if (1 < anim.GetInteger ("Anim")) {
                setAnimation (0);
                attackFlag = false;
            }
        }
    }
}