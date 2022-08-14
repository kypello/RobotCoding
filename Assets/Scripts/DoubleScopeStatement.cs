using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DoubleScopeStatement")]
public class DoubleScopeStatement : SingleScopeStatement
{
    public Statement divider;

    void Awake() {
        Debug.Log("StatementAwake!!");

        codeBlock = new CodeBlock();
        codeBlock.hostStatement = this;
        codeBlock.statements.Add(Instantiate(divider));
        codeBlock.statements.Add(Instantiate(scopeEnder));

        InitializeSegments();

        hasMultiChoiceSegments = true;
    }

    public override string GetCollapsedSuffix() {
        string suffix = "";
        int lines = 0;
        int linesSinceBracket = 0;
        while (lines < codeBlock.statements.Count) {
            if (codeBlock.statements[lines].displayOnParentScope) {
                if (linesSinceBracket == 0) {
                    suffix += " }";
                }
                else {
                    suffix += " ... }";
                }
                linesSinceBracket = -1;

                if (lines < codeBlock.statements.Count - 1) {
                    suffix += " <key>else<def> {";
                }
            }

            lines++;
            linesSinceBracket++;
        }

        return suffix;
    }
}
