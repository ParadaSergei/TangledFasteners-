using System.Collections;
using UnityEngine;

namespace TangledFasteners
{
    public class Nut : MonoBehaviour
    {
        public BoltColorType colorType;
        public Renderer meshRenderer;

        private void Awake()
        {
            if (meshRenderer == null)
            {
                meshRenderer = GetComponentInChildren<Renderer>();
            }
        }

        public void ApplyColor()
        {
            if (meshRenderer == null) meshRenderer = GetComponentInChildren<Renderer>();
            if (meshRenderer != null)
            {
                meshRenderer.material.color = Bolt.GetColor(colorType);
            }
        }

        public IEnumerator MoveToPosition(Vector3 startPos, Vector3 topLiftPos, Vector3 targetLiftPos, Vector3 finalPos, System.Action onComplete = null)
        {
            float duration = 0.15f;
            float elapsed = 0f;

            // Step 1: Up
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                transform.position = Vector3.Lerp(startPos, topLiftPos, elapsed / duration);
                yield return null;
            }

            // Step 2: Across
            elapsed = 0f;
            duration = 0.2f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                transform.position = Vector3.Lerp(topLiftPos, targetLiftPos, elapsed / duration);
                yield return null;
            }

            // Step 3: Down
            elapsed = 0f;
            duration = 0.15f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                transform.position = Vector3.Lerp(targetLiftPos, finalPos, elapsed / duration);
                yield return null;
            }

            transform.position = finalPos;
            onComplete?.Invoke();
        }
    }
}
