using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISlotManager
{
    DragDropManager DragDropManager {get;}

    void ReportMouseOver(int slotIndex);
    void ReportMouseLeave(int slotIndex);
}
