using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
public class Actoin : MonoBehaviour
{
    public float Longtap = 1f;
    float time = 0;
    bool flag = false;
    Vector3 old;
    float counter;
    Animator anim;
    public Button center;
    void Start()
    {
        anim=GetComponent<Animator>();
        Attack();
    }
    void Attack()
    {
        var singleclick=center.onClick.AsObservable();
        singleclick
        .Buffer(singleclick.Throttle(TimeSpan.FromMilliseconds(200)))
        .Where(tap=> 1 >= tap.Count)
        .Subscribe(_=>setAnimation(2));

        var doubleclick =center.onClick.AsObservable();
        doubleclick
        // 0.2秒以内のメッセージをまとめる
        .Buffer(doubleclick.Throttle(TimeSpan.FromMilliseconds(200)))
        // タップ回数が2回以上だったら処理する
        .Where(tap => tap.Count >= 2)
        .Subscribe(tap => {
                print("たぶ");
                setAnimation(3);
            });
        var longtap=this.UpdateAsObservable();
        longtap.TakeUntil(doubleclick).RepeatSafe()
        .Subscribe(_=>{
                CheckLongtap();
                
            });
    }
    void LateUpdate()
    {
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
                setAnimation(1);
            }
            //タッチしている＆指が動いている
            else if (touch.phase == TouchPhase.Moved)
            {
                setAnimation(1);
            }
            //タッチしている指が離れた
            else if (touch.phase == TouchPhase.Ended)
            {
                if(anim.GetInteger("Anim")<2){
                    setAnimation(0);
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
