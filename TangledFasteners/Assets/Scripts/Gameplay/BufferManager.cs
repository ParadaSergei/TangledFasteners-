using System.Collections.Generic;
using UnityEngine;

namespace TangledFasteners
{
    public class BufferManager : MonoBehaviour
    {
        public Hole[] bufferHoles;

        public Hole GetFreeBufferHole()
        {
            foreach (var hole in bufferHoles)
            {
                if (hole != null && hole.IsEmpty)
                {
                    return hole;
                }
            }
            return null;
        }

        public bool IsBufferFull()
        {
            foreach (var hole in bufferHoles)
            {
                if (hole != null && hole.IsEmpty)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
