using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public RectTransform crosshair;
    public GameObject codeEditor;
    public Player player;
    public PlayerLook playerLook;
    public StatementSlotManager slotManager;

    Robot robot;
    bool codeEditorOpen = false;

    void Update() {
        if (!codeEditorOpen) {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, 3f, 1<<6)) {
                crosshair.sizeDelta = Vector2.one * 24f;
                if (Input.GetMouseButtonDown(0)) {
                    OpenCodeEditor(hit.collider.GetComponent<Robot>());
                }
            }
            else {
                crosshair.sizeDelta = Vector2.one * 12f;
            }
        }
        else {
            if (Input.GetKeyDown(KeyCode.R)) {
                CloseCodeEditor();
            }
        }
    }

    void OpenCodeEditor(Robot r) {
        codeEditorOpen = true;
        robot = r;

        codeEditor.SetActive(true);
        player.control = false;
        playerLook.control = false;
        Cursor.lockState = CursorLockMode.None;
        crosshair.gameObject.SetActive(false);

        slotManager.LoadRobotCode(robot.codeBlock);
    }

    void CloseCodeEditor() {
        codeEditorOpen = false;
        
        codeEditor.SetActive(false);
        player.control = true;
        playerLook.control = true;
        crosshair.gameObject.SetActive(true);
    }
}
