using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TangledFasteners
{
    public class NutSortManager : MonoBehaviour
    {
        public static NutSortManager Instance { get; private set; }

        public List<BoltPeg> bolts = new List<BoltPeg>();
        public int moveCount { get; private set; }
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

        public void ResetState()
        {
            isBusy = false;
            selectedBolt = null;
            moveCount = 0;
            bolts.Clear();
            UIManager.Instance?.UpdateMoveCount(moveCount);
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
                    HighlightBoltGroup(selectedBolt, true);
                    AudioManager.Instance?.PlaySound(SoundType.Pick);
                }
            }
            else
            {
                // Second tap: deselect or transfer
                if (selectedBolt == clickedBolt)
                {
                    HighlightBoltGroup(selectedBolt, false);
                    selectedBolt = null;
                }
                else
                {
                    TryTransferNuts(selectedBolt, clickedBolt);
                }
            }
        }

        private void HighlightBoltGroup(BoltPeg bolt, bool highlight)
        {
            if (bolt == null || bolt.IsEmpty) return;
            List<Nut> group = bolt.GetConsecutiveTopNutsOfSameColor();

            Vector3 basePos = bolt.baseTransform != null ? bolt.baseTransform.position : bolt.transform.position;
            int startIndex = bolt.stackedNuts.Count - group.Count;

            for (int i = 0; i < group.Count; i++)
            {
                Nut nut = bolt.stackedNuts[startIndex + i];
                Vector3 normalPos = basePos + Vector3.up * ((startIndex + i) * bolt.nutHeight + 0.25f);
                Vector3 targetPos = normalPos + (highlight ? Vector3.up * 0.4f : Vector3.zero);
                nut.transform.position = targetPos;
            }
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

            // Target validity check: cannot transfer if target is full
            if (target.IsFull)
            {
                Deselect(source);
                return;
            }

            // Target validity check: if target is not empty, top nut color must match moving color
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

            // Top-to-bottom transfer list (topmost nut moves first so nuts don't clip through each other)
            List<Nut> nutsToTransfer = new List<Nut>();
            for (int i = 0; i < countToMove; i++)
            {
                nutsToTransfer.Add(movingNuts[i]);
            }

            StartCoroutine(AnimateTransferGroup(source, target, nutsToTransfer));
        }

        private void Deselect(BoltPeg source)
        {
            HighlightBoltGroup(source, false);
            selectedBolt = null;
        }

        private IEnumerator AnimateTransferGroup(BoltPeg source, BoltPeg target, List<Nut> nuts)
        {
            isBusy = true;
            HighlightBoltGroup(source, false);
            selectedBolt = null;

            moveCount++;
            UIManager.Instance?.UpdateMoveCount(moveCount);

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
            Dictionary<BoltColorType, int> colorPegCounts = new Dictionary<BoltColorType, int>();

            foreach (var bolt in bolts)
            {
                if (bolt.IsEmpty) continue;

                if (!bolt.IsSingleColor())
                {
                    return; // Peg contains mixed colors
                }

                BoltColorType color = bolt.TopNut.colorType;
                if (colorPegCounts.ContainsKey(color))
                {
                    return; // Same color split across multiple pegs
                }
                colorPegCounts[color] = 1;
            }

            Debug.Log("LEVEL SOLVED!");
            AudioManager.Instance?.PlaySound(SoundType.Win);
            UIManager.Instance?.ShowLevelCompletePanel();
        }
    }
}
