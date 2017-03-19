using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class police : people {
    new void Start () {
        base.Start ();
        HP = EnemyDB.Police;
        HP = NPCHP ();
        EnemyDB.EntryEnemy (this.gameObject.name, HP);
    }
    bool Awareness;
    GameObject Player;
    new void Update () {
        base.Update ();
        // if (!PlayerController.BattleFlag) {
        //     Walk ();
        //     if (count.countdowsflag) {
        //         count.counter += Time.deltaTime;
        //     }
        //     if (count.limit < count.counter) {
        //         EnemyDB.DeleteEnemy (this.name);
        //         Destroy (this.gameObject);
        //     }
        // } else {
        //     Awareness = false;
        // }
    }
    new void Walk () {
        print (Awareness);
        if (Awareness) {
            int direction = 1;
            direction = Dir ();
            this.transform.position = Player.transform.position * Time.deltaTime;
        } else {
            int direction = 1;
            direction = Dir ();
            this.transform.Translate (direction * speed * Time.deltaTime, 0, 0);
        }
    }
    new void OnTriggerEnter2D (Collider2D other) {
        if (other.tag == "MainCamera") {
            count.counter = 0;
            count.countdowsflag = false;
        }
        //snifferアクション時魂を表示する処理を記入
        if (other.tag == "controller") return;

        //至近距離でプレイヤーに接触した場合
        if (other.tag == "hand") {
            Awareness = true;
            Player = other.gameObject;
            Attack ();
        }
    }
    new  void OnTriggerStay2D (Collider2D other) {
        if (other.tag == "MainCamera") {
            EyeLine (other);
        }
    }
    new public void EyeLine (Collider2D other) {
        //レイヤーマスク作成
        RaycastHit2D hit = HitObj ();
        //なにかと衝突した時だけそのオブジェクトの名前をログに出す
        if (hit.collider) {
        print(hit.collider.name);
            
            if (hit.collider.tag == "hand") {
                Awareness = true;
                Player = other.gameObject;
                Attack ();
            }
            if (hit.collider.tag == "Player") {
                print ("攻撃喰らったわ");
            }
        }
    }
    /// <summary>
    /// 攻撃しているときにPlayerを見たら発砲するように
    /// </summary>
    void Attack () {
        print ("攻撃");
    }
}