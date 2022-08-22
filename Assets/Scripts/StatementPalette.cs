using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatementPalette : MonoBehaviour, ISlotManager, IScrollable
{
    public int slotCount;
    public List<StatementSlot> slots = new List<StatementSlot>();
    public StatementSlot slotPrefab;
    public DragDropManager dragDropManager;
    public DragDropManager DragDropManager => dragDropManager;
    ScrollableArea scrollableArea;

    public List<Statement> templateStatements = new List<Statement>();
    List<Statement> availableStatements = new List<Statement>();
    int highlightedSlotIndex = -1;

    void Awake() {
        foreach (Statement statement in templateStatements) {
            availableStatements.Add(Instantiate(statement));
        }

        for (int i = 0; i < slotCount; i++) {
            slots.Add(Instantiate(slotPrefab, transform));
            slots[i].SetUp(i, this, new Vector2(550f, 60f), 60f);
            slots[i].freeFloating = true;
            slots[i].hideSegments = true;
            slots[i].collapsed = true;
        }

        scrollableArea = GetComponent<ScrollableArea>();

        if (availableStatements.Count > slotCount) {
            scrollableArea.SetScrollLength(availableStatements.Count - slotCount);
        }
        else {
            scrollableArea.SetScrollLength(0);
        }
    }

    void Start() {
        ArrangeStatements(0);
    }

    void ArrangeStatements(int statementIndex) {
        for (int slotIndex = 0; slotIndex < slotCount; slotIndex++) {
            if (statementIndex < availableStatements.Count) {
                slots[slotIndex].SetStatement(availableStatements[statementIndex]);

                statementIndex++;
            }
            else {
                slots[slotIndex].SetStatement(null);
            }
        }
    }

    void Update() {
        if (highlightedSlotIndex != -1 && Input.GetMouseButtonDown(0)) {
            if (dragDropManager.draggingStatement) {
                dragDropManager.Drop(true);
            }
            dragDropManager.PickUp(Instantiate(slots[highlightedSlotIndex].statement), true);
            slots[highlightedSlotIndex].SetHighlighted(false);
        }
    }

    public void ReportMouseOver(int slotIndex) {
        if (highlightedSlotIndex != -1) {
            slots[highlightedSlotIndex].SetHighlighted(false);
        }
        if (slots[slotIndex].statement != null) {
            highlightedSlotIndex = slotIndex;
            slots[highlightedSlotIndex].SetHighlighted(true);
        }
        else {
            highlightedSlotIndex = -1;
        }
    }

    public void ReportMouseLeave(int slotIndex) {
        if (highlightedSlotIndex == slotIndex) {
            slots[highlightedSlotIndex].SetHighlighted(false);
            highlightedSlotIndex = -1;
        }
    }

    public void UpdateScrollOffset(int scrollDelta) {
        ArrangeStatements(scrollableArea.GetOffset());
    }
}
