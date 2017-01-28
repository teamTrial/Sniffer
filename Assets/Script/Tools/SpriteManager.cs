using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
//Resourcesから画像を読み込むクラス
public class SpriteManager : SingletonMonoBehaviour<SpriteManager> {
    private const string SPRITE_PATH = "Sprite";
    private Dictionary<string , Sprite> spriteDict = null;
    void Awake() {
        if ( this != Instance ) {
            Destroy(this);
            return;
        }
        DontDestroyOnLoad(this.gameObject);
        this.spriteDict = new Dictionary<string , Sprite>( );
        Texture[] spriteList = Resources.LoadAll<Texture>(SPRITE_PATH);
        for(int i=0 ;i<spriteList.Length ;i++ ) {
            var getSprite = Resources.Load<Sprite>(SPRITE_PATH + "/" + spriteList[i].name);
            this.spriteDict[getSprite.name] = getSprite;
        }
    }
    public Sprite LoadSprite(string spriteName) {
        if ( !this.spriteDict.ContainsKey(spriteName) ) throw new ArgumentException(
            spriteName + " がありませんでした。タイプミスでないのでしたら、InspectorViewから"+spriteName+"のTextureTypeがSpriteになってるか確認してください。" , "spriteName");
        return spriteDict[spriteName];
    }
}
