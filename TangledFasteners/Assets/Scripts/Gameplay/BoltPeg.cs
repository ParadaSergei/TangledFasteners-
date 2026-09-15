using System.Collections.Generic;
using UnityEngine;

namespace TangledFasteners
{
    public class BoltPeg : MonoBehaviour
    {
        public int capacity = 5; // Length of the bolt (number of nuts it can hold)
        public float nutHeight = 0.45f;
        public Transform baseTransform;
        public List<Nut> stackedNuts = new List<Nut>();

        public bool IsFull => stackedNuts.Count >= capacity;
        public bool IsEmpty => stackedNuts.Count == 0;

        public Nut TopNut => IsEmpty ? null : stackedNuts[stackedNuts.Count - 1];

        public Vector3 GetTopSlotPosition()
        {
            Vector3 basePos = baseTransform != null ? baseTransform.position : transform.position;
            return basePos + Vector3.up * (stackedNuts.Count * nutHeight + 0.25f);
        }

        public Vector3 GetLiftPosition()
        {
            Vector3 basePos = baseTransform != null ? baseTransform.position : transform.position;
            // Higher lift altitude above top of bolt thread to clearly clear the bolt
            return basePos + Vector3.up * ((capacity + 2.2f) * nutHeight + 1.2f);
        }

        public List<Nut> GetConsecutiveTopNutsOfSameColor()
        {
            List<Nut> group = new List<Nut>();
            if (IsEmpty) return group;

            BoltColorType targetColor = TopNut.colorType;
            for (int i = stackedNuts.Count - 1; i >= 0; i--)
            {
                if (stackedNuts[i].colorType == targetColor)
                {
                    group.Add(stackedNuts[i]);
                }
                else
                {
                    break;
                }
            }
            return group;
        }

        public void AddNut(Nut nut)
        {
            stackedNuts.Add(nut);
        }

        public void RemoveNut(Nut nut)
        {
            stackedNuts.Remove(nut);
        }

        public bool IsSingleColor()
        {
            if (IsEmpty) return true;
            BoltColorType firstColor = stackedNuts[0].colorType;
            foreach (var nut in stackedNuts)
            {
                if (nut.colorType != firstColor) return false;
            }
            return true;
        }
    }
}
