using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

/// <summary>
/// デモ用の暫定処置
/// </summary>

public class checkGoal : MonoBehaviour {
    public string SceneName;
    public float loadtime = 2f;
    public string checkColor;

    public enum Dir {
        left,
        right
    }

    public Dir _dir;

    void OnCollisionEnter2D (Collision2D Player) {
        if (Player.gameObject.tag == "Player") {
            if (Player.gameObject.GetComponent<human> ().NPCColor == checkColor) {
                this.GetComponent<BoxCollider2D> ().isTrigger = true;
            }
        }
    }

    void OnTriggerExit2D (Collider2D Player) {
        if (Player.gameObject.tag == "Player") {
            if (Player.gameObject.GetComponent<people> ().NPCColor == checkColor) {
                this.GetComponent<BoxCollider2D> ().isTrigger = false;
                LeftorRight ();
            }
        }
    }
    void LeftorRight () {
        var Player = GameObject.FindWithTag ("Player");
        if (_dir == Dir.right) {
            Comparison (this.transform, Player.transform);
        } else if (_dir == Dir.left) {
            Comparison (Player.transform, this.transform);
        }
    }
    void Comparison (Transform A, Transform B) {
        if (A.position.x < B.position.x) {
            StageClear.ClearFlag = true;
            FadeManager.Instance.LoadLevel (SceneName, loadtime);
            Destroy (GameObject.Find ("Manager"));
        }
    }
}