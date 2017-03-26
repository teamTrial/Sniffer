using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class police : human, IAction {
    //プレイヤーとの距離
    float offset;
    float _offset = 0.7f;
    public float CameraAnimationTime;
    public static bool BattleFlag = false;
    new void Start () {
        base.Start ();
        BattleFlag = false;
        offset = _offset;
        HP = EnemyDB.normalPeople;

        //テスト
        Action ();
    }
    protected override void OnTriggerEnter2D (Collider2D other) {
        if (other.tag == "MainCamera") {
            inCamera = true;
            count.counter = 0;
            count.countdowsflag = false;
        }
        //snifferアクション時魂を表示する処理を記入
        if (other.tag == "controller") return;

        //至近距離でプレイヤーに接触した場合
        if (other.tag == "hand") {
            print ("当たった");
            speed = 0;
        }
        if (other.tag == "stand") {
            print ("構えるモーション接触");
        }
    }
    void StartBattle (GameObject NPC, int HP = 10) {
        print("バトル");
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
            }).AddTo (this.gameObject);
        //ズームの時間を取得
        AnimatorStateInfo cameraAnim = Camera.main.GetComponent<Animator> ().GetCurrentAnimatorStateInfo (0);
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
                LevelFive ();
            } else if (hit.collider.tag == "Player") {
                if (PlayerController.BattleFlag) {
                    //speed=0　誰かが戦っているとき
                    if (speed != 0) {
                        LevelFive ();
                        return;
                    }
                    Action ();
                }

            }
        }
    }
    /// <summary>
    /// 認知度が高かった時にプレイヤーを見つけたら
    /// </summary>
    public override void Action () {
        LevelFive ();
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
            .Where (_ => !BattleFlag)
            .Subscribe (_ => {
                var EnemyPos = Player.transform.position;
                EnemyPos.x = Player.transform.position.x + offset;
                this.transform.position = Vector3.Lerp (this.transform.position, EnemyPos, Time.deltaTime);
                var Distance = Mathf.Abs (EnemyPos.x - this.transform.position.x);
                if (Distance < 0.05f) {
                    StartBattle (this.gameObject, HP);
                    BattleFlag = true;
                }
            });
    }
    protected override void Walk () {
        if(BattleFlag){
            return;
        }
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