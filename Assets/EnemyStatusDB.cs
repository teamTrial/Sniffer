using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
public class EnemyStatusDB : MonoBehaviour {
    public List<Status> Enemy = new List<Status> ();
    public GameObject enemy;
    public int Limit = 10;
    public int Counter;
    Transform pos;
    Transform rightpos, leftpos;
    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start () {
		rightpos=GameObject.Find("Right").transform;
		leftpos=GameObject.Find("Left").transform;
        Counter = 0;
        Observable.Timer (TimeSpan.FromSeconds (5), TimeSpan.FromSeconds (2))
            .Where (_ => 0 <= Counter && Counter < Limit)
            .Subscribe (_ => {
                Instance (UnityEngine.Random.Range (-5.5f, 0.5f));
            }).AddTo (this.gameObject);
    }
    public void EntryEnemy (String EnemyName, float HP) {
        Enemy.Add (new Status (EnemyName, HP));
    }
	public void DeleteEnemy(){
		Counter--;
	}
    void Instance (float randompos) {
        if ((Counter % 2) == 0) {
            pos = rightpos;
        } else {
            pos = leftpos;
        }
        var ins = Instantiate (enemy, pos).gameObject;
        if (GameObject.Find ("Main Camera").transform.position.x < pos.position.x) {
            //左向き左移動
            ins.transform.position = new Vector3 (pos.transform.position.x + (-randompos), pos.transform.position.y, pos.transform.position.z);
            ins.transform.localScale = new Vector2 (-ins.transform.localScale.x, ins.transform.localScale.y);
        } else {
            ins.transform.position = new Vector3 (pos.transform.position.x + randompos, pos.transform.position.y, pos.transform.position.z);
            //右向き右移動
        }
        Counter++;
    }
}