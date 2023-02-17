using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    // Shake scene camera
    public IEnumerator Shake(float duration, float strength){
        Vector3 originalPosition = transform.localPosition; // Original position of the camera before shake
        float elapsed            = 0.0f;                    // Time elapsed since shake start

        // Process camera shake for given time duration
        while(elapsed < duration){
            float x = Random.Range(-1f, 1f) * strength; // Random x position for camera shake
            float y = Random.Range(-1f, 1f) * strength; // Random y position for camera shake
        
            transform.localPosition = new Vector3(x, y, originalPosition.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        // Restore original camera position
        transform.localPosition = originalPosition;
    }
}
