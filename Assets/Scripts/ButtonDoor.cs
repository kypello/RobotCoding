using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonDoor : ButtonDevice
{
    public Animation animation;
    public GameObject collider;
    public AudioSource openSound;
    public AudioSource closeSound;
    public ParticleSystem particles;
    bool open = false;

    public override void Activate() {
        animation.Play("DoorOpen");
        collider.gameObject.SetActive(false);
        openSound.Play();
        open = true;
    }

    public override void Deactivate() {
        animation.Play("DoorClose");
        collider.gameObject.SetActive(true);
        closeSound.Play();
        open = false;
        StartCoroutine(DoParticles());
    }

    IEnumerator DoParticles() {
        float timePassed = 0f;
        while (timePassed < 0.25f) {
            yield return null;
            timePassed += Time.deltaTime;

            if (open) {
                yield break;
            }
        }
        particles.Play();
    }
}
