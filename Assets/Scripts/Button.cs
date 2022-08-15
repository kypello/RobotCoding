using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    public GameObject buttonUnpressed;
    public List<ButtonDevice> buttonDevices = new List<ButtonDevice>();
    List<Collider> triggerOccupants = new List<Collider>();

    void OnTriggerEnter(Collider col) {
        if (triggerOccupants.Count == 0) {
            Press();
        }
        triggerOccupants.Add(col);
    }

    void OnTriggerExit(Collider col) {
        triggerOccupants.Remove(col);
        if (triggerOccupants.Count == 0) {
            Unpress();
        }
    }

    void Press() {
        buttonUnpressed.SetActive(false);
        foreach (ButtonDevice buttonDevice in buttonDevices) {
            buttonDevice.Activate();
        }
    }

    void Unpress() {
        buttonUnpressed.SetActive(true);
        foreach (ButtonDevice buttonDevice in buttonDevices) {
            buttonDevice.Deactivate();
        }
    }
}
