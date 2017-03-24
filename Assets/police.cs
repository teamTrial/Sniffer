using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class police : human, IAction {
    //プレイヤーとの距離
    float offset;
    float _offset=2f;
    new void Start () {
        base.Start ();
        offset=_offset;
        HP = EnemyDB.normalPeople;
        //テスト
        Action ();
    }

    // /// <summary>
    // /// NPCの視線
    // /// </summary>
    protected override void EyeLine (Collider2D other) {
        //レイヤーマスク作成
        RaycastHit2D hit = HitObj ();
        if (hit.collider) {
            //攻撃する瞬間を見たら
            if (hit.collider.tag == "hand") {
                LevelFive();
            } else if (hit.collider.tag == "Player") {
                if (PlayerController.BattleFlag) {
                    //speed=0　誰かが戦っているとき
                    if (speed != 0) {
                        LevelFive();
                        return;
                    }
                    Action();
                }
            }
        }
    }
    /// <summary>
    /// 認知度が高かった時にプレイヤーを見つけたら
    /// </summary>
    public override void Action () {
        LevelFive();
    }
    void LevetOne () {

    }
    void LevelTwo () {

    }
    void LevelThree () {

    }
    void LevelFour () {

    }
    void LevelFive () {
        var Player = GameObject.FindGameObjectWithTag ("Player");
        //認知度マックス　遠距離攻撃とかする
        this.UpdateAsObservable ()
            .Subscribe (_ => {
                var EnemyPos = Player.transform.position;
                EnemyPos.x = Player.transform.position.x + offset;
                this.transform.position = Vector3.Lerp (this.transform.position, EnemyPos, Time.deltaTime);
            });
    }
    protected override void Walk () {
        int direction = 1;
        float PlayerDir = Mathf.Abs (this.transform.localScale.x);
        //if(戦ってない時)
        direction = Dir ();
        this.transform.localScale = new Vector2 (newDir () * PlayerDir, this.transform.localScale.y);
        this.transform.Translate (direction * speed * Time.deltaTime, 0, 0);
    }
    int newDir () {
        int direction;
        var Player = GameObject.FindGameObjectWithTag ("Player").transform;
        //左
        if (this.transform.localScale.x < 0) {
            direction = -1;
            offset = _offset;
            if (this.transform.position.x < Player.position.x) {
                direction = 1;
                offset = -_offset;
                return direction;
            }
        }
        //右
        else {
            direction = 1;
            offset = -_offset;
            if (Player.position.x < this.transform.position.x) {
                direction = -1;
                offset = _offset;
                return direction;
            }
        }

        return direction;
    }
}