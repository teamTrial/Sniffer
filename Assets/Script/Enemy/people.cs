using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (EnemyRay))]
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
    Count count;
    public float speed = 1f;
    private InstanceEnemy Right, Left;
    private StageManager StageManager;
    private Battle battle;
    private PlayerController endbattle;
    protected SniffeUI sniffer;
    protected EnemyStatusDB　 EnemyDB;
    public enum _EnemyType {
        people,
        JK,
        police
    }
    protected int NPCHP () {
        return Random.Range (HP - 5, HP + 3);
    }

    protected void Start () {
        EnemyDB = EnemyStatusDB.Instance;
        if(this.tag=="Player"){
            this.GetComponent<people>().enabled=false;
        }
        count = new Count ();
        sniffer = transform.FindChild ("HP").GetComponent<SniffeUI> ();
        StageManager = GameObject.Find ("Manager").GetComponent<StageManager> ();
        battle = GameObject.Find ("Manager").GetComponent<Battle> ();
        endbattle = GameObject.Find ("UI/Controller").GetComponent<PlayerController> ();
        speed = speed * Random.Range (0.3f, 1.0f);
    }
    // protected void SetStatus (_EnemyType EnemyType) {
    //     EnemyDB = EnemyStatusDB.Instance;

    //     if (EnemyType == _EnemyType.people) {
    //         HP = EnemyDB.normalPeople;
    //     } else if (EnemyType == _EnemyType.JK) {

    //     } else if (EnemyType == _EnemyType.police) {

    //     }
    //     HP = NPCHP ();
    //     EnemyDB.EntryEnemy (this.gameObject.name, HP);
    // }
    protected void Update () {
        if (!PlayerController.BattleFlag) {
            walk ();
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

    void walk () {
        int direction = 1;
        direction = Dir ();
        this.transform.Translate (direction * speed * Time.deltaTime, 0, 0);
    }

    private int Dir () {
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