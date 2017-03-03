using System;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Status {
    public float HP ;
    public float OldHP{
        get{
            return HP;
        }
        private set{}
    }
    public String Name;
    public float MP { get; set; }
    public Status(String name,float _HP){
        HP=_HP;
        Name=name;
    }
}