using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class Battle : MonoBehaviour {
    public int HP = 10;
    void Start () {
        // タップダウンストリームを作成
        var tapDownStream = this.UpdateAsObservable ()
            .Where (_ => Input.GetMouseButtonDown (0));
        tapDownStream
            .Select (_ => 1)
            .Scan ((sum, addCount) => sum + addCount)
            .Where (totalCount => HP < totalCount)
            .Subscribe (totalCount => {
                Destroy (this.gameObject);
                Debug.Log (totalCount);
            }).AddTo (this.gameObject);
    }
}