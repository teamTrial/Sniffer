using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// NPC(襲われる系)の機能を実装
/// </summary>
public class people : human {
    protected int NPCHP () {
        return Random.Range (HP - 5, HP + 3);
    }

    /// <summary>
    /// NPCの視線
    /// </summary>
    public override void EyeLine (Collider2D other) {
        //レイヤーマスク作成
        RaycastHit2D hit = HitObj ();
        //なにかと衝突した時だけそのオブジェクトの名前をログに出す
        if (hit.collider) {
            if (hit.collider.tag == "hand") {
                Escape ();
            } else if (hit.collider.tag == "Player") {
                if (PlayerController.BattleFlag) {
                    if (speed != 0) {
                        Escape ();
                    }
                }
            }
        }
    }
    //NPCが持ってる
    public void Escape (float EscapeSpeed = 2) {
        this.transform.localScale = new Vector2 (-this.transform.localScale.x, this.transform.localScale.y);
        this.GetComponent<people> ().speed = EscapeSpeed;
    }
}