using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class poeple : MonoBehaviour {
    float counter;
    int limit=10;
    bool countdowsflag;
    public float speed=1f;
    private InstanceEnemy Right,Left;
    void Start () {
        speed=speed*Random.Range(0.3f,1.3f);
        counter=0;
        Right=GameObject.Find("CreatePeople_Right").GetComponent<InstanceEnemy>();
        Left=GameObject.Find("CreatePeople_Left").GetComponent<InstanceEnemy>();
    }
    
    void Update () {
        walk();
        if(countdowsflag){
            counter+=Time.deltaTime;
        }
        if(limit<counter){
            Destroy(this.gameObject);
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag=="MainCamera"){
            counter=0;
            countdowsflag=false;
        }
        
    }
    //見えなくなったら
    void OnTriggerExit2D(Collider2D other){
        if(other.tag=="MainCamera"){
            countdowsflag=true;
        }
    }
    void walk(){
        int direction=1;
        //左
        if(this.transform.localScale.x<0){
            direction=-1;
        }
        //右
        else{
            direction=1;
        }
        this.transform.Translate(direction*speed*Time.deltaTime,0,0);
    }
    void OnDestroy()
    {
       Right.enemyCounter--;
       Left.enemyCounter--; 
    }
}
