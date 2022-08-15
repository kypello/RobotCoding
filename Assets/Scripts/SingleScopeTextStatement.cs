using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SingleScopeTextStatement")]
public class SingleScopeTextStatement : SingleScopeStatement
{
    public override void InitializeSegments() {
        highlightableSegments = new HighlightableSegment[1];
        highlightableSegments[0] = new TextInputSegment("2", "[a]", true, false);
    }
}
