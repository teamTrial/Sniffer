using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
public class Actoin : MonoBehaviour {
    Animator anim;
    public Button center;
    //攻撃中は移動できないように
    public static bool attackFlag {
        get;
        private set;
    }
    float LongTap = 0;
    void Start () {
        anim = GetComponent<Animator> ();
        Attack ();
    }
    void Attack () {
        // var singleclick=center.onClick.AsObservable();
        // singleclick
        // .Buffer(singleclick.Throttle(TimeSpan.FromMilliseconds(200)))
        // .Where(tap=> 1 >= tap.Count)
        // .Subscribe(tap => {
        //     attackFlag=true;
        //     setAnimation(2);
        // });

        var doubleclick = center.onClick.AsObservable ();
        doubleclick
            .Where (attack => !PlayerController.BattleFlag)
            // 0.2秒以内のメッセージをまとめる
            .Buffer (doubleclick.Throttle (TimeSpan.FromMilliseconds (300)))
            // タップ回数が2回以上だったら処理する
            .Where (tap => tap.Count >= 2)
            .Subscribe (tap => {
                attackFlag = true;
                setAnimation (3);
            });
        var longtap = this.UpdateAsObservable ();
        longtap.TakeUntil (doubleclick).RepeatSafe ()
            .Subscribe (_ => {
                CheckLongtap ();
            });
    }
    void LateUpdate () {
        checkAnim ();
    }

    void CheckLongtap () {

        if (Input.touchCount > 0) {
            Touch touch = Input.GetTouch (0);
            if (!attackFlag) {
                //タッチしている＆指が動いてない
                if (touch.phase == TouchPhase.Stationary) {
                    if (PlayerController.CenterFlag) {
                        LongTap += Time.deltaTime;
                        // setAnimation(1);
                    }
                }
                //タッチしている＆指が動いている
                else if (touch.phase == TouchPhase.Moved) {
                    if (PlayerController.CenterFlag) {
                        setAnimation (1);
                        LongTap += Time.deltaTime;
                    }
                }
            }
            //タッチしている指が離れた
            if (touch.phase == TouchPhase.Ended) {
                if (PlayerController.CenterFlag) {
                    if (LongTap > 1.0f) {
                        print ("襲うアクション");
                        setAnimation (3);
                    }
                }
                LongTap = 0;
                if (anim.GetInteger ("Anim") < 2) {
                    setAnimation (0);
                }
            }
        }
    }
    /// <summary>
    ///アニメーションを再生させる
    ///0=止まる
    ///1=歩く
    ///2=構える
    ///3=攻撃
    /// </summary>
    void setAnimation (int animnum = 0) {
        anim.SetInteger ("Anim", animnum);
    }
    void checkAnim () {
        AnimatorStateInfo animInfo = anim.GetCurrentAnimatorStateInfo (0);
        if (1.0f < animInfo.normalizedTime) {
            //アニメーションが止まると移動以外だった場合
            if (1 < anim.GetInteger ("Anim")) {
                setAnimation (0);
                attackFlag = false;
            }
        }
    }
}