using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
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

    protected int currentItemNo = 0
;

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

    protected override void Start () {
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
            item.anchoredPosition = direction == Direction.Vertical ? new Vector2 (0, -itemScale * i) : new Vector2 (itemScale * i, 0);
            itemList.AddLast (item);

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
        Observable.Timer (TimeSpan.FromSeconds (2), TimeSpan.FromSeconds (1))
            .Subscribe (_ => {
                var item = itemList.First.Value;
                itemList.RemoveFirst ();
                itemList.AddLast (item);
                currentItemNo++;
                if (instantateItemCount < currentItemNo) {
                    currentItemNo = 0;
                }
                //ここの処理をいい感じにすれば滑らかになりそう
                var pos = itemScale * currentItemNo;
                item.anchoredPosition = new Vector2 (-pos, 0);
                //アイテム情報の更新
                onUpdateItem.Invoke (currentItemNo + instantateItemCount, item.gameObject);
            });
    }

    [System.Serializable]
    public class OnItemPositionChange : UnityEngine.Events.UnityEvent<int, GameObject> { }
}