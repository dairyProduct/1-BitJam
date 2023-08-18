using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private Vector3 originalPos;
    private void Start() {
        originalPos = transform.localPosition;
    }

    public IEnumerator Shake(float duration, float magnitude) {
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            //float x = Random.Range(-1, 1) * magnitude;
            //float y = Random.Range(-1, 1) * magnitude;
            Vector2 coord = new Vector2(Random.Range(-5, 5), Random.Range(-5, 5));
            float x = Mathf.PerlinNoise(coord.x,coord.y) * magnitude;
            float y = Mathf.PerlinNoise(coord.x,coord.y) * magnitude;

            transform.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = originalPos;
    }
}
