using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public interface IInfiniteScrollSetup {
    void OnPostSetupItems ();
    void OnUpdateItem (int itemCount, GameObject obj);
}
public class InfiniteScroll : UIBehaviour {
    [SerializeField]
    private RectTransform itemPrototype;

    [SerializeField, Range (0, 30)]
    /// <summary>
    /// 表示するバナーの数
    /// </summary>
    int instantateItemCount = 9;

    public Direction direction;

    public OnItemPositionChange onUpdateItem = new OnItemPositionChange ();

    [System.NonSerialized]
    public LinkedList<RectTransform> itemList = new LinkedList<RectTransform> ();

    protected float diffPreFramePosition = 0;

    protected int currentItemNo = 0;

    public enum Direction {
        Vertical,
        Horizontal,
    }

    // cache component

    private RectTransform _rectTransform;
    protected RectTransform rectTransform {
        get {
            if (_rectTransform == null) _rectTransform = GetComponent<RectTransform> ();
            return _rectTransform;
        }
    }

    private float anchoredPosition {
        get {
            return direction == Direction.Vertical ? -rectTransform.anchoredPosition.y : rectTransform.anchoredPosition.x;
        }
    }

    private float _itemScale = -1;
    //プロトタイプのアイテムのサイズを取得
    public float itemScale {
        get {
            if (itemPrototype != null && _itemScale == -1) {
                _itemScale = direction == Direction.Vertical ? itemPrototype.sizeDelta.y : itemPrototype.sizeDelta.x;
            }
            return _itemScale;
        }
    }
    float startTime;
    float[] Pos;
    protected override void Start () {
        Pos = new float[instantateItemCount];

        var controllers = GetComponents<MonoBehaviour> ()
            .Where (item => item is IInfiniteScrollSetup)
            .Select (item => item as IInfiniteScrollSetup)
            .ToList ();

        // create items

        var scrollRect = GetComponentInParent<ScrollRect> ();
        scrollRect.horizontal = direction == Direction.Horizontal;
        scrollRect.vertical = direction == Direction.Vertical;
        scrollRect.content = rectTransform;

        itemPrototype.gameObject.SetActive (false);

        for (int i = 0; i < instantateItemCount; i++) {
            var item = GameObject.Instantiate (itemPrototype) as RectTransform;
            item.SetParent (transform, false);
            item.name = i.ToString ();
            // item.anchoredPosition = direction == Direction.Vertical ? new Vector2 (0, -itemScale * i) : new Vector2 (itemScale * i, 0);
            item.anchoredPosition = new Vector2 (-itemScale * (i - 1), 0);
            itemList.AddLast (item);
            Pos[i] = (int) item.anchoredPosition.x; //要素の数x座標を代入
            item.gameObject.SetActive (true);

            foreach (var controller in controllers) {
                controller.OnUpdateItem (i, item.gameObject);

            }
        }

        foreach (var controller in controllers) {
            controller.OnPostSetupItems ();
        }
        DisplayAd ();
    }

    //ニュースフィードを表示する機能
    void DisplayAd () {
        Observable.Timer (TimeSpan.FromSeconds (1), TimeSpan.FromSeconds (0.5f))
            .TakeWhile (clear => !StageClear.ClearFlag)
            .Subscribe (_ => {
                //戦闘の奴は一番後ろに
                var item = itemList.First.Value;
                itemList.RemoveFirst ();
                itemList.AddLast (item);
                var name = int.Parse (item.name);
                var PosX = (int) item.anchoredPosition.x;
                if (PosX < 0) {
                    //最後尾に移行する
                    item.anchoredPosition = new Vector2 (547, 0);
                } else if ((int) PosX == 0) {
                    item.anchoredPosition = new Vector2 (-547, 0);
                    // MoveBanner (item, new Vector2 (-547, 0));
                } else if (0 < PosX) {
                    item.anchoredPosition = new Vector2 (0, 0);
                    // MoveBanner (item, new Vector2 (0, 0));
                }
                onUpdateItem.Invoke (currentItemNo + instantateItemCount, item.gameObject);

                // print(PosX+"==="+item.name);

                // } else {
                //     float pos = name;
                //     if ((pos) == 0) {
                //         pos = Pos[name];
                //         // MoveBanner (item, new Vector2 (pos, 0));
                //     } else {
                //         pos = Pos[name - 1];
                //     }
                // }
                // print(item.name+"==="+Pos[name]);
                // onUpdateItem.Invoke (currentItemNo + instantateItemCount, item.gameObject);
                //一番後ろへ
                // var pos = itemScale * instantateItemCount - itemScale * currentItemNo;
                // print(item.anchoredPosition.x);

                // print (currentItemNo + " " + pos);
                // item.anchoredPosition = new Vector2 (pos, 0);

                // if (item.anchoredPosition.x < 0) {
                //     var pos = Pos[Pos.Length - 1];
                //     print(item.name+" "+pos);
                //     item.anchoredPosition = new Vector2 (pos, 0);
                // } else {
                //     if((currentItemNo - 1)<0){
                //         currentItemNo=0;
                //     }
                //     var pos = Pos[currentItemNo];
                //     MoveBanner (item, new Vector2 (pos, 0));
                // }
                // currentItemNo++;
                // if (instantateItemCount <= currentItemNo) {
                //     currentItemNo = 0;
                // }
                //アイテム情報の更新

            });
    }
    void MoveBanner (RectTransform item, Vector2 pos) {
        startTime = Time.timeSinceLevelLoad;
        this.UpdateAsObservable ()
            // .TakeWhile (_ => (int) item.anchoredPosition.x != (int) pos.x)
            .Subscribe (_ => {
                var diff = Time.timeSinceLevelLoad - startTime;
                var rate = diff;
                item.anchoredPosition = Vector2.Lerp (item.anchoredPosition, pos, rate);
            });
    }

    [System.Serializable]
    public class OnItemPositionChange : UnityEngine.Events.UnityEvent<int, GameObject> { }
}