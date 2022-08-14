using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DragDropManager : MonoBehaviour
{
    public Statement statementBeingDragged;

    public bool draggingStatement = false;

    public RectTransform mouseStatement;
    TMP_Text mouseStatementText;
    const float canvasWidth = 2560f;
    const float canvasHeight = 1440f;
    const float characterWidth = 29f;

    public bool highlightText;

    Color defaultLineColor = new Color(0.8f, 0.8f, 0.8f, 1f);
    Color discardLineColor = new Color(0.8f, 0.8f, 0.8f, 0.6f);

    bool pickedUpThisFrame = false;

    void Awake() {
        mouseStatementText = mouseStatement.GetComponent<TMP_Text>();
    }

    public void PickUp(Statement statement, bool hideMultiChoice = false) {
        draggingStatement = true;
        statementBeingDragged = statement;

        mouseStatementText.text = StatementSlot.ColorString(StatementSlot.InsertSegments(statementBeingDragged, hideMultiChoice) + statementBeingDragged.GetCollapsedSuffix());

        mouseStatement.sizeDelta = new Vector2(StatementSlot.CleanString(mouseStatementText.text).Length * characterWidth, 60f);
        SetMouseStatementPosition();

        pickedUpThisFrame = true;
    }

    public void Drop(bool destroy) {
        if (pickedUpThisFrame) {
            return;
        }

        if (destroy) {
            DestroyStatement(statementBeingDragged);
        }

        draggingStatement = false;
        statementBeingDragged = null;
        mouseStatementText.text = "";
    }

    void DestroyStatement(Statement statement) {
        if (statement.codeBlock != null) {
            for (int i = statement.codeBlock.statements.Count - 1; i >= 0; i--) {
                DestroyStatement(statement.codeBlock.statements[i]);
            }
        }
        Debug.Log("Destroying statement: " + statement.statementText);
        Destroy(statement);
    }

    void Update() {
        if (draggingStatement) {
            SetMouseStatementPosition();
        }
    }

    void LateUpdate() {
        if (highlightText) {
            mouseStatementText.color = defaultLineColor;
            highlightText = false;
        }
        else {
            mouseStatementText.color = discardLineColor;
        }

        pickedUpThisFrame = false;
    }

    void SetMouseStatementPosition() {
        mouseStatement.anchoredPosition = new Vector2((Input.mousePosition.x / Screen.width) * canvasWidth, (Input.mousePosition.y / Screen.height) * canvasHeight);
    }
}
