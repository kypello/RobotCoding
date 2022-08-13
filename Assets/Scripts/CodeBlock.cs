using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CodeBlock
{
    public Statement hostStatement = null;

    public List<Statement> statements = new List<Statement>();

    public int scope;

    public void SetScope(int s) {
        scope = s;
        foreach (Statement statement in statements) {
            if (statement.codeBlock != null) {
                statement.codeBlock.SetScope(s+1);
            }
        }
    }

    public void RemoveStatement(Statement statement) {
        statements.Remove(statement);
    }

    /*
    public void InsertStatement(Statement statement, int targetIndex, int globalIndex = 0, bool endGoesToEnclosingScope = false) {
        int localIndex = 0;
        while (localIndex < statements.Count && globalIndex < targetIndex) {
            if (statements[localIndex].codeBlock != null) {
                statements[localIndex].codeBlock.InsertStatement(statement, targetIndex, globalIndex + 1, true);
                globalIndex += statements[localIndex].codeBlock.GetLineCount();
            }

            localIndex++;
            globalIndex++;
        }

        if (globalIndex == targetIndex) {
            if (localIndex == statements.Count - 1 && endGoesToEnclosingScope) {
                hostStatement.parentCodeBlock.statements.Insert(hostStatement.parentCodeBlock.statements.IndexOf(hostStatement) + 1, statement);
                statement.parentCodeBlock = hostStatement.parentCodeBlock;
                if (statement.codeBlock != null)  {
                    statement.codeBlock.SetScope(scope);
                }
            }
            else {
                statements.Insert(localIndex, statement);
                statement.parentCodeBlock = this;
                if (statement.codeBlock != null)  {
                    statement.codeBlock.SetScope(scope+1);
                }
            }
        }
    }
    */

    public void InsertStatement(Statement statement, int index) {
        statements.Insert(index, statement);
        statement.parentCodeBlock = this;
        if (statement.codeBlock != null)  {
            statement.codeBlock.SetScope(scope + 1);
        }
    }

    public int GetLineCount() {
        int lineCount = 0;
        foreach (Statement statement in statements) {
            lineCount++;
            if (statement.codeBlock != null) {
                lineCount += statement.codeBlock.GetLineCount();
            }
        }
        return lineCount;
    }

    public bool ContainsCodeBlock(CodeBlock codeBlock) {
        if (codeBlock == this) {
            return true;
        }

        foreach (Statement statement in statements) {
            if (statement.codeBlock != null && statement.codeBlock.ContainsCodeBlock(codeBlock)) {
                return true;
            }
        }

        return false;
    }
}
