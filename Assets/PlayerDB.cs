using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
public class PlayerDB : SingletonMonoBehaviour<PlayerDB> {

    public Image HP_ui {
        get {
            return GameObject.Find ("UI/HP").GetComponent<Image> ();
        }
    }

    public Image AP_ui {
        get {
            return GameObject.Find ("UI/AP").GetComponent<Image> ();
        }
    }
    /// <summary>
    /// AttentionPoint=AP 注目度に関するポイント
    /// </summary>
    [HeaderAttribute ("注目度に関するポイント")]
    public float AP = 0;
    private float OldAP = 0;
    public float HP = 15;
    private float OldHP;
    /// <summary>
    /// 経験値(LVに影響)
    /// </summary>
    float Exp = 0;
    /// <summary>
    /// プレイヤーのレベル(NPCのウェーブに影響)
    /// </summary>
    public int Lv = 1;
    /// <summary>
    /// バトル時のカメラが揺れる奴
    /// </summary>
    GameObject cameraShake;
    private PlayerController playercontroller;
    private Animator BattleAnimation;
    private GameObject PoliceInfo;
    private float offset = 0.2f;
    void Start () {
        OldHP = HP;
        playercontroller = GameObject.Find ("UI/Controller").GetComponent<PlayerController> ();
        BattleAnimation = Camera.main.GetComponent<Animator> ();
#if UNITY_EDITOR
        // Heel (0);
#endif
        cameraShake = Camera.main.gameObject;
    }
    /// <summary>
    /// HPを回復、APを減らす
    /// </summary>
    /// <param name="interval">
    /// 開始するまでの時間
    /// </param>
    public void Heel (int interval = 20) {
        this.UpdateAsObservable ()
            .TakeWhile (NotCentor => !PlayerController.BattleFlag)
            .Delay (TimeSpan.FromSeconds (interval))
            .Subscribe (_ => {
                SyncHP (true);
            }).AddTo (this.gameObject);
    }

    public void Damage (String EnemyName, int EnemyHP, float min = 0.005f, float max = 0.02f, GameObject Police = null) {
        var random = UnityEngine.Random.Range (min, max);
        EnemyStatusDB.Instance.Enemy[EnemyName] = EnemyHP;
        if (Police != null) {
            PoliceInfo = Police;
        }
        SyncHP (false, random);
        //カメラをがくがくさせる
        iTween.ShakePosition (cameraShake, new Vector2 (0.1f, 0.1f), 0.3f);
    }
    /// <summary>
    /// UIのHPとPlayerのHPを同期させる
    /// </summary> 
    /// <param name="Mode">true=回復,false=ダメージ</param>
    void SyncHP (bool Mode, float damage = 0) {
        if (Mode) {
            var heelPoint = 0.01f * Time.deltaTime;
            HP_ui.fillAmount += heelPoint;
            AttentionPointSystem (heelPoint);
        } else {
            HP_ui.fillAmount -= damage;
            AP_ui.fillAmount += damage;
            OldAP = AP;
            if (HP <= 0) {
                if (police.BattleFlag || PlayerController.BattleFlag) {
                    Death ();
                }
            }
        }
        AP = AP_ui.fillAmount * 5;
        HP = HP_ui.fillAmount * OldHP;
    }
    /// <summary>
    /// OldAPが1になった場合最低値が1になるように
    /// OldAPが2になった場合最低値が2になるように
    /// OldAPが3になった場合最低値が3になるように
    /// </summary>
    void AttentionPointSystem (float heelPoint) {
        //認知度MAXの場合は処理を中断
        if (5 <= AP) {
            return;
        }
        //小数点を切り捨てる
        OldAP = (Mathf.Floor (OldAP));
        if (OldAP < AP) {
            AP_ui.fillAmount -= heelPoint;
        }
    }
    public void getExp (float EnemyExp) {
        Exp += EnemyExp;
        if (Lv <= Exp) {
            LevelUp ();
        }
    }
    void Death () {
        print ("プレイヤー死亡");
        //警察に殺されました
        if (police.BattleFlag) {
            Arrest ();
            police.PoliceWinFlag = true;
        }
        //NPCに殺されました
        if (PlayerController.BattleFlag) {

        }
        police.BattleFlag = false;
        playercontroller.EndBattle ();
    }
    /// <summary>
    /// 逮捕されて連行される
    /// </summary>
    void Arrest () {
        var Player = GameObject.FindGameObjectWithTag ("Player");
        var PlayerDir = Player.transform.localScale;
        PoliceInfo.GetComponent<human> ().speed = 1f;
        var PoliceDir = PoliceInfo.transform.localScale;
        PoliceDir.x = -PoliceDir.x;
        PoliceInfo.transform.localScale = PoliceDir;
        //3秒後ステージセレクトへ
        Observable.Timer (TimeSpan.FromSeconds (3.5f)).Subscribe (_ => {
            FadeManager.Instance.LoadLevel ("localSelect", 0.5f);
            Destroy (GameObject.Find ("Manager"));
        });
        this.UpdateAsObservable ()
            .Subscribe (_ => {
                var EnemyPos = PoliceInfo.transform.position;
                EnemyPos.x = PoliceInfo.transform.position.x + offset;
                Player.transform.position = Vector3.Lerp (Player.transform.position, PoliceInfo.transform.position, 1.2f*Time.deltaTime);
            });
        opposite (PoliceInfo);
    }
    /// <summary>
    /// プレイヤーがPoliceの正面を向くように
    /// </summary>
    /// <param name="Police"></param>
    public void opposite (GameObject Police) {
        int direction;
        var Player = GameObject.FindGameObjectWithTag ("Player").transform;
        var newPlayerDir = Player.transform.localScale;
        //Playerが右を向いているとき
        if (0 < Player.localScale.x) {
            direction = -1;
            //プレイヤーが右を向いている&Policeが右側にいるとき
            if (Player.position.x < Police.transform.position.x) {
                direction = 1;
            }
        }
        //Playerが左を向いているとき
        else {
            direction = -1;
            if (Police.transform.position.x < Player.position.x) {
                direction = 1;
            }
        }
        newPlayerDir.x = direction * newPlayerDir.x;
        Player.transform.localScale = newPlayerDir;
    }
    void LevelUp () {
        Lv++;
        Exp = 0;
        print ("レベルアップ");
        NextWave (Lv);
    }
    void NextWave (int Lv) {
        if (Lv % 2 == 0) {
            EnemyStatusDB.Instance.WaveLv++;
        }
    }
}