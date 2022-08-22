using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SensitivityChanger : MonoBehaviour, IScrollable
{
    public TMP_Text text;
    ScrollableArea scrollableArea;
    static int sensitivity = 10;

    void Start() {
        scrollableArea = GetComponent<ScrollableArea>();
        scrollableArea.SetScrollLength(40);
        scrollableArea.SetOffset(40 - sensitivity);
    }

    // Update is called once per frame
    void Update()
    {
        text.text = "> mouse sensitivity: <color=#AAFFAA>" + 50 * sensitivity;
    }

    public void UpdateScrollOffset(int scrollDelta) {
        sensitivity -= scrollDelta;
        PlayerLook.sensitivity = 50f * sensitivity;
    }
}
