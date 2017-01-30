using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hand : MonoBehaviour {
    private Animator anim;
    private float x;
    void Start () {
        anim=GameObject.Find("Character").GetComponent<Animator>();
    }
    
    void Update () {
        checkAnim();
    }
    void checkAnim(){
        AnimatorStateInfo animInfo = anim.GetCurrentAnimatorStateInfo(0);
        if(animInfo.normalizedTime<0.5f )
        {
            if(anim.GetInteger("Anim")>2){
                print("幅絵");
                x+=0.1f;
                this.transform.localScale=new Vector2(x,this.transform.localScale.y);
            }
        }else if(0.5f<animInfo.normalizedTime){
            if(anim.GetInteger("Anim")==3){
                x-=0.1f;
                this.transform.localScale=new Vector2(x,this.transform.localScale.y);
            }
        }
    }
    
}
