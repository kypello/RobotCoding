using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    public AudioSource melody;

    Robot robot;
    bool codeEditorOpen = false;

    public GameObject pauseMenu;
    bool pauseMenuOpen = false;

    public TMP_Text pixelButtonText;
    public MouseOver pixelButtonMouseOver;

    public TMP_Text resumeButtonText;
    public MouseOver resumeButtonMouseOver;

    public TMP_Text quitButtonText;
    public MouseOver quitButtonMouseOver;

    static bool globalPixelsEnabled = true;
    bool pixelsEnabled = true;
    public GameObject cameraRenderImage;
    public Camera uiCam;
    public Camera playerCam;
    public RenderTexture cameraRenderTexture;

    void Awake() {
        closeMouseOver = closeButton.GetComponent<MouseOver>();
        runMouseOver = runButton.GetComponent<MouseOver>();
        if (!globalPixelsEnabled) {
            TogglePixelEffect();
        }
    }

    void Update() {
        if (playerLook.control) {
            if (Input.GetKeyDown(KeyCode.R)) {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                return;
            }

            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, 3f, 1<<6)) {
                crosshair.sizeDelta = Vector2.one * 24f;
                if (Input.GetMouseButtonDown(0)) {
                    OpenCodeEditor(hit.collider.GetComponent<Robot>());
                    return;
                }
            }
            else {
                crosshair.sizeDelta = Vector2.one * 12f;
            }

            if (Input.GetKeyDown(KeyCode.P)) {
                OpenPauseMenu();
            }
        }
        else if (codeEditorOpen) {
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
        else if (pauseMenuOpen) {
            if (Input.GetKeyDown(KeyCode.P)) {
                ClosePauseMenu();
                return;
            }

            if (pixelButtonMouseOver.mouseOver) {
                bool updatedPixelButton = false;

                if (Input.GetMouseButtonDown(0)) {
                    globalPixelsEnabled = !globalPixelsEnabled;
                    TogglePixelEffect();
                    updatedPixelButton = true;
                }

                if (!pixelButtonMouseOver.mouseOverLastFrame || updatedPixelButton) {
                    if (pixelsEnabled) {
                        pixelButtonText.text = "<mark=#FFFFFF11>> low-res effect: <color=#AAFFAA>enabled";
                    }
                    else {
                        pixelButtonText.text = "<mark=#FFFFFF11>> low-res effect: <color=#FF8888>disabled";
                    }

                }
            }
            else {
                if (pixelButtonMouseOver.mouseOverLastFrame) {
                    if (pixelsEnabled) {
                        pixelButtonText.text = "> low-res effect: <color=#AAFFAA>enabled";
                    }
                    else {
                        pixelButtonText.text = "> low-res effect: <color=#FF8888>disabled";
                    }
                }
            }

            if (resumeButtonMouseOver.mouseOver) {
                if (!resumeButtonMouseOver.mouseOverLastFrame) {
                    resumeButtonText.text = "<mark=#FFFFFF11>> <color=#AAFFAA>resume";
                }

                if (Input.GetMouseButtonDown(0)) {
                    ClosePauseMenu();
                    return;
                }
            }
            else {
                if (resumeButtonMouseOver.mouseOverLastFrame) {
                    resumeButtonText.text = "> <color=#AAFFAA>resume";
                }
            }

            if (quitButtonMouseOver.mouseOver) {
                if (!quitButtonMouseOver.mouseOverLastFrame) {
                    quitButtonText.text = "<mark=#FFFFFF11>> <color=#FF8888>quit";
                }

                if (Input.GetMouseButtonDown(0)) {
                    Application.Quit();
                    return;
                }
            }
            else {
                if (quitButtonMouseOver.mouseOverLastFrame) {
                    quitButtonText.text = "> <color=#FF8888>quit";
                }
            }
        }
    }

    void TogglePixelEffect() {
        pixelsEnabled = !pixelsEnabled;
        cameraRenderImage.SetActive(pixelsEnabled);
        uiCam.enabled = pixelsEnabled;
        if (pixelsEnabled) {
            playerCam.targetTexture = cameraRenderTexture;
        }
        else {
            playerCam.targetTexture = null;
        }
    }

    void OpenCodeEditor(Robot r) {
        codeEditorOpen = true;
        robot = r;

        melody.volume = 0f;
        MovingElement.timeScale = 0f;
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

        slotManager.LoadRobotCode(Robot.rootBlock);
    }

    void CloseCodeEditor() {
        codeEditorOpen = false;

        melody.volume = 0.6f;
        MovingElement.timeScale = 1f;
        codeEditor.SetActive(false);
        player.control = true;
        playerLook.control = true;
        Cursor.lockState = CursorLockMode.Locked;
        crosshair.gameObject.SetActive(true);
    }

    void OpenPauseMenu() {
        pauseMenuOpen = true;
        melody.volume = 0f;
        MovingElement.timeScale = 0f;
        pauseMenu.SetActive(true);
        player.control = false;
        playerLook.control = false;
        Cursor.lockState = CursorLockMode.None;
        crosshair.gameObject.SetActive(false);

        pixelButtonMouseOver.mouseOver = false;
        pixelButtonMouseOver.mouseOverLastFrame = false;
        if (pixelsEnabled) {
            pixelButtonText.text = "> low-res effect: <color=#AAFFAA>enabled";
        }
        else {
            pixelButtonText.text = "> low-res effect: <color=#FF8888>disabled";
        }

        quitButtonMouseOver.mouseOver = false;
        quitButtonMouseOver.mouseOverLastFrame = false;
        quitButtonText.text = "> <color=#FF8888>quit";

        resumeButtonMouseOver.mouseOver = false;
        resumeButtonMouseOver.mouseOverLastFrame = false;
        resumeButtonText.text = "> <color=#AAFFAA>resume";
    }

    void ClosePauseMenu() {
        pauseMenuOpen = false;
        melody.volume = 0.6f;
        MovingElement.timeScale = 1f;
        pauseMenu.SetActive(false);
        player.control = true;
        playerLook.control = true;
        Cursor.lockState = CursorLockMode.Locked;

        pixelButtonMouseOver.mouseOver = false;
        pixelButtonMouseOver.mouseOverLastFrame = false;

        crosshair.gameObject.SetActive(true);
    }
}
