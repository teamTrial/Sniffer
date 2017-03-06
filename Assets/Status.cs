using System;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Status :IEquatable<Status>{
    public String Name;
    
    public float HP ;
    public float OldHP{
        get{
            return HP;
        }
        private set{}
    }
    public float MP { get; set; }
    public Status(String name,float _HP){
        Name=name;
        HP=_HP;
    }
    public bool Equals(Status obj)  
    {  
        return (this.Name == obj.Name);  
    }  
  
    public override bool Equals(Object obj)  
    {  
        if (obj == null)  
            return base.Equals(obj);  
        if (obj is Status)  
            return Equals(obj as Status);  
        return false;  
    }  
    public override int GetHashCode()  
    {  
        return Name.GetHashCode();  
    }  
}