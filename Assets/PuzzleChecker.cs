using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class PuzzleChecker : MonoBehaviour
{
    [Header("Puzzle Components")]
    public RectTransform puzzlePanel;

    [Header("Level Settings")]
    public int levelNumber = 1;

    [Header("Win Screen Components")]
    public GameObject winScreenPanel;
    public VideoPlayer videoPlayer;
    public Button replayButton;
    public Button nextLevelButton; // --- for serv

    private GameDataManager dataManager;
    private LevelManager levelManager; // --- serv

    private void OnEnable()
    {
        PieceInteractor.OnPieceRotated += HandlePieceRotation;
    }

    private void OnDisable()
    {
        PieceInteractor.OnPieceRotated -= HandlePieceRotation;
    }

    void Start()
    {
        dataManager = GetComponent<GameDataManager>();
        levelManager = GetComponent<LevelManager>(); // --- serv

        if (winScreenPanel != null)
        {
            winScreenPanel.SetActive(false);
        }

        if (replayButton != null)
        {
            replayButton.onClick.AddListener(ReplayTheVideo);
        }
        
        // --- serv
        if (nextLevelButton != null)
        {
            // When the "Next Level" button is clicked, it will call the LevelManager.
            nextLevelButton.onClick.AddListener(() => levelManager.LoadNextLevel());
        }

        if (dataManager != null)
        {
            dataManager.StartLevel(levelNumber);
        }
    }

    private void HandlePieceRotation(int pieceIndex)
    {
        if (dataManager != null)
        {
            dataManager.RecordClick(pieceIndex);
        }
        CheckPuzzleCompletion();
    }

    public void CheckPuzzleCompletion()
    {
        if (puzzlePanel == null || puzzlePanel.childCount == 0) return;
        bool allPiecesCorrect = true; 
        foreach (Transform piece in puzzlePanel)
        {
            if (Mathf.RoundToInt(piece.localEulerAngles.z) % 360 != 0)
            {
                allPiecesCorrect = false; 
                break;
            }
        }
        
        if (allPiecesCorrect)
        {
            Debug.Log("Puzzle Solved!");
            ShowWinScreen();
            
            if (dataManager != null)
            {
                dataManager.EndLevel();
            }
        }
    }

    private void ShowWinScreen()
    {
        // ... (rest of the method is the same)
        if (winScreenPanel != null)
        {
            winScreenPanel.SetActive(true);
        }
        if (videoPlayer != null)
        {
            videoPlayer.Stop();
            videoPlayer.Play();
        }
    }

    public void ReplayTheVideo()
    {
        // ... (rest of the method is the same)
        if (videoPlayer != null)
        {
            Debug.Log("Replaying video.");
            videoPlayer.Stop();
            videoPlayer.Play();
        }
    }
}
