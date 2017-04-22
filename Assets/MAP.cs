using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MAP : MonoBehaviour {
	public string stagenum;
	void Start()
	{
	ActiveStage(3);	
	}
	public void ActiveStage(int stagenum=1){
		//ステージのアクティブ化
		GameObject.Find("Canvas").transform.Find("stage"+stagenum).gameObject.SetActive(true);
	}
}