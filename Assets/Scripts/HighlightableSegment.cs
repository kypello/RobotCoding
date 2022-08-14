using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HighlightableSegment
{
    public string correspondingSegmentInStatement;

    public float startBound;
    public float endBound;

    public abstract string GetText();

    public abstract void Click(StatementSlot slot);
}
