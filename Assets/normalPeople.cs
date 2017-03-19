using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class normalPeople : people {

    new void Start () {
        base.Start();
        Enemytag="Normal";
        HP = EnemyDB.normalPeople;
        HP = NPCHP ();
        EnemyDB.EntryEnemy (this.gameObject.name, HP);
    }
}