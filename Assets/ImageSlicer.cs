using UnityEngine;
using UnityEngine.UI;

public class ImageSlicer : MonoBehaviour
{
    [Tooltip("The source image you want to slice into a puzzle.")]
    public Texture2D sourceImage;

    [Tooltip("The UI Panel that will hold all the puzzle pieces.")]
    public RectTransform puzzlePanel;

    [Tooltip("The dimension of the puzzle. For a 3x3 puzzle, enter 3. For a 4x4, enter 4, etc.")]
    [Range(2, 10)] 
    public int n = 3;

    void Start()
    {
        GeneratePuzzle();
    }

 
    public void GeneratePuzzle()
    {
        if (sourceImage == null)
        {
            return;
        }
        if (puzzlePanel == null)
        {
            return;
        }

        foreach (Transform child in puzzlePanel)
        {
            if (Application.isEditor && !Application.isPlaying)
            {
                DestroyImmediate(child.gameObject);
            }
            else
            {
                Destroy(child.gameObject);
            }
        }

        int sliceWidth = sourceImage.width / n;
        int sliceHeight = sourceImage.height / n;

        float panelWidth = puzzlePanel.rect.width;
        float panelHeight = puzzlePanel.rect.height;

        float pieceUIWidth = panelWidth / n;
        float pieceUIHeight = panelHeight / n;

        for (int row = 0; row < n; row++)
        {
            for (int col = 0; col < n; col++)
            {
                Rect spriteRect = new Rect(col * sliceWidth, (n - 1 - row) * sliceHeight, sliceWidth, sliceHeight);
                Sprite pieceSprite = Sprite.Create(sourceImage, spriteRect, new Vector2(0.5f, 0.5f));

                GameObject pieceObject = new GameObject($"Piece_{row}_{col}");
                pieceObject.transform.SetParent(puzzlePanel.transform, false);

                Image imageComponent = pieceObject.AddComponent<Image>();
                imageComponent.sprite = pieceSprite;

          
                pieceObject.AddComponent<PieceInteractor>();

                RectTransform rectTransform = pieceObject.GetComponent<RectTransform>();

                rectTransform.anchorMin = new Vector2(0, 1);
                rectTransform.anchorMax = new Vector2(0, 1);
                rectTransform.pivot = new Vector2(0.5f, 0.5f);

                rectTransform.sizeDelta = new Vector2(pieceUIWidth, pieceUIHeight);

                float xPos = (col * pieceUIWidth) + (pieceUIWidth / 2);
                float yPos = (-row * pieceUIHeight) - (pieceUIHeight / 2);
                rectTransform.anchoredPosition = new Vector2(xPos, yPos);
            }
        }

        if (!Application.isPlaying)
        {
            Debug.Log($"Successfully generated a {n}x{n} puzzle in the editor!");
        }
    }
}
