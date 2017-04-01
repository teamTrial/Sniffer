using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniffeUI : MonoBehaviour {
    SpriteRenderer UI;
    Transform parent;
    Vector3 DefSize;
    /// <summary>
    /// NPCの基準のHP
    /// </summary>
    float AverageSize;
    float MaxHP, afterHP;
    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start () {

        UI = this.GetComponent<SpriteRenderer> ();
        parent = transform.parent.transform;
        if (parent.tag == "Player") {
            this.GetComponent<SniffeUI> ().enabled = false;
        }
        try {
            MaxHP = (int) EnemyStatusDB.Instance.Enemy[parent.name];
        } catch (KeyNotFoundException) {
            EnemyStatusDB.Instance.EntryEnemy (parent.name, 10);
        }
    }
    public void checkDefSize(float npcHP ,float average){
        this.transform.localScale=(npcHP/average)*this.transform.localScale;
        DefSize = this.transform.localScale;
    }
    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update () {
        CheckSnifferActionFlag ();
        CheckDir ();
        ChangeSize ();
    }
    public void CheckSnifferActionFlag () {
        this.transform.position = parent.position + (Vector3.up * 0.15f);
        if (PlayerController.SnifferActionFlag) {
            UI.enabled = true;
        } else {
            UI.enabled = false;
        }
    }
    public void UpdateSize () {
        DefSize = this.transform.localScale;
    }
    void CheckDir () {
        var Parent = transform.parent;
        if (Parent.localScale.x < 0) {
            UI.flipX = true;
        } else {
            UI.flipX = false;
        }
    }
    /// <summary>
    /// 親オブジェクトの残りHPに応じてサイズを変更する
    /// </summary>
    void ChangeSize () {
        if(police.PoliceWinFlag||StageClear.ClearFlag){
            return;
        }
        afterHP = EnemyStatusDB.Instance.Enemy[parent.name];
        this.transform.localScale = DefSize * (afterHP / MaxHP);
    }
}