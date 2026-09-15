using System.Collections.Generic;
using UnityEngine;

namespace TangledFasteners
{
    public class Plank : MonoBehaviour
    {
        public List<Hole> attachedHoles = new List<Hole>();
        private Rigidbody rb;
        private HingeJoint activeHinge;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody>();
            }
            rb.isKinematic = true;
        }

        private void Update()
        {
            EvaluatePhysicsState();
        }

        public void EvaluatePhysicsState()
        {
            int activeBolts = 0;
            Hole singleHole = null;

            foreach (var hole in attachedHoles)
            {
                if (hole != null && hole.currentBolt != null)
                {
                    activeBolts++;
                    singleHole = hole;
                }
            }

            if (activeBolts == 0)
            {
                if (activeHinge != null)
                {
                    Destroy(activeHinge);
                    activeHinge = null;
                }

                // Fall down and release completely
                rb.isKinematic = false;
                rb.useGravity = true;
                rb.constraints = RigidbodyConstraints.None;
                Destroy(gameObject, 3f);
            }
            else if (activeBolts == 1 && singleHole != null)
            {
                // Pivot around the single bolt using HingeJoint
                rb.isKinematic = false;
                rb.useGravity = true;

                if (activeHinge == null)
                {
                    activeHinge = gameObject.AddComponent<HingeJoint>();
                    activeHinge.axis = Vector3.forward;
                    activeHinge.anchor = transform.InverseTransformPoint(singleHole.transform.position);
                }
            }
            else
            {
                // Fixed by 2 or more bolts
                if (activeHinge != null)
                {
                    Destroy(activeHinge);
                    activeHinge = null;
                }
                rb.isKinematic = true;
            }
        }
    }
}
