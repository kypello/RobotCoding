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
    }
}
