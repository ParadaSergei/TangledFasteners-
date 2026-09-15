using UnityEngine;

namespace TangledFasteners
{
    public class Hole : MonoBehaviour
    {
        public Bolt currentBolt;

        public bool IsEmpty => currentBolt == null;

        public void AttachBolt(Bolt bolt)
        {
            if (currentBolt != null && currentBolt != bolt)
            {
                currentBolt.currentHole = null;
            }

            currentBolt = bolt;
            if (bolt != null)
            {
                bolt.currentHole = this;
                bolt.transform.position = transform.position;
            }
        }

        public void DetachBolt()
        {
            if (currentBolt != null)
            {
                currentBolt.currentHole = null;
                currentBolt = null;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = IsEmpty ? Color.green : Color.red;
            Gizmos.DrawWireSphere(transform.position, 0.15f);
        }
    }
}
