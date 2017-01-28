using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
public class InstanceEnemy : MonoBehaviour {
    public GameObject enemy;
    public int enemyLimit=0;
    public int enemyCounter;
    Transform pos;
    void Start () {
        enemyCounter=0;
        pos=this.transform;
        Observable.Timer(TimeSpan.FromSeconds(5),TimeSpan.FromSeconds(2))
        .Where(_=>enemyCounter<enemyLimit)
        .Subscribe(_=>{
                Instance(UnityEngine.Random.Range(-5.5f,0.5f));
            }).AddTo(this.gameObject);
    }
    void Instance(float randompos){
        var ins=Instantiate(enemy,pos).gameObject;
        if(GameObject.Find("Main Camera").transform.position.x<this.transform.position.x){
            //左向き左移動
			ins.transform.position=	new Vector3(pos.transform.position.x+(-randompos),pos.transform.position.y,pos.transform.position.z);
            ins.transform.localScale=new Vector2(-ins.transform.localScale.x, ins.transform.localScale.y);
        }else{
            ins.transform.position=	new Vector3(pos.transform.position.x+randompos,pos.transform.position.y,pos.transform.position.z);
            //右向き右移動
        }
        enemyCounter++;
    }
}
