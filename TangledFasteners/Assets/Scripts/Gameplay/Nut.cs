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
                meshRenderer.material.color = GetColor(colorType);
            }
        }

        public static Color GetColor(BoltColorType type)
        {
            switch (type)
            {
                case BoltColorType.Red: return Color.red;
                case BoltColorType.Blue: return Color.blue;
                case BoltColorType.Green: return Color.green;
                case BoltColorType.Yellow: return Color.yellow;
                case BoltColorType.Purple: return new Color(0.5f, 0f, 0.5f);
                case BoltColorType.Orange: return new Color(1f, 0.5f, 0f);
                default: return Color.white;
            }
        }

        public IEnumerator MoveToPosition(Vector3 startPos, Vector3 topLiftPos, Vector3 targetLiftPos, Vector3 finalPos, System.Action onComplete = null)
        {
            float duration = 0.25f;
            float elapsed = 0f;

            // Step 1: Unscrew Lift (Move up higher + twist rotation)
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                transform.position = Vector3.Lerp(startPos, topLiftPos, t);
                transform.Rotate(Vector3.up, 720f * Time.deltaTime, Space.Self);
                yield return null;
            }

            // Step 2: Across to target bolt
            elapsed = 0f;
            duration = 0.2f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                transform.position = Vector3.Lerp(topLiftPos, targetLiftPos, t);
                yield return null;
            }

            // Step 3: Screw Down onto target peg (Move down + reverse twist rotation)
            elapsed = 0f;
            duration = 0.25f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                transform.position = Vector3.Lerp(targetLiftPos, finalPos, t);
                transform.Rotate(Vector3.up, -720f * Time.deltaTime, Space.Self);
                yield return null;
            }

            transform.position = finalPos;
            transform.rotation = Quaternion.identity;
            onComplete?.Invoke();
        }
    }
}
