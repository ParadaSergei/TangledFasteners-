using UnityEngine;
using UnityEngine.UI;

namespace TangledFasteners
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("Top Bar UI")]
        public Text levelText;
        public Button restartButton;
        public Button soundButton;
        public Text soundButtonText;

        [Header("Victory Popup UI")]
        public GameObject levelCompletePanel;
        public Text winTitleText;
        public Button continueButton;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            if (restartButton != null)
            {
                restartButton.onClick.RemoveAllListeners();
                restartButton.onClick.AddListener(OnRestartClicked);
            }

            if (soundButton != null)
            {
                soundButton.onClick.RemoveAllListeners();
                soundButton.onClick.AddListener(OnSoundToggleClicked);
            }

            if (continueButton != null)
            {
                continueButton.onClick.RemoveAllListeners();
                continueButton.onClick.AddListener(OnContinueClicked);
            }

            if (levelCompletePanel != null)
                levelCompletePanel.SetActive(false);
        }

        public void UpdateLevelText(int level)
        {
            if (levelText != null)
            {
                levelText.text = $"Уровень {level}";
            }
        }

        public void UpdateSoundButtonUI(bool isMuted)
        {
            if (soundButtonText != null)
            {
                soundButtonText.text = isMuted ? "Звук: ВЫКЛ" : "Звук: ВКЛ";
            }
        }

        public void ShowLevelCompletePanel()
        {
            if (levelCompletePanel != null)
            {
                levelCompletePanel.SetActive(true);
            }
        }

        public void HideLevelCompletePanel()
        {
            if (levelCompletePanel != null)
            {
                levelCompletePanel.SetActive(false);
            }
        }

        private void OnRestartClicked()
        {
            LevelManager.Instance?.RestartLevel();
        }

        private void OnSoundToggleClicked()
        {
            AudioManager.Instance?.ToggleSound();
        }

        private void OnContinueClicked()
        {
            LevelManager.Instance?.NextLevel();
        }
    }
}
