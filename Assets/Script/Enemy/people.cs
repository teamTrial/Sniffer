using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class people : MonoBehaviour {
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
    public int HP;
    protected Count count;
    public float speed = 1f;
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
    protected int NPCHP () {
        return Random.Range (HP - 5, HP + 3);
    }

    protected void Start () {
        EnemyDB = EnemyStatusDB.Instance;
        if (this.tag == "Player") {
            this.GetComponent<people> ().enabled = false;
        }
        count = new Count ();
        sniffer = transform.FindChild ("HP").GetComponent<SniffeUI> ();
        StageManager = GameObject.Find ("Manager").GetComponent<StageManager> ();
        battle = GameObject.Find ("Manager").GetComponent<Battle> ();
        endbattle = GameObject.Find ("UI/Controller").GetComponent<PlayerController> ();
        speed = speed * Random.Range (0.3f, 1.0f);
    }
    protected void Update () {
        if (!PlayerController.BattleFlag) {
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
            EyeLine (other);
            count.counter = 0;
            count.countdowsflag = false;
        }
        //snifferアクション時魂を表示する処理を記入
        if (other.tag == "controller") return;

        //至近距離でプレイヤーに接触した場合
        if (other.tag == "hand") {
            // Destroy(this.gameObject);
            // StageManager.EnemyNum=StageManager.EnemyNum+1;
            // StageManager.UpdateNum();
            print ("当たった");
            // GetComponent<Battle>().enabled=true;
            battle.StartBattle (this.gameObject, HP);
        }
        if (other.tag == "stand") {
            print ("構えるモーション接触");
        }
    }
    //見えなくなったら
    protected void OnTriggerExit2D (Collider2D other) {
        if (other.tag == "MainCamera") {
            count.countdowsflag = true;
        }
    }

    protected void Walk () {
        int direction = 1;
        direction = Dir ();
        this.transform.Translate (direction * speed * Time.deltaTime, 0, 0);
    }

    protected int Dir () {
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
    /// <summary>
    /// NPCの視線
    /// </summary>
    public void EyeLine (Collider2D other) {
        //レイヤーマスク作成

        RaycastHit2D hit = HitObj ();
        //なにかと衝突した時だけそのオブジェクトの名前をログに出す
        if (hit.collider) {
            if (hit.collider.tag == "hand") {
                Escape ();
            }
        }
    }
    protected RaycastHit2D HitObj () {
        //Rayの長さ
        Vector2 dir = RayDirection ();
        Vector2 pos = new Vector2 (transform.position.x + (dir.x * 0.5f), transform.position.y + height);
        RaycastHit2D hit = Physics2D.Raycast (pos, dir, maxDistance);
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
    public void Escape (float EscapeSpeed = 2) {
        this.transform.localScale = new Vector2 (-this.transform.localScale.x, this.transform.localScale.y);
        this.GetComponent<people> ().speed = EscapeSpeed;
    }
}