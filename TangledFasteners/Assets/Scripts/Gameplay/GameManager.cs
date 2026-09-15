using UnityEngine;

namespace TangledFasteners
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public BufferManager bufferManager;
        public SortingBox[] sortingBoxes;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public bool TryProcessBoltClick(Bolt bolt)
        {
            if (bolt == null || bolt.isMoving) return false;

            // 1. Try send to matching sorting box
            SortingBox matchingBox = FindMatchingBox(bolt);
            if (matchingBox != null)
            {
                Hole sourceHole = bolt.currentHole;
                if (sourceHole != null) sourceHole.DetachBolt();

                Transform targetSlot = matchingBox.GetNextSlotTransform();
                matchingBox.AddBolt(bolt);

                StartCoroutine(bolt.UnscrewAndMoveTo(targetSlot.position, () =>
                {
                    bolt.transform.SetParent(targetSlot);
                    CheckGameStatus();
                }));
                return true;
            }

            // 2. Try send to buffer hole if box is unavailable
            if (bufferManager != null)
            {
                Hole freeBufferHole = bufferManager.GetFreeBufferHole();
                if (freeBufferHole != null)
                {
                    Hole sourceHole = bolt.currentHole;
                    if (sourceHole != null) sourceHole.DetachBolt();

                    freeBufferHole.AttachBolt(bolt);

                    StartCoroutine(bolt.UnscrewAndMoveTo(freeBufferHole.transform.position, () =>
                    {
                        CheckGameStatus();
                    }));
                    return true;
                }
            }

            Debug.LogWarning("No available box or buffer space!");
            return false;
        }

        private SortingBox FindMatchingBox(Bolt bolt)
        {
            if (sortingBoxes == null) return null;
            foreach (var box in sortingBoxes)
            {
                if (box != null && box.CanAccept(bolt))
                {
                    return box;
                }
            }
            return null;
        }

        public void OnBoxCleared()
        {
            // Re-check buffered bolts to see if any can now enter newly active boxes
            CheckBufferedBolts();
            CheckGameStatus();
        }

        private void CheckBufferedBolts()
        {
            if (bufferManager == null || bufferManager.bufferHoles == null) return;

            foreach (var bHole in bufferManager.bufferHoles)
            {
                if (bHole != null && !bHole.IsEmpty)
                {
                    Bolt bBolt = bHole.currentBolt;
                    SortingBox matchBox = FindMatchingBox(bBolt);
                    if (matchBox != null)
                    {
                        bHole.DetachBolt();
                        Transform targetSlot = matchBox.GetNextSlotTransform();
                        matchBox.AddBolt(bBolt);
                        StartCoroutine(bBolt.UnscrewAndMoveTo(targetSlot.position, () =>
                        {
                            bBolt.transform.SetParent(targetSlot);
                            CheckGameStatus();
                        }));
                    }
                }
            }
        }

        public void CheckGameStatus()
        {
            Bolt[] remainingBolts = FindObjectsByType<Bolt>(FindObjectsSortMode.None);
            if (remainingBolts.Length == 0)
            {
                Debug.Log("LEVEL COMPLETED! VICTORY!");
            }
            else if (bufferManager != null && bufferManager.IsBufferFull())
            {
                // Check if any bolt in buffer can move
                bool validMoveExists = false;
                foreach (var bHole in bufferManager.bufferHoles)
                {
                    if (!bHole.IsEmpty && FindMatchingBox(bHole.currentBolt) != null)
                    {
                        validMoveExists = true;
                        break;
                    }
                }

                if (!validMoveExists)
                {
                    Debug.Log("GAME OVER! Buffer is full with no matching boxes.");
                }
            }
        }
    }
}
