using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DropdownMenu : MonoBehaviour
{
    public static DropdownMenu instance;

    const float lineHeight = 60f;
    const float characterWidth = 29f;
    const float canvasWidth = 2560f;
    const float canvasHeight = 1440f;

    const float rightBound = 2430f;
    const float leftBound = 710f;
    const float bottomBound = 24f;

    RectTransform rectTransform;

    int itemCount;
    public List<TMP_Text> itemSlots = new List<TMP_Text>();
    List<MouseOver> itemSlotMouseOvers = new List<MouseOver>();

    bool displaying = false;
    MouseOver mouseOver;

    int currentlyHighlighted = -1;
    const string highlightTag = "<mark=#FFFFFF11>";

    public RectTransform panel;
    MultiChoiceSegment multiChoiceSegment;
    StatementSlot slot;

    void Awake() {
        instance = this;

        rectTransform = GetComponent<RectTransform>();
        mouseOver = GetComponent<MouseOver>();
        foreach (TMP_Text itemSlot in itemSlots) {
            itemSlotMouseOvers.Add(itemSlot.GetComponent<MouseOver>());
        }

        for (int i = 0; i < itemSlots.Count; i++) {
            itemSlots[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(12f, -12f - i * lineHeight);
        }

        gameObject.SetActive(false);
    }

    public void Display(StatementSlot sl, MultiChoiceSegment seg, Vector2 position) {
        string[] items = seg.choices;
        multiChoiceSegment = seg;
        slot = sl;

        gameObject.SetActive(true);
        int longestLineLength = 0;
        int i = 0;
        while (i < items.Length) {
            itemSlots[i].gameObject.SetActive(true);
            itemSlots[i].text = StatementSlot.ColorString(items[i]);

            int lineLength = StatementSlot.CleanString(items[i]).Length;
            if (lineLength > longestLineLength) {
                longestLineLength = lineLength;
            }

            i++;
        }

        while (i < itemSlots.Count) {
            itemSlots[i].gameObject.SetActive(false);
            i++;
        }

        for (int j = 0; j < items.Length; j++) {
            itemSlots[j].GetComponent<RectTransform>().sizeDelta = new Vector2(longestLineLength * characterWidth, 70f);
        }

        foreach (MouseOver itemMouseOver in itemSlotMouseOvers) {
            itemMouseOver.mouseOver = false;
        }
        mouseOver.mouseOver = true;

        rectTransform.sizeDelta = new Vector2(longestLineLength * characterWidth + 74f, 70f);
        panel.sizeDelta = new Vector2(longestLineLength * characterWidth + 50f, lineHeight * items.Length + 24f);

        if (position.x - rectTransform.sizeDelta.x / 2f < leftBound) {
            position.x = leftBound + rectTransform.sizeDelta.x / 2f;
        }
        if (position.x + rectTransform.sizeDelta.x / 2f > rightBound) {
            position.x = rightBound - rectTransform.sizeDelta.x / 2f;
        }
        if (position.y - rectTransform.sizeDelta.y / 2f - panel.sizeDelta.y < bottomBound) {
            panel.pivot = new Vector2(0, 0);
            panel.anchoredPosition = new Vector2(12f, 70f);
        }
        else {
            panel.pivot = new Vector2(0, 1);
            panel.anchoredPosition = new Vector2(12f, 0f);
        }
        rectTransform.anchoredPosition = position;

        itemCount = items.Length;
        currentlyHighlighted = -1;
        displaying = true;
    }

    void Update() {
        if (displaying) {
            if (!mouseOver.mouseOver) {
                displaying = false;
                gameObject.SetActive(false);
                return;
            }

            for (int i = 0; i < itemCount; i++) {
                if (itemSlotMouseOvers[i].mouseOver) {
                    if (currentlyHighlighted != i) {
                        if (currentlyHighlighted != -1) {
                            itemSlots[currentlyHighlighted].text = itemSlots[currentlyHighlighted].text.Remove(0, 16);
                        }
                        itemSlots[i].text = itemSlots[i].text.Insert(0, highlightTag);
                        currentlyHighlighted = i;
                    }

                    if (Input.GetMouseButtonDown(0)) {
                        if (multiChoiceSegment.selectedChoice != i) {
                            multiChoiceSegment.SelectChoice(i);
                            slot.CalculateSegmentBounds();
                            slot.ForceUpdate();
                        }
                        displaying = false;
                        gameObject.SetActive(false);
                        return;
                    }
                }
                else {
                    if (currentlyHighlighted == i) {
                        itemSlots[i].text = itemSlots[i].text.Remove(0, 16);
                        currentlyHighlighted = -1;
                    }
                }
            }
        }
    }
}
