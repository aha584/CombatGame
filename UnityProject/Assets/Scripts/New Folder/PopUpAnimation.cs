using UnityEngine;
using DG.Tweening;

public class PopUpAnimation : MonoBehaviour
{
    [SerializeField] private float duration;
    private void OnEnable()
    {
        transform.DOScale(1, duration).From(0).SetEase(Ease.OutBack);
    }
    public void OnClick()
    {
        transform.DOScale(0, duration).From(1).SetEase(Ease.InBack);
    }
}
