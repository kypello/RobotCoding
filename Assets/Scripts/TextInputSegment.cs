using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextInputSegment : HighlightableSegment
{
    public string text;
    bool number;
    bool negativesAllowed;

    public TextInputSegment(string t, string s, bool n, bool m) {
        text = t;
        correspondingSegmentInStatement = s;
        number = n;
        negativesAllowed = m;
    }

    public override string GetText() {
        return text;
    }

    public override void Click(StatementSlot slot) {
        TextInputMonitor.instance.StartMonitoring(this, slot, number, negativesAllowed);
    }

    public void SetText(string t) {
        text = t;
    }
}
