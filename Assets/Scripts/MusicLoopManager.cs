using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicLoopManager : MonoBehaviour
{
    public AudioSource[] loops = new AudioSource[3];

    void Update() {
        foreach (AudioSource loop in loops) {
            if (loop.isPlaying) {
                return;
            }
        }
        foreach (AudioSource loop in loops) {
            loop.Play();
        }
    }
}
