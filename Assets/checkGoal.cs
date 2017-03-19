using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// デモ用の暫定処置
/// </summary>

public class checkGoal : MonoBehaviour {
    public string checkColor;
	public enum Dir{
		left,
		right
	};
	
	public Dir _dir;
	bool timer;
	float time;
	

	/// <summary>
	/// Update is called every frame, if the MonoBehaviour is enabled.
	/// </summary>
	void Update()
	{
		if(timer){
			time+=Time.deltaTime;
		}
		if(time>2){
			timer=false;
			time=0;
			FadeManager.Instance.LoadLevel("localSelect",2);
			Destroy(GameObject.Find("Manager"));
			GameObject.Find ("UI/Controller").GetComponent<PlayerController> ().enabled=false;
		}
	}
    /// <summary>
    /// Sent when an incoming collider makes contact with this object's
    /// collider (2D physics only).
    /// </summary>
    /// <param name="Player">The Collision2D data associated with this collision.</param>
    void OnCollisionEnter2D (Collision2D Player) {
        if (Player.gameObject.tag == "Player") {
            if (Player.gameObject.GetComponent<people> ().NPCColor == checkColor) {
                this.GetComponent<BoxCollider2D> ().isTrigger = true;
				time=0;
				timer=true;
            }
        }
    }
    void OnCollisionExit2D (Collision2D Player) {
        if (Player.gameObject.tag == "Player") {
            if (Player.gameObject.GetComponent<people> ().NPCColor == checkColor) {
                this.GetComponent<BoxCollider2D> ().isTrigger = false;
				timer=false;
				time=0;
            }
        }
    }
}