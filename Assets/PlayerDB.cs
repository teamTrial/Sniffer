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
            return GameObject.Find ("UI/MP").GetComponent<Image> ();
        }
    }
    /// <summary>
    /// AttentionPoint=AP 注目度に関するポイント
    /// </summary>
    [HeaderAttribute ("注目度に関するポイント")]
    public float AP = 0;
    private float OldAP = 0;
    GameObject cameraShake;
    // Status Player = new Status ("Player",15);
    public float HP = 15;
    private float OldHP;
    float Exp = 0;
    public int Lv = 1;
    private PlayerController playercontroller;
    private Animator BattleAnimation;
    private GameObject policeInfo;
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
            policeInfo = Police;
        }
        SyncHP (false, random);
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
        OldAP = (Mathf.Floor (OldAP));
        print (OldAP);
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
        policeInfo.GetComponent<human> ().speed = 1f;
        var PoliceDir = policeInfo.transform.localScale;
        PoliceDir.x = -PoliceDir.x;
        policeInfo.transform.localScale = PoliceDir;
        this.UpdateAsObservable ()
            .Subscribe (_ => {
                var EnemyPos = policeInfo.transform.position;
                EnemyPos.x = policeInfo.transform.position.x + offset;
                Player.transform.position = Vector3.Lerp (Player.transform.position, policeInfo.transform.position, Time.deltaTime);
            }).AddTo (this.gameObject);
        PlayerDir.x = oppositePlayerDir () * PlayerDir.x;
        Player.transform.localScale = PlayerDir;
        Observable.Timer (TimeSpan.FromSeconds (2)).Subscribe (_ => {
            FadeManager.Instance.LoadLevel ("localSelect", 0.5f);
            Destroy (GameObject.Find ("Manager"));
        });
    }

    int oppositePlayerDir () {
        int direction;
        var Player = GameObject.FindGameObjectWithTag ("Player").transform;
        //左
        if (Player.localScale.x < 0) {
            direction = -1;
            if (Player.localScale.x < policeInfo.transform.position.x) {
                direction = 1;
                return direction;
            }
        }
        //右
        else {
            direction = 1;
            if (policeInfo.transform.position.x < Player.localScale.x) {
                direction = -1;
                return direction;
            }
        }

        return direction;
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