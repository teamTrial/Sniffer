using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class Actoin : MonoBehaviour
{
    public float Longtap = 1f;
    void Start()
    {
        hoge();
        // hoge2( );
    }
    void hoge()
    {
        var doubleclick = this.UpdateAsObservable()
        .Where(_ => Input.GetMouseButtonDown(0));
        doubleclick
        // 0.2秒以内のメッセージをまとめる
        .Buffer(doubleclick.Throttle(TimeSpan.FromMilliseconds(200)))
        // タップ回数が2回以上だったら処理する
        .Where(tap => tap.Count >= 2)
        .Subscribe(tap => Debug.Log("Do Something."));
    }
    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        CheckLongtap();
    }
    float time = 0;
    bool flag = false;
    Vector3 old;
    float counter;
    void CheckLongtap()
    {

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Stationary)
            {
                counter += 0.5f;
                if (counter > 5f)
                {
                    time += 0.1f;
                    if (5f < time)
                    {
                        print("ロングタップ＋固定");
                        flag = true;
                        counter = 0;
                    }
                }
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                counter += 0.5f;
                if (counter > 5f)
                {
                    print("移動したのでキャンセル");
                    time = 0;
                    flag = false;
                    counter = 0;
                }
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                if (flag)
                {
                    print("構えるモーション");
                }
                flag = false;
                counter = 0;
            }
        }
    }
}
