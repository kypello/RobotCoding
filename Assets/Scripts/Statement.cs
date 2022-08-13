using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Statement")]
public class Statement : ScriptableObject
{
    public string statementText;

    public CodeBlock parentCodeBlock = null;
    public CodeBlock codeBlock = null;

    public int scope;
    public bool isScopeEnder = false;
}
