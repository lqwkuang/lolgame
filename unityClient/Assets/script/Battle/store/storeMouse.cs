using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class storeMouse : MonoBehaviour,IDragHandler
{
    private RectTransform rectrans;
    private void Awake()
    {
        rectrans =transform.parent.parent.GetComponent<RectTransform>();
    }
    public void OnDrag(PointerEventData eventData)
    {
        rectrans.anchoredPosition += eventData.delta;
    }
}
