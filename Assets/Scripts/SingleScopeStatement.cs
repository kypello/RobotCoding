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
        codeBlock.InsertStatement(Instantiate(scopeEnder), 0);

        InitializeSegments();
        InsertStatements();
    }

    protected virtual void InsertStatements() {}

    public override string GetCollapsedSuffix() {
        if (codeBlock.statements.Count > 1) {
            return " ... }";
        }
        else {
            return " }";
        }
    }
}
