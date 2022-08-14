using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TextInputStatement")]
public class TextInputStatement : Statement
{
    public bool number;
    public bool negativesAllowed;
    public string defaultText;

    void Awake() {
        Debug.Log("StatementAwake!!");
        
        InitializeSegments();

        hasMultiChoiceSegments = false;
    }

    public override void InitializeSegments() {
        highlightableSegments = new HighlightableSegment[1];
        highlightableSegments[0] = new TextInputSegment(defaultText, "[a]", number, negativesAllowed);
    }
}
