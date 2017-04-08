using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FixedScrollRect : ScrollRect {
    private const float POWER = 10;

    private InfiniteScroll _infinityScroll;
    private InfiniteScroll infinityScroll {
        get {
            if (_infinityScroll == null)
                _infinityScroll = GetComponentInChildren<InfiniteScroll> ();
            return _infinityScroll;
        }
    }

    public bool isDrag {
        get;
        private set;
    }

    private float Velocity {
        get {
            return (infinityScroll.direction == InfiniteScroll.Direction.Vertical) ?
                -velocity.y :
                velocity.x;
        }
    }

    private RectTransform _rectTransform;
    private float AnchoredPosition {
        get {
            if (_rectTransform == null)
                _rectTransform = transform.GetChild (0).GetComponent<RectTransform> ();
            return (infinityScroll.direction == InfiniteScroll.Direction.Vertical) ?
                -_rectTransform.anchoredPosition.y :
                _rectTransform.anchoredPosition.x;
        }
        set {
            if (infinityScroll.direction == InfiniteScroll.Direction.Vertical)
                _rectTransform.anchoredPosition = new Vector2 (0, -value);
            else
                _rectTransform.anchoredPosition = new Vector2 (value, 0);
        }
    }

    void Update () {

        // if (isDrag || Mathf.Abs (Velocity) > 200)
        // 	return;
		//アンカーの移動処理(AからBまでの間)
        float diff = AnchoredPosition % infinityScroll.itemScale;
        var adjust = infinityScroll.itemScale * -1f; //* ((AnchoredPosition > 0f) ? 1 : -1);
        AnchoredPosition += (adjust - diff) * Time.deltaTime * POWER;
        // if (Mathf.Abs (diff) > infinityScroll.itemScale / 2) {
        // 	var adjust = infinityScroll.itemScale * ((AnchoredPosition > 0f) ? 1 : -1);
        // 	AnchoredPosition += (adjust - diff) * Time.deltaTime * POWER;
        // } else {
        // 	AnchoredPosition -= diff * Time.deltaTime * POWER;
        // }
    }

    public override void OnBeginDrag (PointerEventData eventData) {
        base.OnBeginDrag (eventData); // 削除した場合、挙動に影響有り
        isDrag = true;
    }

    public override void OnEndDrag(PointerEventData eventData){
    	base.OnEndDrag (eventData);	// 削除した場合、挙動に影響有り
    	isDrag = false;
    }

}