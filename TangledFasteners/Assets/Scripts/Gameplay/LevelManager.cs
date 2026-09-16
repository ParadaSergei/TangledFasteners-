using UnityEngine;
using UnityEngine.SceneManagement;

namespace TangledFasteners
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance { get; private set; }

        public LevelGenerator levelGenerator;
        public int currentLevel = 1;

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
            LoadCurrentLevel();
        }

        public void LoadCurrentLevel()
        {
            Time.timeScale = 1f;

            if (levelGenerator == null)
            {
                levelGenerator = FindFirstObjectByType<LevelGenerator>();
            }

            if (levelGenerator != null)
            {
                levelGenerator.GenerateLevel(currentLevel);
                UIManager.Instance?.UpdateLevelText(currentLevel);
            }
        }

        public void RestartLevel()
        {
            LoadCurrentLevel();
            UIManager.Instance?.HideLevelCompletePanel();
        }

        private bool isChangingLevel;

        public void NextLevel()
        {
            if (isChangingLevel) return;
            isChangingLevel = true;

            currentLevel++;
            LoadCurrentLevel();
            UIManager.Instance?.HideLevelCompletePanel();

            isChangingLevel = false;
        }

        public void LoadSceneByName(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
