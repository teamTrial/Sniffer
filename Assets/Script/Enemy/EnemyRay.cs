using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRay : MonoBehaviour {
    
    void Start () {
        
    }
    
    void Update () {
        //レイヤーマスク作成
        
        //Rayの長さ
        float maxDistance = 3;
        Vector2 dir=Direction();
        Vector2 pos=new Vector2(transform.position.x+(dir.x),transform.position.y);
        RaycastHit2D hit = Physics2D.Raycast(pos, dir,maxDistance);
#if UNITY_EDITOR
        Debug.DrawRay(pos,dir*maxDistance,Color.green);
#endif
        //なにかと衝突した時だけそのオブジェクトの名前をログに出す
        if(hit.collider){
            Debug.Log(hit.collider.gameObject.name);
        }
        
    }
    Vector2 Direction(){
        var dir=this.transform.localScale.x;
        if(dir<0){
            return Vector2.left;
        }
        return Vector2.right;
    }
}
