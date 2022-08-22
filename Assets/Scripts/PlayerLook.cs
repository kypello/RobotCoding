using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerLook : MonoBehaviour
{
    public CharacterController controller;
    public static float sensitivity = 500f;
    float sensitivityMessageTimer = -1f;
    float xRotation = 0f;
    public TMP_Text sensitivityUpdateText;

    public bool control = true;
    Vector3 lockOnPoint;

    bool lookingAtPoint = false;
    bool mouseMovedSinceUnlocking;

    //public Pause pause;
    //public PlayButton playButton;

    void Update()
    {
        //if (pause.paused || !playButton.playing) {
        //    Cursor.lockState = CursorLockMode.None;
        //}
        //else {
        //    Cursor.lockState = CursorLockMode.Locked;
        //}

        if (control) {
            Cursor.lockState = CursorLockMode.Locked;

            /*
            if (Input.GetKeyDown(KeyCode.UpArrow)) {
                sensitivity += 50f;
                sensitivityUpdateText.gameObject.SetActive(true);
                sensitivityUpdateText.text = "sensitivity: " + sensitivity;
                sensitivityMessageTimer = 2f;
            }
            if (Input.GetKeyDown(KeyCode.DownArrow) && sensitivity > 51f) {
                sensitivity -= 50f;
                sensitivityUpdateText.gameObject.SetActive(true);
                sensitivityUpdateText.text = "sensitivity: " + sensitivity;
                sensitivityMessageTimer = 2f;
            }
            */

            if (mouseMovedSinceUnlocking) {
                xRotation -= Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
                xRotation = Mathf.Clamp(xRotation, -90f, 90f);
                transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

                controller.transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime);
            }
            else {
                if (Input.GetAxis("Mouse X") != 0f || Input.GetAxis("Mouse Y") != 0f) {
                    mouseMovedSinceUnlocking = true;
                }
            }
        }
        else {
            mouseMovedSinceUnlocking = false;
        }

        if (sensitivityMessageTimer > 0f) {
            sensitivityMessageTimer -= Time.deltaTime;
            if (sensitivityMessageTimer <= 0f) {
                sensitivityUpdateText.gameObject.SetActive(false);
            }
        }
    }

    public IEnumerator LookAt(Vector3 point) {
        while (lookingAtPoint) {
            yield return null;
        }

        lookingAtPoint = true;

        Vector3 targetDir = (point - transform.position).normalized;

        float dotProduct;

        do {
            if (control) {
                break;
            }

            dotProduct = Vector3.Dot(transform.forward, targetDir);

            Vector3 delta = Vector3.RotateTowards(transform.forward, targetDir, (dotProduct * -dotProduct + 1f) * 2f * Mathf.PI * Time.deltaTime, 0f);

            transform.localRotation = Quaternion.LookRotation(delta);
            transform.localRotation = Quaternion.Euler(Vector3.right * transform.localEulerAngles.x);

            controller.transform.localRotation = Quaternion.LookRotation(delta);
            controller.transform.localRotation = Quaternion.Euler(Vector3.up * controller.transform.localEulerAngles.y);

            xRotation = Mathf.Repeat(transform.localEulerAngles.x + 90f, 180f) - 90f;

            yield return null;
        } while (dotProduct < 0.99f);

        lookingAtPoint = false;
    }
}
