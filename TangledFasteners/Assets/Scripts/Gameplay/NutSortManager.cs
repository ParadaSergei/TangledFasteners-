using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TangledFasteners
{
    public class NutSortManager : MonoBehaviour
    {
        public static NutSortManager Instance { get; private set; }

        public List<BoltPeg> bolts = new List<BoltPeg>();
        private BoltPeg selectedBolt;
        private bool isBusy;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void OnBoltClicked(BoltPeg clickedBolt)
        {
            if (isBusy) return;

            if (selectedBolt == null)
            {
                // Select source bolt if not empty
                if (clickedBolt != null && !clickedBolt.IsEmpty)
                {
                    selectedBolt = clickedBolt;
                    HighlightBolt(selectedBolt, true);
                    AudioManager.Instance?.PlaySound(SoundType.Pick);
                }
            }
            else
            {
                // Second tap: deselect or transfer
                if (selectedBolt == clickedBolt)
                {
                    HighlightBolt(selectedBolt, false);
                    selectedBolt = null;
                }
                else
                {
                    TryTransferNuts(selectedBolt, clickedBolt);
                }
            }
        }

        private void HighlightBolt(BoltPeg bolt, bool highlight)
        {
            if (bolt == null || bolt.TopNut == null) return;
            Vector3 basePos = bolt.baseTransform != null ? bolt.baseTransform.position : bolt.transform.position;
            int nutIndex = bolt.stackedNuts.Count - 1;
            Vector3 normalPos = basePos + Vector3.up * (nutIndex * bolt.nutHeight + 0.3f);
            Vector3 targetPos = normalPos + (highlight ? Vector3.up * 0.4f : Vector3.zero);
            bolt.TopNut.transform.position = targetPos;
        }

        private void TryTransferNuts(BoltPeg source, BoltPeg target)
        {
            List<Nut> movingNuts = source.GetConsecutiveTopNutsOfSameColor();
            if (movingNuts.Count == 0)
            {
                Deselect(source);
                return;
            }

            BoltColorType movingColor = movingNuts[0].colorType;

            // Target validity check
            if (target.IsFull)
            {
                Deselect(source);
                return;
            }

            if (!target.IsEmpty && target.TopNut.colorType != movingColor)
            {
                Deselect(source);
                return;
            }

            // How many nuts can target accept?
            int targetAvailableSpace = target.capacity - target.stackedNuts.Count;
            int countToMove = Mathf.Min(movingNuts.Count, targetAvailableSpace);

            if (countToMove <= 0)
            {
                Deselect(source);
                return;
            }

            List<Nut> nutsToTransfer = new List<Nut>();
            for (int i = 0; i < countToMove; i++)
            {
                nutsToTransfer.Add(movingNuts[i]);
            }

            StartCoroutine(AnimateTransferGroup(source, target, nutsToTransfer));
        }

        private void Deselect(BoltPeg source)
        {
            HighlightBolt(source, false);
            selectedBolt = null;
        }

        private IEnumerator AnimateTransferGroup(BoltPeg source, BoltPeg target, List<Nut> nuts)
        {
            isBusy = true;
            HighlightBolt(source, false);
            selectedBolt = null;

            Vector3 sourceLift = source.GetLiftPosition();
            Vector3 targetLift = target.GetLiftPosition();

            AudioManager.Instance?.PlaySound(SoundType.Place);

            foreach (var nut in nuts)
            {
                source.RemoveNut(nut);
                Vector3 finalPos = target.GetTopSlotPosition();
                target.AddNut(nut);

                yield return nut.MoveToPosition(nut.transform.position, sourceLift, targetLift, finalPos);
            }

            isBusy = false;
            CheckWinCondition();
        }

        public void CheckWinCondition()
        {
            foreach (var bolt in bolts)
            {
                if (!bolt.IsEmpty && !bolt.IsSingleColorAndFull())
                {
                    return; // Level not solved yet
                }
            }

            Debug.Log("LEVEL SOLVED!");
            AudioManager.Instance?.PlaySound(SoundType.Win);
            UIManager.Instance?.ShowLevelCompletePanel();
        }
    }
}
