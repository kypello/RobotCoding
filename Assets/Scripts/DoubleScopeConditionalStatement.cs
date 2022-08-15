using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DoubleScopeConditionalStatement")]
public class DoubleScopeConditionalStatement : SingleScopeConditionalStatement
{
    public Statement divider;

    protected override void InsertStatements() {
        codeBlock.InsertStatement(Instantiate(divider), 0);
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
