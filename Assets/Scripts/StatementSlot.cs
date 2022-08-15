using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class StatementSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    TMP_Text text;
    public RectTransform rectTransform;

    const float lineHeight = 60f;
    const float characterWidth = 29f;
    const float codeLeftmostPoint = 750f;
    const float canvasWidth = 2560f;
    ISlotManager manager;

    bool mouseOver;
    int fixedIndex;

    public Statement statement = null;
    bool highlighted = false;
    int lineNumber;
    bool fullLineHighlight;

    public int globalIndex;
    public int localIndex;
    public CodeBlock codeBlock;
    public bool freeFloating = false;

    bool updateNeeded = true;
    public int highlightedSegment = -1;
    public bool hideSegments;
    public bool collapsed = false;

    public void SetUp(int i, ISlotManager m, Vector2 size, float extraSpace = 0) {
        manager = m;
        fixedIndex = i;

        text = GetComponent<TMP_Text>();
        rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(0f, -(lineHeight + extraSpace) * fixedIndex);
        rectTransform.sizeDelta = size;

        SetLineNumber(fixedIndex);

        updateNeeded = true;
    }

    string GetLineNumberString() {
        string color;
        if (fullLineHighlight) {
            color = "<color=#FFFFFFAA>";
        }
        else {
            color = "<color=#FFFFFF22>";
        }
        if (lineNumber < 9) {
            return color + " " + (lineNumber + 1) + "</color>    ";
        }
        else {
            return color + (lineNumber + 1) + "</color>    ";
        }
    }

    public void SetLineNumber(int n) {
        if (lineNumber != n) {
            lineNumber = n;
            updateNeeded = true;
        }
    }

    public void SetStatement(Statement s) {
        if (statement != s) {
            statement = s;
            updateNeeded = true;

            if (statement != null && statement.highlightableSegments.Length > 0) {
                CalculateSegmentBounds();
            }
        }
    }

    public void SetHighlighted(bool h) {
        if (highlighted != h) {
            highlighted = h;
            updateNeeded = true;

            SetSegmentHighlight();
        }
    }

    public void SetFullLineHighlight(bool h) {
        if (fullLineHighlight != h) {
            fullLineHighlight = h;
            updateNeeded = true;
        }
    }

    public void ForceUpdate() {
        updateNeeded = true;
    }

    void LateUpdate() {
        if (updateNeeded) {
            Display();
            updateNeeded = false;
        }
    }

    int GetScope() {
        if (codeBlock != null) {
            return codeBlock.scope;
        }
        return 0;
    }

    void Display() {
        string fullText = "";

        if (!freeFloating) {
            fullText += GetLineNumberString();

            int scope = codeBlock.scope;
            if (statement != null && statement.displayOnParentScope) {
                scope--;
            }

            for (int i = 0; i < scope; i++) {
                fullText += "    ";
            }
        }

        if (highlighted && highlightedSegment == -1) {
            fullText += "<mark=#FFFFFF11>";

            if (statement == null) {
                fullText += "<color=#FFFFFF00>";
                int characterCount = CleanString(manager.DragDropManager.statementBeingDragged.statementText).Length;
                for (int i = 0; i < characterCount; i++) {
                    fullText += 'a';
                }
            }
        }

        if (statement != null) {
            string statementText = statement.statementText;

            if (statement.highlightableSegments.Length > 0) {
                for (int i = 0; i < statement.highlightableSegments.Length; i++) {
                    if (highlighted && highlightedSegment == i) {
                        statementText = statementText.Replace(statement.highlightableSegments[i].correspondingSegmentInStatement, "<mark=#FFFFFF11>" + statement.highlightableSegments[i].GetText() + "</mark>");
                    }
                    else {
                        if (hideSegments && statement.hasMultiChoiceSegments) {
                            statementText = statementText.Replace(statement.highlightableSegments[i].correspondingSegmentInStatement, "");
                        }
                        else {
                            statementText = statementText.Replace(statement.highlightableSegments[i].correspondingSegmentInStatement, statement.highlightableSegments[i].GetText());
                        }
                    }
                }
            }

            fullText += statementText;

            if (collapsed) {
                fullText += statement.GetCollapsedSuffix();
            }

            if (statement.invisibleInCode && !freeFloating) {
                fullText = fullText.Replace("<com>", "<inv>");
            }
        }

        text.text = ColorString(fullText);
    }

    public void CalculateSegmentBounds() {
        string statementText = statement.statementText;
        foreach (HighlightableSegment highlightableSegment in statement.highlightableSegments) {
            statementText = statementText.Replace(highlightableSegment.correspondingSegmentInStatement, highlightableSegment.GetText());
        }

        foreach (HighlightableSegment highlightableSegment in statement.highlightableSegments) {
            int segmentStartCharacter = CleanString(statementText).IndexOf(CleanString(highlightableSegment.GetText()));
            int segmentEndCharacter = segmentStartCharacter + CleanString(highlightableSegment.GetText()).Length;

            highlightableSegment.startBound = (codeLeftmostPoint + (6 + GetScope() * 4 + segmentStartCharacter) * characterWidth) / canvasWidth;
            highlightableSegment.endBound = (codeLeftmostPoint + (6 + GetScope() * 4 + segmentEndCharacter) * characterWidth) / canvasWidth;
        }
    }

    public void SetSegmentHighlight() {
        int highlight = CheckIfHighlightingSegment();
        if (highlight != highlightedSegment) {
            highlightedSegment = highlight;
            updateNeeded = true;
        }
    }

    int CheckIfHighlightingSegment() {
        if (statement == null) {
            return -1;
        }

        for (int i = 0; i < statement.highlightableSegments.Length; i++) {
            float mousePos = Input.mousePosition.x / Screen.width;

            if (mousePos >= statement.highlightableSegments[i].startBound && mousePos < statement.highlightableSegments[i].endBound) {
                return i;
            }
        }

        return -1;
    }

    public static string InsertSegments(Statement s, bool hideMultiChoice) {
        string text = s.statementText;
        if (s.hasMultiChoiceSegments && hideMultiChoice) {
            for (int i = 0; i < s.highlightableSegments.Length; i++) {
                text = text.Replace(s.highlightableSegments[i].correspondingSegmentInStatement, "");
            }
        }
        else {
            for (int i = 0; i < s.highlightableSegments.Length; i++) {
                text = text.Replace(s.highlightableSegments[i].correspondingSegmentInStatement, s.highlightableSegments[i].GetText());
            }
        }

        return text;
    }

    public static string CleanString(string s) {
        while (s.Contains("<")) {
            int a = s.IndexOf("<");
            int b = s.IndexOf(">");
            s = s.Remove(a, (b-a)+1);
        }
        return s;
    }

    public static string ColorString(string s) {
        return s.Replace("<fun>", "<color=#FF8888>").Replace("<val>", "<color=#AAFFAA>").Replace("<def>", "</color>").Replace("<key>", "<color=#FF88FF>").Replace("<com>", "<color=#FFFFFF22>").Replace("<inv>", "<color=#FFFFFF00>");
    }

    public void OnPointerEnter(PointerEventData eventData) {
        mouseOver = true;

        manager.ReportMouseOver(fixedIndex);
    }

    public void OnPointerExit(PointerEventData eventData) {
        mouseOver = false;

        manager.ReportMouseLeave(fixedIndex);
    }
}
