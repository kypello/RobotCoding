using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SingleScopeConditionalStatement")]
public class SingleScopeConditionalStatement : SingleScopeStatement
{
    public override void InitializeSegments() {
        hasMultiChoiceSegments = true;

        highlightableSegments = new HighlightableSegment[3];
        highlightableSegments[2] = new DependentMultiChoiceSegment(new string[0], "[c]", new string[][]{new string[]{"<val>button<def>", "<val>crate<def>", "<val>metal<def>"}, new string[]{"<val>button<def>", "<val>crate<def>", "<val>metal<def>", "<val>wall<def>", "<val>pit<def>"}});
        highlightableSegments[0] = new DependencyMultiChoiceSegment(new string[]{"<fun>checkBelow<def>() ", "<fun>checkInFront<def>() "}, "[a]", highlightableSegments[2] as DependentMultiChoiceSegment);
        highlightableSegments[1] = new MultiChoiceSegment(new string[]{ "<key>==<def> ", "<key>!=<def> "}, "[b]");

    }
}
