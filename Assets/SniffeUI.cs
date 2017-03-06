using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniffeUI : MonoBehaviour {
	SpriteRenderer UI;
	Transform parent;
	Vector3 DefSize;
	int MaxHP,afterHP;
	/// <summary>
	/// Start is called on the frame when a script is enabled just before
	/// any of the Update methods is called the first time.
	/// </summary>
	void Start()
	{
		UI=this.GetComponent<SpriteRenderer>();
		parent=transform.parent.transform;
		DefSize=this.transform.position;
	}
	/// <summary>
	/// Update is called every frame, if the MonoBehaviour is enabled.
	/// </summary>
	void Update()
	{
		CheckSnifferActionFlag();
		CheckDir();
	}
    public void CheckSnifferActionFlag () {
		this.transform.position=parent.position+(Vector3.up*0.15f);
        if (PlayerController.SnifferActionFlag) {
			UI.enabled=true;
		} else {
			UI.enabled=false;
		}
    }
	void CheckDir(){
		var Parent=transform.parent;
		if(Parent.localScale.x<0){
			UI.flipX=true;
		}else{
			UI.flipX=false;
		}
	}
}