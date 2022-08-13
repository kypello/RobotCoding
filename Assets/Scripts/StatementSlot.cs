using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class StatementSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    TMP_Text text;
    RectTransform rectTransform;

    const float lineHeight = 60f;
    const float characterWidth = 29f;
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
        }
    }

    public void SetHighlighted(bool h) {
        if (highlighted != h) {
            highlighted = h;
            updateNeeded = true;
        }
    }

    void LateUpdate() {
        if (updateNeeded) {
            Display();
            updateNeeded = false;
        }
    }

    void Display() {
        text.text = "";

        if (!freeFloating) {
            text.text += GetLineNumberString();

            int scope = codeBlock.scope;
            if (statement != null && statement.isScopeEnder) {
                scope--;
            }

            for (int i = 0; i < scope; i++) {
                text.text += "    ";
            }
        }

        if (highlighted) {
            text.text += "<mark=#FFFFFF11>";

            if (statement == null) {
                text.text += "<color=#FFFFFF00>" + manager.DragDropManager.statementBeingDragged.statementText;
            }
        }

        if (statement != null) {
            text.text += statement.statementText;
        }
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
