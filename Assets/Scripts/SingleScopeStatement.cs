using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SingleScopeStatement")]
public class SingleScopeStatement : Statement
{
    public Statement scopeEnder;

    void Awake() {
        Debug.Log("StatementAwake!!");

        codeBlock = new CodeBlock();
        codeBlock.hostStatement = this;
        codeBlock.statements.Add(Instantiate(scopeEnder));

        InitializeSegments();

        hasMultiChoiceSegments = true;
    }

    public override void InitializeSegments() {
        highlightableSegments = new HighlightableSegment[3];
        highlightableSegments[0] = new MultiChoiceSegment(new string[]{"<fun>checkBelow<def>() ", "<fun>checkInFront<def>() "}, "[a]");
        highlightableSegments[1] = new MultiChoiceSegment(new string[]{ "<key>==<def> ", "<key>!=<def> "}, "[b]");
        highlightableSegments[2] = new MultiChoiceSegment(new string[]{"<val>red<def>", "<val>blue<def>"}, "[c]");
    }

    public override string GetCollapsedSuffix() {
        if (codeBlock.statements.Count > 1) {
            return " ... }";
        }
        else {
            return " }";
        }
    }
}
