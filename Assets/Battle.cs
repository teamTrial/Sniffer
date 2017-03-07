using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
public class Battle : MonoBehaviour {
    // public int HP = 10;
    // public bool 
    PlayerController playercontroller;
    PlayerStatus playerstatus;
    public float CameraAnimationTime;
    void Start () {
        playercontroller = GameObject.Find ("UI/Controller").GetComponent<PlayerController> ();
        playerstatus = GetComponent<PlayerStatus> ();
    }
    public void StartBattle (GameObject NPC, int HP = 10) {
        // タップダウンストリームを作成
        var tapDownStream = this.UpdateAsObservable ()
            .Where (_ => Input.GetMouseButtonDown (0));
        tapDownStream
            .Select (_ => 1)
            .TakeWhile (NotCentor => PlayerController.BattleFlag)
            .Scan ((sum, addCount) => sum + addCount)
            .Do (totalCount => playerstatus.Damage (NPC.name,HP-totalCount))
            .Where (totalCount => HP < totalCount)
            .Subscribe (totalCount => {
                var Player = GameObject.FindGameObjectWithTag ("Player");
                ChangeController (NPC, Player);
            }).AddTo (this.gameObject);
        //ズームの時間を取得
       AnimatorStateInfo cameraAnim = Camera.main.GetComponent<Animator> ().GetCurrentAnimatorStateInfo (0);
        CameraAnimationTime = cameraAnim.length + 2;
        //タイムアップが来たら
        Observable.Timer (TimeSpan.FromSeconds (CameraAnimationTime)).Subscribe (_ => {
            playercontroller.EndBattle ();
        });
    }
    void ChangeController (GameObject NPC, GameObject Player) {
        playercontroller.EndBattle ();
        //カメラワークと操作権をNPCに移す
        GameObject.Find ("Main Camera").GetComponent<CameraMove> ().target = NPC.transform;
        GameObject.Find ("UI/Controller").GetComponent<PlayerController> ().Player = NPC;
        NPC.tag = "controller";
        NPC.layer = LayerMask.NameToLayer ("Highlight");
        NPC.GetComponent<people> ().enabled = false;;

        // NPC.AddComponent<Actoin> ();

        //元プレイヤーをNPCのAIを導入する
        //3/7 プレイヤーのジャンルに分けてAddComponentするコードを変更した方がよい
        Player.AddComponent<people> ();
        Player.GetComponent<EnemyRay> ().Escape ();
        NPC.layer = LayerMask.NameToLayer ("Default");
        Player.tag = "enemy";

    }
}