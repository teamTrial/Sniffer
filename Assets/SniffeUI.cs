using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniffeUI : MonoBehaviour {
	SpriteRenderer UI;
	/// <summary>
	/// Start is called on the frame when a script is enabled just before
	/// any of the Update methods is called the first time.
	/// </summary>
	void Start()
	{
		UI=this.GetComponent<SpriteRenderer>();
	}
    public void CheckSnifferActionFlag () {
        if (PlayerController.SnifferActionFlag) {
			UI.enabled=true;
		} else {
			UI.enabled=false;
		}
    }
}