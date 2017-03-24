using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// プレイヤーを見つけたときのアクションを実装する
/// </summary>
interface IAction {
    void Action ();
}
/// <summary>
/// /// 歩くや視線などの移動処理、トリガー系を実装
/// </summary>
public class human : MonoBehaviour, IAction {
    public class Count {
        public int limit;
        public float counter;
        public bool countdowsflag;
        public Count (int _limit = 10) {
            countdowsflag = false;
            limit = _limit;
            counter = 0;
        }
    }

    public string NPCColor = "NULL";
    [HideInInspector]
    public int HP;
    protected Count count;
    public string Enemytag;
    private InstanceEnemy Right, Left;
    private StageManager StageManager;
    private Battle battle;
    private PlayerController endbattle;
    protected SniffeUI sniffer;
    protected EnemyStatusDB　 EnemyDB;

    [HeaderAttribute ("見える範囲")]
    public float maxDistance = 3;
    [HeaderAttribute ("目線")]
    public float height = 0.5f;
    public LayerMask PlayerMask;
    /// <summary>
    /// カメラ内か否かのフラグ　true =カメラ内　false=カメラ外
    /// </summary>
    bool inCamera;
    public float speed = 1f;
    protected void Start () {
        EnemyDB = EnemyStatusDB.Instance;
        if (this.tag == "Player") {
            this.GetComponent<people> ().enabled = false;
            maxDistance = 0;
        }
        count = new Count ();
        sniffer = transform.FindChild ("HP").GetComponent<SniffeUI> ();
        StageManager = GameObject.Find ("Manager").GetComponent<StageManager> ();
        battle = GameObject.Find ("Manager").GetComponent<Battle> ();
        endbattle = GameObject.Find ("UI/Controller").GetComponent<PlayerController> ();
        speed = speed * Random.Range (0.3f, 1.0f);
    }
    protected void Update () {
        if (PlayerController.BattleFlag && !inCamera) {

        } else {
            Walk ();
            if (count.countdowsflag) {
                count.counter += Time.deltaTime;
            }
            if (count.limit < count.counter) {
                EnemyDB.DeleteEnemy (this.name);
                Destroy (this.gameObject);
            }
        }
    }
    protected void OnTriggerEnter2D (Collider2D other) {
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
            BattleDir (GameObject.FindGameObjectWithTag ("Player").transform.localScale.x);
            battle.StartBattle (this.gameObject, HP, Enemytag);
        }
        if (other.tag == "stand") {
            print ("構えるモーション接触");
        }
    }
    protected void OnTriggerStay2D (Collider2D other) {
        if (other.tag == "MainCamera") {
            EyeLine (other);
        }
    }
    //見えなくなったら
    protected void OnTriggerExit2D (Collider2D other) {
        if (other.tag == "MainCamera") {
            inCamera = false;
            count.countdowsflag = true;
        }
    }
    /// <summary>
    /// バトル開始時背後を取られていたら振り向くように
    /// </summary>
    void BattleDir (float playerDir) {
        var dir = 1;
        if (playerDir < 0) {
            dir = 1;
        } else {
            dir = -1;
        }
        this.transform.localScale = new Vector2 (dir * Mathf.Abs (this.transform.localScale.x), this.transform.localScale.y);
    }

    // /// <summary>
    // /// NPCの視線
    // /// </summary>
    protected virtual void EyeLine (Collider2D other) {
        //レイヤーマスク作成
        RaycastHit2D hit = HitObj ();
        if (hit.collider) {
            if (hit.collider.tag == "hand") {
                Action ();
            } else if (hit.collider.tag == "Player") {
                if (PlayerController.BattleFlag) {
                    //speed=0　誰かが戦っているとき
                    if (speed != 0) {
                        Action ();
                    }
                }
            }
        }
    }
    //視線にプレイヤーをとらえたときの動作
    public virtual void Action(){}
    protected RaycastHit2D HitObj () {
        //Rayの長さ
        Vector2 dir = RayDirection ();
        Vector2 pos = new Vector2 (transform.position.x + (dir.x * 0.5f), transform.position.y + height);
        RaycastHit2D hit = Physics2D.Raycast (pos, dir, maxDistance, PlayerMask);
#if UNITY_EDITOR
        Debug.DrawRay (pos, dir * maxDistance, Color.green);
#endif
        return hit;
    }
    Vector2 RayDirection () {
        var dir = this.transform.localScale.x;
        if (dir < 0) {
            return Vector2.left;
        }
        return Vector2.right;
    }
    protected virtual void Walk () {
        int direction = 1;
        direction = Dir ();
        this.transform.Translate (direction * speed * Time.deltaTime, 0, 0);
    }
    //進む方向に関する処理
    protected virtual int Dir () {
        int direction;
        //左
        if (this.transform.localScale.x < 0) {
            direction = -1;
        }
        //右
        else {
            direction = 1;
        }

        return direction;
    }
}