using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : SingletonMonoBehaviour<AudioManager> {
    private const string BGM_PATH = "Audio/BGM";
    private const string SE_PATH = "Audio/SE";
    public int MaxSE = 10;
    private AudioSource bgmSource = null;
    private List<AudioSource> seSources = null;
    private Dictionary<string , AudioClip> bgmDict = null;
    private Dictionary<string , AudioClip> seDict = null;

    public void Awake() {
        if ( this != Instance ) {
            Destroy(this);
            return;
        }

        DontDestroyOnLoad(this.gameObject);

        //create listener
        if ( FindObjectsOfType(typeof(AudioListener)).All(o => !( (AudioListener)o ).enabled) ) {
            this.gameObject.AddComponent<AudioListener>( );
        }
        //create audio sources
        this.bgmSource = this.gameObject.AddComponent<AudioSource>( );
        this.seSources = new List<AudioSource>( );

        //create clip dictionaries
        this.bgmDict = new Dictionary<string , AudioClip>( );
        this.seDict = new Dictionary<string , AudioClip>( );

        object[] bgmList = Resources.LoadAll(BGM_PATH);
        object[] seList = Resources.LoadAll(SE_PATH);

        foreach ( AudioClip bgm in bgmList ) {
            this.bgmDict[bgm.name] = bgm;
        }
        foreach ( AudioClip se in seList ) {
            this.seDict[se.name] = se;
        }
    }

    public void PlaySE(string seName) {
        if ( !this.seDict.ContainsKey(seName) ) throw new ArgumentException(seName + " not found" , "seName");

        AudioSource source = this.seSources.FirstOrDefault(s => !s.isPlaying);
        if ( source == null ) {
            if ( this.seSources.Count >= this.MaxSE ) {
                Debug.Log("SE AudioSource is full");
                return;
            }
            source = this.gameObject.AddComponent<AudioSource>( );
            this.seSources.Add(source);
        }
        source.clip = this.seDict[seName];
        source.Play( );
    }

    public void StopSE() {
        this.seSources.ForEach(s => s.Stop( ));
    }

    public void PlayBGM(string bgmName) {
        if ( !this.bgmDict.ContainsKey(bgmName) ) throw new ArgumentException(bgmName + " not found" , "bgmName");
        if ( this.bgmSource.clip == this.bgmDict[bgmName] ) return;
        this.bgmSource.Stop( );
        this.bgmSource.clip = this.bgmDict[bgmName];
        this.bgmSource.Play( );
    }
    public float PlayBGMLength(string bgmName) {
        this.bgmSource.clip = this.bgmDict[bgmName];
        return this.bgmSource.clip.length;
    }
    public void StopBGM() {
        this.bgmSource.Stop( );
        this.bgmSource.clip = null;
    }


}