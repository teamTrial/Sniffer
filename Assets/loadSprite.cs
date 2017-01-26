using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class loadSprite : MonoBehaviour {
	public string SpriteName;
	void Start()
	{
		var spritemanger=SpriteManager.Instance.LoadSprite(SpriteName);
		GetComponent<SpriteRenderer>().sprite=spritemanger;
	}
}
