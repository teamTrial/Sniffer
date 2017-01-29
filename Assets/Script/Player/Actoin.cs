using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class Actoin : MonoBehaviour
{
    public float Longtap = 1f;
    float time = 0;
    bool flag = false;
    Vector3 old;
    float counter;
    Animator anim;
    void Start()
    {
        anim=GetComponent<Animator>();
        Attack();
    }
    void Attack()
    {
        var doubleclick = this.UpdateAsObservable()
        .Where(_ => Input.GetMouseButtonDown(0));
        doubleclick
        // 0.2秒以内のメッセージをまとめる
        .Buffer(doubleclick.Throttle(TimeSpan.FromMilliseconds(200)))
        // タップ回数が2回以上だったら処理する
        .Where(tap => tap.Count >= 2)
        .Subscribe(tap => {
                print("たぶ");
                setAnimation(3);
            });
    }
    void Update()
    {
        CheckLongtap();
        checkAnim();
        
    }
    void CheckLongtap()
    {
        
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            //タッチしている＆指が動いてない
            if (touch.phase == TouchPhase.Stationary)
            {
                counter += 0.5f;
                if (counter > 5f)
                {
                    time += 0.5f;
                    if (5f < time)
                    {
                        print("チャージ完了");
                        flag = true;
                        counter = 0;
                    }
                }
            }
            //タッチしている＆指が動いている
            else if (touch.phase == TouchPhase.Moved)
            {
                counter += 0.5f;
                if (counter > 5f)
                {
                    // print("移動したのでキャンセル");
                    //初期化----------------
                    time = 0;
                    flag = false;
                    counter = 0;
                    //---------------------
                }
            }
            //タッチしている指が離れた
            else if (touch.phase == TouchPhase.Ended)
            {
                if (flag)
                {
                    print("構えるモーション");
                    setAnimation(2);
                }
                time=0;
                flag = false;
                counter = 0;
                return;
            }
            setAnimation(1);
        }
    }
    /// <summary>
    ///アニメーションを再生させる
    ///0=止まる
    ///1=歩く
    ///2=構える
    ///3=攻撃
    /// </summary>
    void setAnimation(int animnum=0){
        anim.SetInteger("Anim",animnum);
    }
    void checkAnim(){
        AnimatorStateInfo animInfo = anim.GetCurrentAnimatorStateInfo(0);
        if(1.0f<animInfo.normalizedTime )
        {
            if(1<anim.GetInteger("Anim")){
                setAnimation(0);
            }
        }
    }
}
