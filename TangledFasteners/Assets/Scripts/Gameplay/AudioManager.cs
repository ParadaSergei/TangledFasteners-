using UnityEngine;

namespace TangledFasteners
{
    public enum SoundType
    {
        Pick,
        Place,
        Win
    }

    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        public AudioSource audioSource;
        public AudioClip pickClip;
        public AudioClip placeClip;
        public AudioClip winClip;

        public bool IsMuted { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        public void ToggleSound()
        {
            IsMuted = !IsMuted;
            if (audioSource != null)
            {
                audioSource.mute = IsMuted;
            }
            UIManager.Instance?.UpdateSoundButtonUI(IsMuted);
        }

        public void PlaySound(SoundType sound)
        {
            if (IsMuted || audioSource == null) return;

            AudioClip clip = null;
            switch (sound)
            {
                case SoundType.Pick: clip = pickClip; break;
                case SoundType.Place: clip = placeClip; break;
                case SoundType.Win: clip = winClip; break;
            }

            if (clip != null)
            {
                audioSource.PlayOneShot(clip);
            }
        }
    }
}
