using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    private const int TOTAL_LEVELS = 5;

    [Tooltip("The base name of your puzzle scenes (e.g., 'Level' for scenes named 'Level1', 'Level2', etc.).")]
    public string levelSceneNameBase = "Level";

    [Tooltip("The name of the scene that will show the analysis.")]
    public string analysisSceneName = "AnalysisScene";


    public void LoadNextLevel()
    {
        PuzzleChecker checker = GetComponent<PuzzleChecker>();
        if (checker == null)
        {
            Debug.LogError("LevelManager could not find PuzzleChecker script.");
            return;
        }

        int currentLevel = checker.levelNumber;

        if (currentLevel < TOTAL_LEVELS)
        {
            int nextLevel = currentLevel + 1;
            string sceneToLoad = levelSceneNameBase + nextLevel;
            Debug.Log($"Loading next level: {sceneToLoad}");
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.Log("All levels complete! Loading Analysis Scene.");
            SceneManager.LoadScene(analysisSceneName);
        }
    }
}
