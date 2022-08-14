using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiChoiceSegment : HighlightableSegment
{
    public string[] choices = new string[1];
    public int selectedChoice = 0;
    const float canvasWidth = 2560f;

    public MultiChoiceSegment(string[] c, string s) {
        choices = c;
        correspondingSegmentInStatement = s;
    }

    public override string GetText() {
        return choices[selectedChoice];
    }

    public override void Click(StatementSlot slot) {
        DropdownMenu.instance.Display(slot, this, new Vector2((Input.mousePosition.x / Screen.width) * canvasWidth, slot.rectTransform.anchoredPosition.y + 1256));
    }

    public void SelectChoice(int choice) {
        selectedChoice = choice;
    }
}
