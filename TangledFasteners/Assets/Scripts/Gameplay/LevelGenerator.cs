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
            if (levelContainer != null)
            {
                foreach (Transform child in levelContainer)
                {
                    DestroyImmediate(child.gameObject);
                }
            }
            else
            {
                levelContainer = new GameObject("LevelContainer").transform;
            }

            if (NutSortManager.Instance != null)
            {
                NutSortManager.Instance.bolts.Clear();
            }

            // Calculate difficulty params
            int totalColors = Mathf.Clamp(2 + (levelNumber - 1) / 2, 2, 5);
            int boltCapacity = Mathf.Clamp(2 + (levelNumber - 1) / 3, 2, 5);
            int totalBolts = totalColors + 2; // Extra empty bolts for sorting space

            // Equal spacing calculation
            float spacing = 2.2f;
            float startX = -((totalBolts - 1) * spacing) / 2f;

            List<BoltPeg> createdBolts = new List<BoltPeg>();

            bool createdTempBoltPrefab = false;
            if (boltPrefab == null)
            {
                boltPrefab = CreateDefaultBoltMesh();
                createdTempBoltPrefab = true;
            }

            bool createdTempNutPrefab = false;
            if (nutPrefab == null)
            {
                nutPrefab = CreateDefaultNutMesh();
                createdTempNutPrefab = true;
            }

            // Create bolts
            for (int i = 0; i < totalBolts; i++)
            {
                Vector3 pos = new Vector3(startX + i * spacing, -1.5f, 0f);
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

            // Clean up temp prefab if created procedurally
            if (createdTempBoltPrefab)
            {
                DestroyImmediate(boltPrefab);
                boltPrefab = null;
            }

            // Create nuts distribution (guaranteed solvable)
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

            // Shuffle nuts
            for (int i = 0; i < nutList.Count; i++)
            {
                int rnd = Random.Range(i, nutList.Count);
                var temp = nutList[i];
                nutList[i] = nutList[rnd];
                nutList[rnd] = temp;
            }

            // Fill initial bolts (leave last 2 bolts empty)
            int nutIndex = 0;
            for (int b = 0; b < totalColors; b++)
            {
                for (int slot = 0; slot < boltCapacity; slot++)
                {
                    if (nutIndex >= nutList.Count) break;

                    BoltColorType color = nutList[nutIndex++];
                    Vector3 nutPos = createdBolts[b].GetTopSlotPosition();

                    GameObject nObj = Instantiate(nutPrefab, nutPos, Quaternion.identity, createdBolts[b].transform);
                    nObj.name = $"Nut_{color}";

                    Nut nut = nObj.GetComponent<Nut>();
                    if (nut == null) nut = nObj.AddComponent<Nut>();
                    nut.colorType = color;
                    nut.ApplyColor();

                    createdBolts[b].AddNut(nut);
                }
            }

            if (createdTempNutPrefab)
            {
                DestroyImmediate(nutPrefab);
                nutPrefab = null;
            }
        }

        private GameObject CreateDefaultBoltMesh()
        {
            GameObject parent = new GameObject("DefaultBoltPegTemplate");
            GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            cylinder.transform.SetParent(parent.transform);
            cylinder.transform.localPosition = new Vector3(0, 1.5f, 0);
            cylinder.transform.localScale = new Vector3(0.3f, 1.5f, 0.3f);

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
            cylinder.AddComponent<Nut>();
            return cylinder;
        }
    }
}
