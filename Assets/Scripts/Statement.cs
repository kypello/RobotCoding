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
    public bool displayOnParentScope = false;
    public bool invisibleInCode = false;

    public HighlightableSegment[] highlightableSegments = new HighlightableSegment[0];
    public bool hasMultiChoiceSegments = false;

    public string id;

    public virtual void InitializeSegments() {}

    public virtual string GetCollapsedSuffix() {
        return "";
    }
}
