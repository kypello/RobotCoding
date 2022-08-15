using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonDoor : ButtonDevice
{
    public Animation animation;
    public GameObject collider;

    public override void Activate() {
        animation.Play("DoorOpen");
        collider.gameObject.SetActive(false);
    }

    public override void Deactivate() {
        animation.Play("DoorClose");
        collider.gameObject.SetActive(true);
    }
}
