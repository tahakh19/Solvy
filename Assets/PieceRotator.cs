using UnityEngine;
using System.Collections;

public class PieceRotator : MonoBehaviour
{
    public RectTransform puzzlePanel;

    private readonly float[] possibleRotations = { 0f, 90f, 180f, 270f };

 
    void Start()
    {

        StartCoroutine(RotatePiecesAfterDelay(0.1f));
    }


    private IEnumerator RotatePiecesAfterDelay(float delay) // IEnumerator: manage over multiple frames
    {
        yield return new WaitForSeconds(delay); //just this func is delaying and game is going

        if (puzzlePanel == null)
        {
            yield break; 
        }

        if (puzzlePanel.childCount == 0)
        {
            yield break;
        }

        Debug.Log($"PieceRotator: Found {puzzlePanel.childCount} pieces to rotate.");

        // Loop through each piece and apply a random rotation.
        foreach (Transform pieceTransform in puzzlePanel)
        {
            int randomIndex = Random.Range(0, possibleRotations.Length);
            float randomZRotation = possibleRotations[randomIndex];
            pieceTransform.localEulerAngles = new Vector3(0, 0, randomZRotation);
        }

        Debug.Log("PieceRotator: Finished rotating all pieces.");
    }
}
