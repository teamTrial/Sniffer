using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageManager : MonoBehaviour {
    public int EnemyNum=0;
    public int ClearNum=10;
    public GameObject obj1,obj2;
    public Text text;
    int num;
    void Start () {
        EnemyNum=0;
    }
    
    void Update () {
        if(ClearNum<EnemyNum){
            Destroy(obj1);
            Destroy(obj2);
            return;
        }
    }
    public void UpdateNum(){
        num++;
        if(ClearNum<=num){
			text.text="条件クリア　←　→　に進め";
			return;
			}
        text.text="食べた人の数="+num.ToString();
    }
}
