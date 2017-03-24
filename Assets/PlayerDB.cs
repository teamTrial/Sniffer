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
    public float AP = 0;
    private float OldAP = 0;
    GameObject cameraShake;
    // Status Player = new Status ("Player",15);
    public float HP = 15;
    // public float MP=15;
    private float OldHP;
    float Exp = 0;
    public int Lv = 1;

    void Start () {
        OldHP = HP;
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

    public void Damage (String EnemyName, int EnemyHP) {
        iTween.ShakePosition (cameraShake, new Vector2 (0.1f, 0.1f), 0.3f);
        var random = UnityEngine.Random.Range (0.005f, 0.02f);
        EnemyStatusDB.Instance.Enemy[EnemyName] = EnemyHP;
        SyncHP (false, random);
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
                Death ();
            }
        }
        print ("OldAP:"+OldAP);
        AP = AP_ui.fillAmount * 5;
        HP = HP_ui.fillAmount * OldHP;
    }
    /// <summary>
    /// OldAPが1になった場合最低値が1になるように
    /// OldAPが2になった場合最低値が2になるように
    /// OldAPが3になった場合最低値が3になるように
    /// </summary>
    void AttentionPointSystem (float heelPoint) {
        if ((int) OldAP < AP) {
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