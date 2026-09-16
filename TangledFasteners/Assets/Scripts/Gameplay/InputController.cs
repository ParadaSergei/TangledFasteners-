using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace TangledFasteners
{
    public class InputController : MonoBehaviour
    {
        public Camera mainCamera;

        private void Start()
        {
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }
        }

        private void Update()
        {
            if (Pointer.current != null && Pointer.current.press.wasPressedThisFrame)
            {
                // Ignore clicks on UI elements so tapping buttons doesn't select 3D objects
                if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                {
                    return;
                }

                Vector2 touchPosition = Pointer.current.position.ReadValue();
                HandleTap(touchPosition);
            }
        }

        private void HandleTap(Vector2 screenPosition)
        {
            if (mainCamera == null) mainCamera = Camera.main;
            if (mainCamera == null) return;

            Ray ray = mainCamera.ScreenPointToRay(screenPosition);
            RaycastHit[] hits = Physics.RaycastAll(ray, 100f);

            foreach (var hit in hits)
            {
                BoltPeg peg = hit.collider.GetComponentInParent<BoltPeg>();
                if (peg != null)
                {
                    NutSortManager.Instance?.OnBoltClicked(peg);
                    break;
                }
            }
        }
    }
}
