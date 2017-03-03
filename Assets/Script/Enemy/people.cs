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
    [SerializeField]
    public int HP=10;

    Count count;
    public float speed = 1f;
    private InstanceEnemy Right, Left;
    private StageManager StageManager;
    private Battle battle;
    private PlayerController endbattle;
    private SniffeUI sniffer;
    EnemyStatusDB　EnemyDB;
    int NPCHP(){
        return Random.Range(HP-5,HP+3);
    }
    void Start () {
        HP=NPCHP();        
       EnemyDB= GameObject.Find ("Manager").GetComponent<EnemyStatusDB> ();
       EnemyDB.EntryEnemy(this.gameObject.name,HP);
        count = new Count ();
        sniffer = transform.FindChild ("HP").GetComponent<SniffeUI> ();
        StageManager = GameObject.Find ("Manager").GetComponent<StageManager> ();
        battle = GameObject.Find ("Manager").GetComponent<Battle> ();
        endbattle = GameObject.Find ("UI/Controller").GetComponent<PlayerController> ();
        speed = speed * Random.Range (0.3f, 1.0f);
        // Right = GameObject.Find ("CreatePeople_Right").GetComponent<InstanceEnemy> ();
        // Left = GameObject.Find ("CreatePeople_Left").GetComponent<InstanceEnemy> ();

    }

    void Update () {
        if (!PlayerController.BattleFlag) {
            walk ();
            if (count.countdowsflag) {
                count.counter += Time.deltaTime;
            }
            if (count.limit < count.counter) {
                Destroy (this.gameObject);
            }
        }
    }
    void OnTriggerEnter2D (Collider2D other) {
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
            battle.StartBattle (this.gameObject);
        }
        if (other.tag == "stand") {
            print ("構えるモーション接触");
        }
    }
    //見えなくなったら
    void OnTriggerExit2D (Collider2D other) {
        if (other.tag == "MainCamera") {
            count.countdowsflag = true;
        }
    }

    void walk ()
    {
        int direction = 1;
        direction = NewMethod();
        this.transform.Translate(direction * speed * Time.deltaTime, 0, 0);
    }

    private int NewMethod()
    {
        int direction;
        //左
        if (this.transform.localScale.x < 0)
        {
            direction = -1;
        }
        //右
        else
        {
            direction = 1;
        }

        return direction;
    }

    void OnDestroy () {
        EnemyDB.DeleteEnemy();
        // Right.enemyCounter--;
        // Left.enemyCounter--;
    }
}