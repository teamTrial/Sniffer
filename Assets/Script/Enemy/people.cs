using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// NPC(襲われる系)の機能を実装
/// </summary>
public class people : human,IAction {
    protected int NPCHP () {
        return Random.Range (HP - 5, HP + 3);
    }
    public override void Action () {
        this.transform.localScale = new Vector2 (-this.transform.localScale.x, this.transform.localScale.y);
        this.GetComponent<people> ().speed = 2;
    }
}