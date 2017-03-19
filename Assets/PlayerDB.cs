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
    public Image MP_ui {
        get {
            return GameObject.Find ("UI/MP").GetComponent<Image> ();
        }
    }
    GameObject cameraShake;
    // Status Player = new Status ("Player",15);
    public float HP = 15;
    // public float MP=15;
    private float OldHP;
    float Exp=0;
    public int Lv = 1;

    void Start () {
        OldHP = HP;
        Heel (0);
        cameraShake = Camera.main.gameObject;
    }
    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update () { }
    public void Heel (int interval = 20) {
        this.UpdateAsObservable ()
            .TakeWhile (NotCentor => !PlayerController.BattleFlag)
            .Delay (TimeSpan.FromSeconds (interval))
            .Subscribe (_ => {
                SyncHP (true);
            }).AddTo (this.gameObject);
    }
    public void Death () {
        this.UpdateAsObservable ()
            .Where (HP => HP_ui.fillAmount == 0)
            .First ()
            .Subscribe (_ => {
                print ("プレイヤー死亡");
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
            HP_ui.fillAmount += 0.01f * Time.deltaTime;
            MP_ui.fillAmount -= 0.005f * Time.deltaTime;
        } else {
            HP_ui.fillAmount -= damage;
            MP_ui.fillAmount += damage;
        }
        HP = HP_ui.fillAmount * OldHP;
    }
    public void getExp (float EnemyExp) {
        Exp += EnemyExp;
        if (Lv <= Exp) {
            LevelUp ();
        }
    }
    void LevelUp () {
        Lv++;
        Exp = 0;
        print("レベルアップ");
        NextWave(Lv);
    }
    void NextWave(int Lv){

        if(Lv%2==0){
            EnemyStatusDB.Instance.WaveLv++;
        }
    }
}