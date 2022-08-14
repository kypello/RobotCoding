using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiChoiceSegment
{
    string[] choices = new string[1];
    int selectedChoice = 0;
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
}
