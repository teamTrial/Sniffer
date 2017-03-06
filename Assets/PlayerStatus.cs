using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
public class PlayerStatus : MonoBehaviour {
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
    Status Player = new Status ("Player",15);

    void Start () {
        cameraShake=Camera.main.gameObject;
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
                HP_ui.fillAmount += 0.01f * Time.deltaTime;
                MP_ui.fillAmount -= 0.005f * Time.deltaTime;
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
    public void Damage () {
        iTween.ShakePosition(cameraShake,new Vector2(0.1f,0.1f),0.3f);
        var random = UnityEngine.Random.Range (0.005f, 0.02f);
        HP_ui.fillAmount -= random;
        Player.HP -= random * Player.OldHP;
        MP_ui.fillAmount += random;
    }
}