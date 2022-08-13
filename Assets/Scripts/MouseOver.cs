using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseOver : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool mouseOver;
    public bool mouseOverLastFrame;

    public void OnPointerEnter(PointerEventData eventData) {
        mouseOver = true;
    }

    public void OnPointerExit(PointerEventData eventData) {
        mouseOver = false;
    }

    void LateUpdate() {
        mouseOverLastFrame = mouseOver;
    }
}
