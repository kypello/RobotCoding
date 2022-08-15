using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerInteract : MonoBehaviour
{
    public RectTransform crosshair;
    public GameObject codeEditor;
    public Player player;
    public PlayerLook playerLook;
    public StatementSlotManager slotManager;

    public TMP_Text closeButton;
    public TMP_Text runButton;
    MouseOver closeMouseOver;
    MouseOver runMouseOver;

    Robot robot;
    bool codeEditorOpen = false;

    void Awake() {
        closeMouseOver = closeButton.GetComponent<MouseOver>();
        runMouseOver = runButton.GetComponent<MouseOver>();
    }

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
            if (slotManager.codeChanged && robot.state == Robot.State.Running) {
                robot.StopRunningCode();
                runButton.text = "> <color=#AAFFAA>run";
                slotManager.SetCurrentRunningLine(-1);
            }

            if (robot.state == Robot.State.Running) {
                if (runMouseOver.mouseOver) {

                    if (!runMouseOver.mouseOverLastFrame) {
                        runButton.text = "<mark=#FFFFFF11>> <color=#FF8888>stop";
                    }
                    if (Input.GetMouseButtonDown(0)) {
                        robot.StopRunningCode();
                        runButton.text = "<mark=#FFFFFF11>> <color=#AAFFAA>run";
                        slotManager.SetCurrentRunningLine(-1);
                    }
                }
                else {
                    if (runMouseOver.mouseOverLastFrame) {
                        runButton.text = "> <color=#FF8888>stop";
                    }
                }
            }
            else {
                if (runMouseOver.mouseOver) {

                    if (!runMouseOver.mouseOverLastFrame) {
                        runButton.text = "<mark=#FFFFFF11>> <color=#AAFFAA>run";
                    }
                    if (Input.GetMouseButtonDown(0)) {
                        CloseCodeEditor();
                        StartCoroutine(robot.RunCode());
                    }
                }
                else {
                    if (runMouseOver.mouseOverLastFrame) {
                        runButton.text = "> <color=#AAFFAA>run";
                    }
                }
            }

            if (closeMouseOver.mouseOver) {
                if (!closeMouseOver.mouseOverLastFrame) {
                    closeButton.text = "<mark=#FFFFFF11>> close";
                }
                if (Input.GetMouseButtonDown(0)) {
                    CloseCodeEditor();
                }
            }
            else {
                if (closeMouseOver.mouseOverLastFrame) {
                    closeButton.text = "> close";
                }
            }
        }
    }

    void OpenCodeEditor(Robot r) {
        codeEditorOpen = true;
        robot = r;

        Robot.timeScale = 0f;
        codeEditor.SetActive(true);
        player.control = false;
        playerLook.control = false;
        Cursor.lockState = CursorLockMode.None;
        crosshair.gameObject.SetActive(false);
        slotManager.codeChanged = false;
        if (robot.state == Robot.State.Running) {
            slotManager.SetCurrentRunningLine(robot.GetCurrentLine());
        }
        else {
            slotManager.SetCurrentRunningLine(-1);
        }


        closeMouseOver.mouseOver = false;
        closeMouseOver.mouseOverLastFrame = false;
        closeButton.text = "> close";

        runMouseOver.mouseOver = false;
        runMouseOver.mouseOverLastFrame = false;
        if (robot.state == Robot.State.Running) {
            runButton.text = "> <color=#FF8888>stop";
        }
        else {
            runButton.text = "> <color=#AAFFAA>run";
        }

        slotManager.LoadRobotCode(robot.rootBlock);
    }

    void CloseCodeEditor() {
        codeEditorOpen = false;

        Robot.timeScale = 1f;
        codeEditor.SetActive(false);
        player.control = true;
        playerLook.control = true;
        crosshair.gameObject.SetActive(true);
    }
}
