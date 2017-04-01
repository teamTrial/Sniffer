using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageClear : MonoBehaviour {
	public string SceneName;
	public float loadtime=2f;
	public static bool ClearFlag{get;set;}
	void Start () {
		ClearFlag=false;
	}
	/// <summary>
	/// Sent when another object enters a trigger collider attached to this
	/// object (2D physics only).
	/// </summary>
	/// <param name="other">The other Collider2D involved in this collision.</param>
	void OnTriggerEnter2D(Collider2D other)
	{	
		if(other.tag=="Player"){
			ClearFlag=true;
			FadeManager.Instance.LoadLevel(SceneName,loadtime);
			Destroy(GameObject.Find("Manager"));
		}
	}
}
