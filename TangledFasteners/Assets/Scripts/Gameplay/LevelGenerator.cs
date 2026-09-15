using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TangledFasteners
{
    public class LevelGenerator : MonoBehaviour
    {
        public GameObject boltPrefab;
        public GameObject nutPrefab;

        public Transform levelContainer;

        public void GenerateLevel(int levelNumber)
        {
            if (NutSortManager.Instance != null)
            {
                NutSortManager.Instance.ResetState();
            }

            // 1. Clear existing level objects safely
            if (levelContainer == null)
            {
                GameObject containerObj = GameObject.Find("LevelContainer");
                if (containerObj == null) containerObj = new GameObject("LevelContainer");
                levelContainer = containerObj.transform;
            }

            for (int i = levelContainer.childCount - 1; i >= 0; i--)
            {
                GameObject child = levelContainer.GetChild(i).gameObject;
                if (Application.isPlaying)
                {
                    Destroy(child);
                }
                else
                {
                    DestroyImmediate(child);
                }
            }

            // 2. Calculate difficulty parameters per level (More nuts & colors per level)
            int totalColors = Mathf.Clamp(2 + (levelNumber - 1) / 2, 2, 5);
            int boltCapacity = Mathf.Clamp(4 + (levelNumber - 1), 4, 7); // Increased nut capacity (4 to 7 nuts per bolt)
            int totalBolts = totalColors + 2; // Extra empty bolts for sorting moves

            // Equal spacing calculation
            float spacing = 2.4f;
            float startX = -((totalBolts - 1) * spacing) / 2f;

            List<BoltPeg> createdBolts = new List<BoltPeg>();

            bool createdTempBoltPrefab = false;
            if (boltPrefab == null)
            {
                boltPrefab = CreateDefaultBoltMesh(boltCapacity);
                createdTempBoltPrefab = true;
            }

            bool createdTempNutPrefab = false;
            if (nutPrefab == null)
            {
                nutPrefab = CreateDefaultNutMesh();
                createdTempNutPrefab = true;
            }

            // 3. Instantiate Bolts
            for (int i = 0; i < totalBolts; i++)
            {
                Vector3 pos = new Vector3(startX + i * spacing, -2.2f, 0f);
                GameObject bObj = Instantiate(boltPrefab, pos, Quaternion.identity, levelContainer);
                bObj.name = $"BoltPeg_{i + 1}";

                BoltPeg peg = bObj.GetComponent<BoltPeg>();
                if (peg == null) peg = bObj.AddComponent<BoltPeg>();
                peg.capacity = boltCapacity;
                createdBolts.Add(peg);

                if (NutSortManager.Instance != null)
                {
                    NutSortManager.Instance.bolts.Add(peg);
                }
            }

            if (createdTempBoltPrefab)
            {
                if (Application.isPlaying) Destroy(boltPrefab);
                else DestroyImmediate(boltPrefab);
                boltPrefab = null;
            }

            // 4. Create Nut Distribution
            List<BoltColorType> availableColors = new List<BoltColorType>
            {
                BoltColorType.Red, BoltColorType.Blue, BoltColorType.Green, BoltColorType.Yellow, BoltColorType.Purple
            };

            List<BoltColorType> nutList = new List<BoltColorType>();
            for (int c = 0; c < totalColors; c++)
            {
                for (int count = 0; count < boltCapacity; count++)
                {
                    nutList.Add(availableColors[c]);
                }
            }

            // Shuffle nuts randomly for solvable puzzle setup
            for (int i = 0; i < nutList.Count; i++)
            {
                int rnd = Random.Range(i, nutList.Count);
                var temp = nutList[i];
                nutList[i] = nutList[rnd];
                nutList[rnd] = temp;
            }

            // Distribute nuts evenly across filled bolts
            int filledBoltsCount = totalBolts - 1;
            int nutIndex = 0;

            while (nutIndex < nutList.Count)
            {
                for (int b = 0; b < filledBoltsCount; b++)
                {
                    if (nutIndex >= nutList.Count) break;
                    if (createdBolts[b].IsFull) continue;

                    BoltColorType color = nutList[nutIndex++];
                    Vector3 nutPos = createdBolts[b].GetTopSlotPosition();

                    GameObject nObj = Instantiate(nutPrefab, nutPos, Quaternion.identity, levelContainer);
                    nObj.name = $"Nut_{color}";
                    nObj.transform.localScale = new Vector3(0.9f, 0.2f, 0.9f);

                    Nut nut = nObj.GetComponent<Nut>();
                    if (nut == null) nut = nObj.AddComponent<Nut>();
                    nut.colorType = color;
                    nut.ApplyColor();

                    createdBolts[b].AddNut(nut);
                }
            }

            if (createdTempNutPrefab)
            {
                if (Application.isPlaying) Destroy(nutPrefab);
                else DestroyImmediate(nutPrefab);
                nutPrefab = null;
            }
        }

        private GameObject CreateDefaultBoltMesh(int capacity)
        {
            GameObject parent = new GameObject("DefaultBoltPegTemplate");
            GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            cylinder.transform.SetParent(parent.transform);

            // Scale height of bolt according to capacity
            float boltHeightScale = capacity * 0.45f;
            cylinder.transform.localPosition = new Vector3(0, boltHeightScale, 0);
            cylinder.transform.localScale = new Vector3(0.3f, boltHeightScale, 0.3f);

            GameObject baseCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            baseCube.transform.SetParent(parent.transform);
            baseCube.transform.localPosition = Vector3.zero;
            baseCube.transform.localScale = new Vector3(1.2f, 0.2f, 1.2f);

            BoltPeg peg = parent.AddComponent<BoltPeg>();
            peg.baseTransform = baseCube.transform;
            return parent;
        }

        private GameObject CreateDefaultNutMesh()
        {
            GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            cylinder.name = "DefaultNutTemplate";
            cylinder.transform.localScale = new Vector3(0.9f, 0.2f, 0.9f);

            Collider col = cylinder.GetComponent<Collider>();
            if (col != null) DestroyImmediate(col);

            cylinder.AddComponent<Nut>();
            return cylinder;
        }
    }
}
