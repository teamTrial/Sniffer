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
        int EnemyHP = HP;
        // タップダウンストリームを作成
        var tapDownStream = this.UpdateAsObservable ()
            .Where (_ => Input.GetMouseButtonDown (0));
        tapDownStream
            .Select (_ => 1)
            .TakeWhile (NotCentor => PlayerController.BattleFlag)
            .Scan ((sum, addCount) => sum + addCount)
            .Do (totalCount => {
                EnemyHP = HP - totalCount + 1;
                playerstatus.Damage (NPC.name, EnemyHP);
            })
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
            EnemyStatusDB.Instance.Enemy[NPC.name] = EnemyHP;
            playercontroller.EndBattle ();
            NPC.GetComponent<people> ().HP = EnemyHP;
            NPC.transform.FindChild ("HP").GetComponent<SniffeUI> ().UpdateSize ();
        });
    }
    void ChangeController (GameObject NPC, GameObject Player) {
        playercontroller.EndBattle ();
        //カメラワークと操作権をNPCに移す
        GameObject.Find ("Main Camera").GetComponent<CameraMove> ().target = NPC.transform;
        GameObject.Find ("UI/Controller").GetComponent<PlayerController> ().Player = NPC;
        NPC.tag = "Player";
        NPC.layer = LayerMask.NameToLayer ("Highlight");
        NPC.GetComponent<people> ().enabled = false;

        // NPC.AddComponent<Actoin> ();

        //元プレイヤーをNPCのAIを導入する
        //　追記3/7 プレイヤーのジャンルに分けてAddComponentするコードを変更した方がよい
        var NPC_AI = Player.GetComponent<people> ();
        NPC_AI.enabled = true;
        NPC_AI.Escape ();
        Player.layer = LayerMask.NameToLayer ("Default");
        Player.tag = "enemy";
    }
}