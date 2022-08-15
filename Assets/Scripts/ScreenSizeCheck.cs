using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenSizeCheck : MonoBehaviour
{
    float widthLastFrame;
    float heightLastFrame;

    PixelRender pixelRender;
    Camera camera;

    void Awake() {
        pixelRender = GetComponent<PixelRender>();
        camera = GetComponent<Camera>();

        RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 16);
        pixelRender.renderTexture = renderTexture;
        camera.targetTexture = renderTexture;

        widthLastFrame = Screen.width;
        heightLastFrame = Screen.height;
    }

    void Update() {
        if (widthLastFrame != Screen.width || heightLastFrame != Screen.height) {
            RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 16);
            pixelRender.renderTexture = renderTexture;
            camera.targetTexture = renderTexture;
        }

        widthLastFrame = Screen.width;
        heightLastFrame = Screen.height;
    }
}
