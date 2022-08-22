using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollableArea : MonoBehaviour
{
    IScrollable slotManager;

    int scrollLength = 0;
    int offset = 0;

    public float scrollbarBackLength = 1260;
    //public float scrollbarCenter = 0;
    public float intervalHeight = 24;
    public float scrollbarThickness = 24;

    public GameObject fullScrollbar;
    public RectTransform scrollbar;
    public MouseOver scrollbarMouseOver;
    public Image[] scrollbarImages;
    public Color defaultColor;
    public Color highlightColor;

    public float minMouseX;
    public float maxMouseX;

    public bool isHorizontal;
    Vector2 thicknessVector;
    Vector2 lengthVector;

    const float canvasHeight = 1440f;
    const float canvasWidth = 2560f;
    bool mouseDragging = false;
    float mouseOffset;
    float scrollbarBounds;
    float intervalSize;

    void Awake() {
        slotManager = GetComponent<IScrollable>();

        if (isHorizontal) {
            thicknessVector = Vector2.up;
            lengthVector = Vector2.right;
        }
        else {
            thicknessVector = Vector2.right;
            lengthVector = Vector2.up;
        }

        UpdateScrollbarSize();
    }

    void Update() {
        if (!mouseDragging) {
            if (scrollbarMouseOver.mouseOver) {
                if (!scrollbarMouseOver.mouseOverLastFrame) {
                    foreach (Image image in scrollbarImages) {
                        image.color = highlightColor;
                    }
                }

                if (Input.GetMouseButtonDown(0)) {
                    mouseDragging = true;
                    if (isHorizontal) {
                        mouseOffset = (Input.mousePosition.x / Screen.width) - (scrollbar.anchoredPosition.x / canvasWidth);
                    }
                    else {
                        mouseOffset = (Input.mousePosition.y / Screen.height) - (scrollbar.anchoredPosition.y / canvasHeight);
                    }
                }
            }
            else if (scrollbarMouseOver.mouseOverLastFrame) {
                foreach (Image image in scrollbarImages) {
                    image.color = defaultColor;
                }
            }
        }

        if (mouseDragging) {
            float mousePosition;
            if (isHorizontal) {
                mousePosition = Mathf.Clamp((Input.mousePosition.x / Screen.width - mouseOffset) * canvasWidth, -scrollbarBounds, scrollbarBounds);
            }
            else {
                mousePosition = Mathf.Clamp((Input.mousePosition.y / Screen.height - mouseOffset) * canvasHeight, -scrollbarBounds, scrollbarBounds);
            }
            scrollbar.anchoredPosition = lengthVector * mousePosition;

            int closestPosition = 0;
            float closestPositionDistance = Mathf.Infinity;
            for (int i = 0; i <= scrollLength; i++) {
                float positionCheck = scrollbarBounds - intervalSize * i;
                float dist = Mathf.Abs(positionCheck - mousePosition);
                if (dist < closestPositionDistance) {
                    closestPositionDistance = dist;
                    closestPosition = i;
                }
            }

            int delta = closestPosition - offset;
            if (delta != 0) {
                offset += delta;
                slotManager.UpdateScrollOffset(delta);
            }

            if (!Input.GetMouseButton(0)) {
                mouseDragging = false;
                if (!scrollbarMouseOver.mouseOver) {
                    foreach (Image image in scrollbarImages) {
                        image.color = defaultColor;
                    }
                }
                UpdateScrollbarPosition();
            }
        }
        else {
            float mouseX = Input.mousePosition.x / Screen.width;
            bool mouseInRange = mouseX >= minMouseX && mouseX <= maxMouseX;

            if (mouseInRange && Input.mouseScrollDelta.y > 0.1f && offset > 0) {
                offset--;
                UpdateScrollbarPosition();
                slotManager.UpdateScrollOffset(-1);
            }
            else if (mouseInRange && Input.mouseScrollDelta.y < -0.1f && offset < scrollLength) {
                offset++;
                UpdateScrollbarPosition();
                slotManager.UpdateScrollOffset(1);
            }
        }
    }

    public int GetOffset() {
        return offset;
    }

    public bool IsAtMaxScroll() {
        return offset == scrollLength;
    }

    public void SetOffset(int o) {
        offset = Mathf.Clamp(o, 0, scrollLength);
        UpdateScrollbarPosition();
    }

    public void SetScrollLength(int l) {
        bool wasAtMax = offset == scrollLength && offset > 0;
        scrollLength = l;
        if (offset > scrollLength || wasAtMax) {
            offset = scrollLength;
        }
        UpdateScrollbarSize();
    }

    void UpdateScrollbarSize() {
        if (scrollLength <= 0) {
            fullScrollbar.SetActive(false);
        }
        else {
            fullScrollbar.SetActive(true);

            float scrollbarLength = scrollbarBackLength - scrollLength * intervalHeight;
            scrollbar.sizeDelta = thicknessVector * scrollbarThickness + lengthVector * scrollbarLength;
            scrollbarBounds = (scrollbarBackLength / 2) - (scrollbarLength / 2);
            intervalSize = (scrollbarBackLength - scrollbarLength) / scrollLength;

            UpdateScrollbarPosition();
        }
    }

    void UpdateScrollbarPosition() {
        scrollbar.anchoredPosition = lengthVector * (scrollbarBounds - intervalSize * offset);
    }
}
