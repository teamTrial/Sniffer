using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
public class Battle : MonoBehaviour {
    PlayerController playercontroller;
    public float CameraAnimationTime;
    void Start () {
        playercontroller = GameObject.Find ("UI/Controller").GetComponent<PlayerController> ();
    }
    public void StartBattle (GameObject NPC, int HP = 10,String Enemytag="") {
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
                PlayerDB.Instance.Damage (NPC.name, EnemyHP);
            })
            .Where (totalCount => HP < totalCount)
            .Subscribe (totalCount => {
                var Player = GameObject.FindGameObjectWithTag ("Player");
                ChangeController (NPC, Player,Enemytag);
            }).AddTo (this.gameObject);
        //ズームの時間を取得
        AnimatorStateInfo cameraAnim = Camera.main.GetComponent<Animator> ().GetCurrentAnimatorStateInfo (0);
        CameraAnimationTime = cameraAnim.length + 2;
        //タイムアップが来たら
        Observable.Timer (TimeSpan.FromSeconds (CameraAnimationTime)).Subscribe (_ => {
            EnemyStatusDB.Instance.Enemy[NPC.name] = EnemyHP;
            playercontroller.EndBattle ();
            people NPCInfo=NPC.GetComponent<people> ();
            NPCInfo.HP = EnemyHP;
            NPCInfo.Escape();
            NPC.transform.FindChild ("HP").GetComponent<SniffeUI> ().UpdateSize ();
        });
    }
    void ChangeController (GameObject NPC, GameObject Player,String Enemytag) {
        playercontroller.EndBattle ();
        //カメラワークと操作権をNPCに移す
        GameObject.Find ("Main Camera").GetComponent<CameraMove> ().target = NPC.transform;
        NPC.GetComponent<Collider2D>().isTrigger=false;
        GameObject.Find ("UI/Controller").GetComponent<PlayerController> ().Player = NPC;
        NPC.tag = "Player";
        NPC.layer = LayerMask.NameToLayer ("Highlight");
        NPC.GetComponent<people> ().enabled = false;

        // NPC.AddComponent<Actoin> ();
        PlayerDB.Instance.getExp(Exp(Enemytag));
        //元プレイヤーをNPCのAIを導入する
        //　追記3/7 プレイヤーのジャンルに分けてAddComponentするコードを変更した方がよい
        var NPC_AI = Player.GetComponent<people> ();
        Player.GetComponent<Collider2D>().isTrigger=true;
        NPC_AI.enabled = true;
        NPC_AI.Escape ();
        Player.layer = LayerMask.NameToLayer ("Default");
        Player.tag = "enemy";
    }
    float Exp(String Enemytag){
        if(Enemytag=="Normal"){
            return 1f;
        }else{
            return 0.1f;
        }
    }
}