using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public IEnumerator Shake(float duration, float magnitude) {
        Vector3 originalPos = transform.localPosition;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            //float x = Random.Range(-1, 1) * magnitude;
            //float y = Random.Range(-1, 1) * magnitude;
            Vector2 coord = new Vector2(Random.Range(-1, 1), Random.Range(-1, 1));
            float x = Mathf.PerlinNoise(coord.x,coord.y) * magnitude;
            float y = Mathf.PerlinNoise(coord.x,coord.y) * magnitude;

            transform.localPosition = new Vector3(x, y, originalPos.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = originalPos;
    }
}
