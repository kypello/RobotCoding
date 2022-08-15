using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingElement : MonoBehaviour
{
    public static float timeScale = 1f;
    public float speed = 2f;
    public AudioSource hitWall;
    public AudioSource land;
    public ParticleSystem landParticles;

    public IEnumerator MoveOneSpace(float direction) {
        bool pathBlocked = Physics.Raycast(transform.position, transform.forward * direction, 2f, 1<<7);

        Vector3 startPosition = transform.position;
        Vector3 targetPosition = startPosition + transform.forward * direction * 2f;

        if (pathBlocked) {
            float progress = 0f;
            while (progress < 0.2f) {
                progress += Time.deltaTime * timeScale * speed;
                transform.position = Vector3.Lerp(startPosition, targetPosition, progress);
                yield return null;
            }
            hitWall.Play();
            //yield return new WaitForSeconds(0.25f);
            while (progress >= 0f) {
                progress -= Time.deltaTime * timeScale * speed;
                transform.position = Vector3.Lerp(startPosition, targetPosition, progress);
                yield return null;
            }
        }
        else {
            float progress = 0f;
            while (progress < 1f) {
                progress += Time.deltaTime * timeScale * speed;
                transform.position = Vector3.Lerp(startPosition, targetPosition, progress);
                yield return null;
            }

            bool wasInAir = false;
            while (!Physics.Raycast(transform.position, -transform.up, 2f, 1<<7)) {
                wasInAir = true;
                startPosition = transform.position;
                targetPosition = startPosition - transform.up * 2f;
                progress = 0f;
                while (progress < 1f) {
                    progress += Time.deltaTime * timeScale * speed * 2f;
                    transform.position = Vector3.Lerp(startPosition, targetPosition, progress);
                    yield return null;
                }
            }
            if (wasInAir) {
                land.Play();
                landParticles.Play();
            }
        }
    }
}
