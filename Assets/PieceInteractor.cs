using UnityEngine;
using UnityEngine.EventSystems;
using System;

// Be tamam qata at ezafe mishe in code
public class PieceInteractor : MonoBehaviour, IPointerClickHandler
{
    public static event Action<int> OnPieceRotated;

    private RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        rectTransform.Rotate(new Vector3(0, 0, 90));
     
        OnPieceRotated?.Invoke(transform.GetSiblingIndex() + 1);
        Debug.Log(OnPieceRotated);
    }
}
