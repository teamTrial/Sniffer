using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRay : MonoBehaviour {
    
    [HeaderAttribute("見える範囲")]
    public float maxDistance = 3;
    [HeaderAttribute("目線")]
    public float height=0.5f;
    void Start () {
    }
    void OnTriggerStay2D(Collider2D other)
    {
        if(other.tag=="MainCamera"){
            //レイヤーマスク作成
            
            //Rayの長さ
            Vector2 dir=Direction();
            Vector2 pos=new Vector2(transform.position.x+(dir.x*0.5f),transform.position.y+height);
            RaycastHit2D hit = Physics2D.Raycast(pos, dir,maxDistance);
#if UNITY_EDITOR
            Debug.DrawRay(pos,dir*maxDistance,Color.green);
#endif
            //なにかと衝突した時だけそのオブジェクトの名前をログに出す
            if(hit.collider){
                if(hit.collider.tag=="hand"){
                    this.transform.localScale=new Vector2(-this.transform.localScale.x,this.transform.localScale.y);
                    this.GetComponent<poeple>().speed=2;
                }
            }
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
