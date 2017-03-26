using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// NPC(襲われる系)の機能を実装
/// </summary>
public class people : human, IAction {
    protected int NPCHP () {
        return Random.Range (HP - 5, HP + 3);
    }
    public override void Action () {
        Escape ();
    }
    void Escape () {
        this.transform.localScale = new Vector2 (EscapeDir()*this.transform.localScale.x, this.transform.localScale.y);
        this.GetComponent<people> ().speed = 2;
    }
    int EscapeDir(){
           int direction;
        var Player = GameObject.FindGameObjectWithTag ("Player").transform;
        //左
        if (this.transform.localScale.x < 0) {
            direction = -1;
            if (this.transform.position.x < Player.position.x) {
                direction = -1;
                return direction;
            }
        }
        //右
        else {
            direction = 1;
            if (Player.position.x < this.transform.position.x) {
                direction = 1;
                return direction;
            }
        }

        return direction;
    }
}