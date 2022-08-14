using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SingleScopeStatement")]
public class SingleScopeStatement : Statement
{
    public Statement scopeEnder;

    void Awake() {
        codeBlock = new CodeBlock();
        codeBlock.hostStatement = this;
        codeBlock.statements.Add(Instantiate(scopeEnder));

        multiChoiceSegments = new MultiChoiceSegment[3];
        multiChoiceSegments[0] = new MultiChoiceSegment(new string[]{"checkBelow()", "checkInFront()"}, "[a]");
        multiChoiceSegments[1] = new MultiChoiceSegment(new string[]{ "==", "!="}, "[b]");
        multiChoiceSegments[2] = new MultiChoiceSegment(new string[]{"red", "blue"}, "[c]");
    }
}
