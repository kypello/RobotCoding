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

    public int globalIndex;
    public int localIndex;
    public CodeBlock codeBlock;
    public bool freeFloating = false;

    bool updateNeeded = true;
    public int highlightedSegment = -1;

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
        if (lineNumber < 9) {
            return "<color=#FFFFFF22> " + (lineNumber + 1) + "</color>    ";
        }
        else {
            return "<color=#FFFFFF22>" + (lineNumber + 1) + "</color>    ";
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

            if (statement != null && statement.multiChoiceSegments.Length > 0) {
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

    public void ForceUpdate() {
        updateNeeded = true;
    }

    void LateUpdate() {
        if (updateNeeded) {
            Display();
            updateNeeded = false;
        }
    }

    void Display() {
        Debug.Log("Display");

        string fullText = "";

        if (!freeFloating) {
            fullText += GetLineNumberString();

            int scope = codeBlock.scope;
            if (statement != null && statement.isScopeEnder) {
                scope--;
            }

            for (int i = 0; i < scope; i++) {
                fullText += "    ";
            }
        }

        if (highlighted && highlightedSegment == -1) {
            fullText += "<mark=#FFFFFF11>";

            if (statement == null) {
                fullText += "<color=#FFFFFF00>" + manager.DragDropManager.statementBeingDragged.statementText;
            }
        }

        if (statement != null) {
            fullText += statement.statementText;

            if (statement.multiChoiceSegments.Length > 0) {
                foreach (MultiChoiceSegment multiChoiceSegment in statement.multiChoiceSegments) {
                    fullText = fullText.Replace(multiChoiceSegment.correspondingSegmentInStatement, multiChoiceSegment.GetChoice());
                }
                if (highlighted && highlightedSegment != -1) {
                    fullText = fullText.Replace(statement.multiChoiceSegments[highlightedSegment].GetChoice(), "<mark=#FFFFFF11>" + statement.multiChoiceSegments[highlightedSegment].GetChoice() + "</mark>");
                }
            }
        }

        text.text = fullText;
    }

    public void CalculateSegmentBounds() {
        string statementText = statement.statementText;
        foreach (MultiChoiceSegment multiChoiceSegment in statement.multiChoiceSegments) {
            statementText = statementText.Replace(multiChoiceSegment.correspondingSegmentInStatement, multiChoiceSegment.GetChoice());
        }

        foreach (MultiChoiceSegment multiChoiceSegment in statement.multiChoiceSegments) {
            int segmentStartCharacter = CleanString(statementText).IndexOf(CleanString(multiChoiceSegment.GetChoice()));
            int segmentEndCharacter = segmentStartCharacter + CleanString(multiChoiceSegment.GetChoice()).Length;

            multiChoiceSegment.startBound = (codeLeftmostPoint + (6 + codeBlock.scope * 4 + segmentStartCharacter) * characterWidth) / canvasWidth;
            multiChoiceSegment.endBound = (codeLeftmostPoint + (6 + codeBlock.scope * 4 + segmentEndCharacter) * characterWidth) / canvasWidth;
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

        for (int i = 0; i < statement.multiChoiceSegments.Length; i++) {
            float mousePos = Input.mousePosition.x / Screen.width;

            if (mousePos >= statement.multiChoiceSegments[i].startBound && mousePos < statement.multiChoiceSegments[i].endBound) {
                return i;
            }
        }

        return -1;
    }

    public static string CleanString(string s) {
        while (s.Contains("<")) {
            int a = s.IndexOf("<");
            int b = s.IndexOf(">");
            s = s.Remove(a, (b-a)+1);
        }
        return s;
    }

    /*
    public void Highlight(int highlightLength) {
        string highlightText = GetLineNumberString() + "<mark=#FFFFFF11><color=#FFFFFF00>";
        for (int i = 0; i < highlightLength; i++) {
            highlightText += "a";
        }
        text.text = highlightText;
    }
    */

    public void OnPointerEnter(PointerEventData eventData) {
        mouseOver = true;

        manager.ReportMouseOver(fixedIndex);
    }

    public void OnPointerExit(PointerEventData eventData) {
        mouseOver = false;

        manager.ReportMouseLeave(fixedIndex);
    }
}
