using System.Collections;
using UnityEngine;

namespace TangledFasteners
{
    public class Bolt : MonoBehaviour
    {
        public BoltColorType colorType;
        public Hole currentHole;
        public Renderer meshRenderer;

        public bool isSelected;
        public bool isMoving;

        private void Start()
        {
            if (meshRenderer == null)
            {
                meshRenderer = GetComponentInChildren<Renderer>();
            }
            ApplyColorMaterial();
        }

        public void ApplyColorMaterial()
        {
            if (meshRenderer != null)
            {
                Material mat = meshRenderer.material;
                mat.color = GetColor(colorType);
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

        public IEnumerator UnscrewAndMoveTo(Vector3 targetPosition, System.Action onComplete)
        {
            isMoving = true;
            Vector3 startPos = transform.position;
            Vector3 liftPos = startPos + Vector3.back * 0.8f; // Unscrew outwards (towards camera in 2.5D/3D)

            // Step 1: Unscrew lift and rotate
            float duration = 0.25f;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                transform.position = Vector3.Lerp(startPos, liftPos, t);
                transform.Rotate(Vector3.forward, 360f * Time.deltaTime * 3f, Space.Self);
                yield return null;
            }

            // Step 2: Move to target lift position
            Vector3 targetLiftPos = targetPosition + Vector3.back * 0.8f;
            elapsed = 0f;
            duration = 0.35f;
            Vector3 midStart = transform.position;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                transform.position = Vector3.Lerp(midStart, targetLiftPos, t);
                yield return null;
            }

            // Step 3: Screw into target hole/slot
            elapsed = 0f;
            duration = 0.2f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                transform.position = Vector3.Lerp(targetLiftPos, targetPosition, t);
                transform.Rotate(Vector3.forward, -360f * Time.deltaTime * 3f, Space.Self);
                yield return null;
            }

            transform.position = targetPosition;
            isMoving = false;
            onComplete?.Invoke();
        }
    }
}
