using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiChoiceSegment
{
    public string[] choices = new string[1];
    public int selectedChoice = 0;
    public readonly string correspondingSegmentInStatement;

    public float startBound;
    public float endBound;

    public MultiChoiceSegment(string[] c, string s) {
        choices = c;
        correspondingSegmentInStatement = s;
    }

    public string GetChoice() {
        return choices[selectedChoice];
    }

    public void SelectChoice(int choice) {
        selectedChoice = choice;
    }
}
