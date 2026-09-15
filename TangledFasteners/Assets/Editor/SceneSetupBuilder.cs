using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using TangledFasteners;

namespace TangledFasteners.Editor
{
    public class SceneSetupBuilder
    {
        [MenuItem("TangledFasteners/Build Game Scene")]
        public static void BuildScene()
        {
            // Open or create active scene
            var scene = EditorSceneManager.GetActiveScene();

            // 1. Setup Camera & Light
            Camera mainCam = Camera.main;
            if (mainCam == null)
            {
                GameObject camObj = new GameObject("Main Camera");
                mainCam = camObj.AddComponent<Camera>();
                camObj.tag = "MainCamera";
            }
            mainCam.transform.position = new Vector3(0, 0, -10f);
            mainCam.transform.rotation = Quaternion.identity;
            mainCam.orthographic = true;
            mainCam.orthographicSize = 6.5f;

            Light dirLight = Object.FindFirstObjectByType<Light>();
            if (dirLight == null)
            {
                GameObject lightObj = new GameObject("Directional Light");
                dirLight = lightObj.AddComponent<Light>();
                dirLight.type = LightType.Directional;
            }
            dirLight.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

            // 2. Setup Board
            GameObject board = GameObject.CreatePrimitive(PrimitiveType.Cube);
            board.name = "Board";
            board.transform.position = new Vector3(0, 0, 1f);
            board.transform.localScale = new Vector3(8f, 10f, 0.5f);

            // 3. Setup Managers
            GameObject mgrObj = new GameObject("GameManager");
            GameManager gm = mgrObj.AddComponent<GameManager>();
            mgrObj.AddComponent<InputController>();
            BufferManager bm = mgrObj.AddComponent<BufferManager>();
            gm.bufferManager = bm;

            // 4. Create Buffer Slots (At Top)
            GameObject bufferParent = new GameObject("BufferSlots");
            Hole[] bufferHoles = new Hole[5];
            for (int i = 0; i < 5; i++)
            {
                GameObject slotObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                slotObj.name = $"BufferSlot_{i}";
                slotObj.transform.SetParent(bufferParent.transform);
                slotObj.transform.position = new Vector3(-3f + i * 1.5f, 4.5f, 0.0f);
                slotObj.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
                slotObj.transform.localScale = new Vector3(0.8f, 0.1f, 0.8f);

                Hole hole = slotObj.AddComponent<Hole>();
                bufferHoles[i] = hole;
            }
            bm.bufferHoles = bufferHoles;

            // 5. Create Sorting Boxes (At Bottom)
            GameObject boxParent = new GameObject("SortingBoxes");
            SortingBox boxRed = CreateSortingBox("SortingBox_Red", BoltColorType.Red, new Vector3(-2f, -4.5f, 0.0f), boxParent.transform);
            SortingBox boxBlue = CreateSortingBox("SortingBox_Blue", BoltColorType.Blue, new Vector3(2f, -4.5f, 0.0f), boxParent.transform);
            gm.sortingBoxes = new SortingBox[] { boxRed, boxBlue };

            // 6. Create Holes on Board (Z = 0f)
            GameObject holesParent = new GameObject("BoardHoles");
            Hole h1 = CreateHole("Hole_1", new Vector3(-2f, 1.5f, 0.0f), holesParent.transform);
            Hole h2 = CreateHole("Hole_2", new Vector3(2f, 1.5f, 0.0f), holesParent.transform);
            Hole h3 = CreateHole("Hole_3", new Vector3(-2f, -1.5f, 0.0f), holesParent.transform);
            Hole h4 = CreateHole("Hole_4", new Vector3(2f, -1.5f, 0.0f), holesParent.transform);

            // 7. Create Planks (Z = 0.2f behind bolts)
            GameObject planksParent = new GameObject("Planks");
            Plank p1 = CreatePlank("Plank_Top", new Vector3(0f, 1.5f, 0.2f), new Vector3(5f, 0.8f, 0.2f), new Hole[] { h1, h2 }, planksParent.transform);
            Plank p2 = CreatePlank("Plank_Bottom", new Vector3(0f, -1.5f, 0.2f), new Vector3(5f, 0.8f, 0.2f), new Hole[] { h3, h4 }, planksParent.transform);

            // 8. Create Bolts (Attached to holes at Z = 0f)
            GameObject boltsParent = new GameObject("Bolts");
            Bolt b1 = CreateBolt("Bolt_Red_1", BoltColorType.Red, h1, boltsParent.transform);
            Bolt b2 = CreateBolt("Bolt_Blue_1", BoltColorType.Blue, h2, boltsParent.transform);
            Bolt b3 = CreateBolt("Bolt_Red_2", BoltColorType.Red, h3, boltsParent.transform);
            Bolt b4 = CreateBolt("Bolt_Blue_2", BoltColorType.Blue, h4, boltsParent.transform);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("Game Scene Built Successfully!");
        }

        private static Hole CreateHole(string name, Vector3 pos, Transform parent)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            obj.name = name;
            obj.transform.SetParent(parent);
            obj.transform.position = pos;
            obj.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            obj.transform.localScale = new Vector3(0.6f, 0.05f, 0.6f);
            return obj.AddComponent<Hole>();
        }

        private static Plank CreatePlank(string name, Vector3 pos, Vector3 scale, Hole[] holes, Transform parent)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj.name = name;
            obj.transform.SetParent(parent);
            obj.transform.position = pos;
            obj.transform.localScale = scale;

            Plank plank = obj.AddComponent<Plank>();
            plank.attachedHoles.AddRange(holes);
            return plank;
        }

        private static Bolt CreateBolt(string name, BoltColorType color, Hole hole, Transform parent)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            obj.name = name;
            obj.transform.SetParent(parent);
            obj.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            obj.transform.localScale = new Vector3(0.5f, 0.3f, 0.5f);

            Bolt bolt = obj.AddComponent<Bolt>();
            bolt.colorType = color;
            bolt.meshRenderer = obj.GetComponent<Renderer>();
            bolt.ApplyColorMaterial();

            if (hole != null)
            {
                hole.AttachBolt(bolt);
            }
            return bolt;
        }

        private static SortingBox CreateSortingBox(string name, BoltColorType color, Vector3 pos, Transform parent)
        {
            GameObject boxObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            boxObj.name = name;
            boxObj.transform.SetParent(parent);
            boxObj.transform.position = pos;
            boxObj.transform.localScale = new Vector3(2.5f, 1.2f, 0.5f);

            SortingBox box = boxObj.AddComponent<SortingBox>();
            box.targetColor = color;
            box.capacity = 3;
            box.boxRenderer = boxObj.GetComponent<Renderer>();

            Transform[] slots = new Transform[3];
            for (int i = 0; i < 3; i++)
            {
                GameObject slot = new GameObject($"Slot_{i}");
                slot.transform.SetParent(boxObj.transform);
                slot.transform.localPosition = new Vector3(-0.35f + i * 0.35f, 0f, -0.6f);
                slots[i] = slot.transform;
            }
            box.slotPositions = slots;

            return box;
        }
    }
}
