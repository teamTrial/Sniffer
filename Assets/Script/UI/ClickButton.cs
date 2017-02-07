using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

public class ClickButton : MonoBehaviour {
    public string sence="globalSelect";
    void Start () {
        Button	Button=this.GetComponent<Button>();
        var startclick=Button.onClick.AsObservable();
        startclick
        .Take(1)
        .Subscribe(tap => {
            FadeManager.Instance.LoadLevel(sence,2f);
        });
    }
}
