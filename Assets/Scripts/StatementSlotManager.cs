using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatementSlotManager : MonoBehaviour, ISlotManager, IScrollable
{
    public int slotCount = 30;
    public List<StatementSlot> slots = new List<StatementSlot>();
    public StatementSlot slotPrefab;
    public Transform canvas;

    public CodeBlock rootBlock;

    int mouseOverSlotIndex = -1;
    int highlightedSlotIndex = -1;

    public Statement[] testStatements;

    public DragDropManager dragDropManager;
    public DragDropManager DragDropManager => dragDropManager;

    public Statement scopeEnderStatement;
    public bool codeChanged = false;

    ScrollableArea scrollableArea;

    int currentRunningLine;

    public DropdownMenu dropdownMenu;
    const float canvasWidth = 2560f;

    void Awake() {
        for (int i = 0; i < slotCount; i++) {
            slots.Add(Instantiate(slotPrefab, transform));
            slots[i].SetUp(i, this, new Vector2(1600f, 60f));
        }

        //SetupTestStatements();

        scrollableArea = GetComponent<ScrollableArea>();
        //ArrangeCodeBlock(rootBlock, -scrollableArea.GetOffset(), -1, true);
    }

    public void LoadRobotCode(CodeBlock robotCodeBlock) {
        rootBlock = robotCodeBlock;
        ArrangeCodeBlock(rootBlock, -scrollableArea.GetOffset(), -1, true);
    }

    void Update() {
        if (highlightedSlotIndex != -1) {
            StatementSlot highlightedSlot = slots[highlightedSlotIndex];
            highlightedSlot.SetSegmentHighlight();


            if (Input.GetMouseButtonDown(1)) {
                if (!dragDropManager.draggingStatement) {
                    codeChanged = true;

                    if (highlightedSlot.statement.displayOnParentScope) {
                        highlightedSlot.codeBlock.hostStatement.parentCodeBlock.RemoveStatement(highlightedSlot.codeBlock.hostStatement);
                        dragDropManager.DestroyStatement(highlightedSlot.codeBlock.hostStatement);
                    }
                    else {
                        highlightedSlot.codeBlock.RemoveStatement(highlightedSlot.statement);
                        dragDropManager.DestroyStatement(highlightedSlot.statement);
                    }

                    ArrangeCodeBlock(rootBlock, -scrollableArea.GetOffset(), -1, true);
                    RecalculateHighlightedSlot();
                }
            }
            else if (Input.GetMouseButtonDown(0)) {
                if (dragDropManager.draggingStatement) {
                    codeChanged = true;

                    highlightedSlot.codeBlock.InsertStatement(dragDropManager.statementBeingDragged, highlightedSlot.localIndex);
                    dragDropManager.Drop(false);

                    int lineCount = rootBlock.GetLineCount();
                    if (lineCount >= slotCount) {
                        scrollableArea.SetScrollLength(lineCount - (slotCount - 1));
                    }
                    else {
                        scrollableArea.SetScrollLength(0);
                    }

                    ArrangeCodeBlock(rootBlock, -scrollableArea.GetOffset(), -1, true);
                    RecalculateHighlightedSlot();
                }
                else {
                    codeChanged = true;

                    if (highlightedSlot.statement.displayOnParentScope) {
                        dragDropManager.PickUp(highlightedSlot.codeBlock.hostStatement);
                        highlightedSlot.codeBlock.hostStatement.parentCodeBlock.RemoveStatement(dragDropManager.statementBeingDragged);

                        ArrangeCodeBlock(rootBlock, -scrollableArea.GetOffset(), highlightedSlotIndex, true);
                        RecalculateHighlightedSlot();
                    }
                    else if (highlightedSlot.statement.highlightableSegments.Length > 0 && highlightedSlot.highlightedSegment != -1) {
                        highlightedSlot.statement.highlightableSegments[highlightedSlot.highlightedSegment].Click(highlightedSlot);
                    }
                    else {
                        dragDropManager.PickUp(highlightedSlot.statement);
                        highlightedSlot.codeBlock.RemoveStatement(highlightedSlot.statement);

                        ArrangeCodeBlock(rootBlock, -scrollableArea.GetOffset(), highlightedSlotIndex, true);
                        RecalculateHighlightedSlot();
                    }
                }
            }
        }
        else {
            if (Input.GetMouseButtonDown(0) && dragDropManager.draggingStatement) {
                dragDropManager.Drop(true);
            }
        }

        /*
        if (highlightedSlotIndex != -1) {
            slots[highlightedSlotIndex].SetSegmentHighlight();
        }
        if (!dragDropManager.draggingStatement && highlightedSlotIndex != -1 && Input.GetMouseButtonDown(0)) {
            codeChanged = true;
            StatementSlot highlightedSlot = slots[highlightedSlotIndex];

            if (highlightedSlot.statement.displayOnParentScope) {
                dragDropManager.PickUp(highlightedSlot.codeBlock.hostStatement);
                highlightedSlot.codeBlock.hostStatement.parentCodeBlock.RemoveStatement(dragDropManager.statementBeingDragged);

                ArrangeCodeBlock(rootBlock, -scrollableArea.GetOffset(), highlightedSlotIndex, true);
                RecalculateHighlightedSlot();
            }
            else if (highlightedSlot.statement.highlightableSegments.Length > 0 && highlightedSlot.highlightedSegment != -1) {
                highlightedSlot.statement.highlightableSegments[highlightedSlot.highlightedSegment].Click(highlightedSlot);
            }
            else {
                dragDropManager.PickUp(highlightedSlot.statement);
                highlightedSlot.codeBlock.RemoveStatement(dragDropManager.statementBeingDragged);

                ArrangeCodeBlock(rootBlock, -scrollableArea.GetOffset(), highlightedSlotIndex, true);
                RecalculateHighlightedSlot();
            }

        }
        else if (dragDropManager.draggingStatement) {
            if (highlightedSlotIndex != -1) {
                dragDropManager.highlightText = true;
            }

            if (Input.GetMouseButtonDown(0)) {
                if (highlightedSlotIndex != -1) {
                    codeChanged = true;
                    StatementSlot highlightedSlot = slots[highlightedSlotIndex];

                    highlightedSlot.codeBlock.InsertStatement(dragDropManager.statementBeingDragged, highlightedSlot.localIndex);
                    dragDropManager.Drop(false);
                }
                else {
                    dragDropManager.Drop(true);
                }

                int lineCount = rootBlock.GetLineCount();
                if (lineCount >= slotCount) {
                    scrollableArea.SetScrollLength(lineCount - (slotCount - 1));
                }
                else {
                    scrollableArea.SetScrollLength(0);
                }

                ArrangeCodeBlock(rootBlock, -scrollableArea.GetOffset(), -1, true);
                RecalculateHighlightedSlot();
            }
        }
        */
    }

    public void SetCurrentRunningLine(int line) {
        int locallyIndexedLine = currentRunningLine - scrollableArea.GetOffset();
        if (locallyIndexedLine >= 0 && locallyIndexedLine < slotCount) {
            slots[locallyIndexedLine].SetFullLineHighlight(false);
        }
        currentRunningLine = line;
        locallyIndexedLine = currentRunningLine - scrollableArea.GetOffset();
        if (locallyIndexedLine >= 0 && locallyIndexedLine < slotCount) {
            slots[locallyIndexedLine].SetFullLineHighlight(true);
        }
    }

    void SetupTestStatements() {
        rootBlock = new CodeBlock();
        rootBlock.SetScope(0);

        for (int i = 0; i < 6; i++) {
            rootBlock.InsertStatement(Instantiate(testStatements[i]), i);
            if (rootBlock.statements[i].statementText == "while (true) {") {
                rootBlock.statements[i].codeBlock.InsertStatement(Instantiate(testStatements[0]), 0);
                rootBlock.statements[i].codeBlock.InsertStatement(Instantiate(testStatements[1]), 1);
                rootBlock.statements[i].codeBlock.InsertStatement(Instantiate(testStatements[2]), 2);
            }
        }

        rootBlock.SetScope(0);
    }


    public void UpdateScrollOffset(int scrollDelta) {
        if (dragDropManager.draggingStatement && highlightedSlotIndex != -1) {
            if (mouseOverSlotIndex == highlightedSlotIndex || scrollDelta == 1) {
                ArrangeCodeBlock(rootBlock, -scrollableArea.GetOffset(), highlightedSlotIndex, true);

                while (highlightedSlotIndex > 0 && slots[highlightedSlotIndex].statement == null && slots[highlightedSlotIndex-1].statement == null) {
                    slots[highlightedSlotIndex].SetHighlighted(false);
                    highlightedSlotIndex--;
                }

                slots[highlightedSlotIndex].SetHighlighted(true);
            }
            else {
                ArrangeCodeBlock(rootBlock, -scrollableArea.GetOffset(), -1, true);
                slots[highlightedSlotIndex].SetHighlighted(false);
                highlightedSlotIndex = mouseOverSlotIndex;

                while (highlightedSlotIndex > 0 && slots[highlightedSlotIndex].statement == null && slots[highlightedSlotIndex-1].statement == null) {
                    highlightedSlotIndex--;
                }

                slots[highlightedSlotIndex].SetHighlighted(true);
            }
        }
        else {
            ArrangeCodeBlock(rootBlock, -scrollableArea.GetOffset(), -1, true);
            RecalculateHighlightedSlot();
            /*
            if (highlightedSlotIndex != -1 && slots[highlightedSlotIndex].statement == null) {
                slots[highlightedSlotIndex].SetHighlighted(false);
                highlightedSlotIndex = -1;
            }
            else if (highlightedSlotIndex == -1 && mouseOverSlotIndex != -1 && slots[mouseOverSlotIndex].statement != null) {
                highlightedSlotIndex = mouseOverSlotIndex;
                slots[highlightedSlotIndex].SetHighlighted(true);
            }
            */
        }
    }

    void ArrangeCodeBlock(CodeBlock codeBlock, int startIndex, int gap, bool fillRemainder, int scope = 0) {
        int slotIndex = startIndex;
        int statementIndex = 0;
        while (slotIndex < slotCount && statementIndex < codeBlock.statements.Count) {
            if (slotIndex >= 0) {
                slots[slotIndex].localIndex = statementIndex;
                slots[slotIndex].globalIndex = slotIndex;
                slots[slotIndex].codeBlock = codeBlock;
                slots[slotIndex].SetLineNumber(slotIndex + scrollableArea.GetOffset());
                if (slotIndex + scrollableArea.GetOffset() == currentRunningLine) {
                    slots[slotIndex].SetFullLineHighlight(true);
                }
                else {
                    slots[slotIndex].SetFullLineHighlight(false);
                }
            }

            if (slotIndex >= 0 && slotIndex == gap) {
                slots[slotIndex].SetStatement(null);
            }
            else {
                if (slotIndex >= 0) {
                    slots[slotIndex].SetStatement(codeBlock.statements[statementIndex]);
                }

                if (codeBlock.statements[statementIndex].codeBlock != null) {
                    ArrangeCodeBlock(codeBlock.statements[statementIndex].codeBlock, slotIndex+1, gap, false, scope+1);
                    slotIndex += codeBlock.statements[statementIndex].codeBlock.GetLineCount();

                    if (dragDropManager.draggingStatement && highlightedSlotIndex != -1 && codeBlock.statements[statementIndex].codeBlock.ContainsCodeBlock(slots[highlightedSlotIndex].codeBlock) && !(highlightedSlotIndex > 0 && slots[highlightedSlotIndex-1].statement.isScopeEnder && slots[highlightedSlotIndex-1].codeBlock == slots[highlightedSlotIndex].codeBlock)) {
                        slotIndex++;
                    }
                }

                statementIndex++;
            }

            slotIndex++;
        }

        if (fillRemainder) {
            while (slotIndex < slotCount) {
                if (slotIndex >= 0) {
                    slots[slotIndex].localIndex = statementIndex;
                    slots[slotIndex].globalIndex = slotIndex;
                    slots[slotIndex].codeBlock = codeBlock;
                    slots[slotIndex].SetLineNumber(slotIndex + scrollableArea.GetOffset());
                    slots[slotIndex].SetStatement(null);
                }

                slotIndex++;
            }
        }
    }

    public void ReportMouseOver(int slotIndex) {
        mouseOverSlotIndex = slotIndex;

        RecalculateHighlightedSlot();
    }

    public void ReportMouseLeave(int slotIndex) {
        if (mouseOverSlotIndex == slotIndex) {
            mouseOverSlotIndex = -1;

            RecalculateHighlightedSlot();
        }
    }

    void RecalculateHighlightedSlot() {
        if (mouseOverSlotIndex == -1) {
            if (highlightedSlotIndex != -1) {
                slots[highlightedSlotIndex].SetHighlighted(false);
                highlightedSlotIndex = -1;

                if (dragDropManager.draggingStatement) {
                    ArrangeCodeBlock(rootBlock, -scrollableArea.GetOffset(), -1, true);
                }
            }
        }
        else {
            if (dragDropManager.draggingStatement) {
                int i = mouseOverSlotIndex;
                while (i > 0 && slots[i].statement == null && slots[i-1].statement == null) {
                    i--;
                }

                if (highlightedSlotIndex != i) {
                    if (highlightedSlotIndex != -1) {
                        slots[highlightedSlotIndex].SetHighlighted(false);
                    }
                    highlightedSlotIndex = i;
                    slots[highlightedSlotIndex].SetHighlighted(true);

                    if (slots[highlightedSlotIndex].statement != null) {
                        ArrangeCodeBlock(rootBlock, -scrollableArea.GetOffset(), highlightedSlotIndex, true);
                    }
                }
            }
            else {
                if (slots[mouseOverSlotIndex].statement != null) {
                    if (highlightedSlotIndex != -1) {
                        slots[highlightedSlotIndex].SetHighlighted(false);
                    }
                    highlightedSlotIndex = mouseOverSlotIndex;
                    slots[highlightedSlotIndex].SetHighlighted(true);
                }
                else {
                    if (highlightedSlotIndex != -1) {
                        slots[highlightedSlotIndex].SetHighlighted(false);
                        highlightedSlotIndex = -1;
                    }
                }
            }
        }
    }
}
