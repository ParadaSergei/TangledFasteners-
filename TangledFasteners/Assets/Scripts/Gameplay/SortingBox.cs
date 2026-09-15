using System.Collections.Generic;
using UnityEngine;

namespace TangledFasteners
{
    public class SortingBox : MonoBehaviour
    {
        public BoltColorType targetColor;
        public int capacity = 3;
        public Transform[] slotPositions;
        public Renderer boxRenderer;

        private List<Bolt> storedBolts = new List<Bolt>();

        private void Start()
        {
            if (boxRenderer != null)
            {
                boxRenderer.material.color = Bolt.GetColor(targetColor);
            }
        }

        public bool IsFull => storedBolts.Count >= capacity;

        public bool CanAccept(Bolt bolt)
        {
            return !IsFull && bolt.colorType == targetColor;
        }

        public Transform GetNextSlotTransform()
        {
            int index = storedBolts.Count;
            if (slotPositions != null && index < slotPositions.Length)
            {
                return slotPositions[index];
            }
            return transform;
        }

        public void AddBolt(Bolt bolt)
        {
            storedBolts.Add(bolt);
            if (IsFull)
            {
                OnBoxFilled();
            }
        }

        private void OnBoxFilled()
        {
            Debug.Log($"Sorting Box for {targetColor} is full!");
            // Animate box leaving or destroy
            StartCoroutine(AnimateBoxComplete());
        }

        private System.Collections.IEnumerator AnimateBoxComplete()
        {
            yield return new WaitForSeconds(0.4f);
            Vector3 startPos = transform.position;
            Vector3 targetPos = startPos + Vector3.right * 10f;
            float elapsed = 0f;
            float duration = 0.5f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                transform.position = Vector3.Lerp(startPos, targetPos, elapsed / duration);
                yield return null;
            }

            foreach (var b in storedBolts)
            {
                if (b != null) Destroy(b.gameObject);
            }
            Destroy(gameObject);

            GameManager.Instance?.OnBoxCleared();
        }
    }
}
