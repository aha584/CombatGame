using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class TestDOTween : MonoBehaviour
{
    [SerializeField] private List<Transform> objs;
    [SerializeField] private Transform target;
    [SerializeField] private TMP_Text text;
    [SerializeField] private float _jumpPower;
    [SerializeField] private Transform target2;
    [SerializeField] private float duration;

    private List<Vector3> _originPos = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //transform.DOScale(3, duration).From(0).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutBounce);
        /*
        Sequence seq = DOTween.Sequence();
        seq.Insert(0f, transform.DOMoveX(target2.position.x, duration).SetEase(Ease.Linear));
        seq.Insert(0f, transform.DOMoveY(target2.position.y, duration).SetEase(Ease.OutQuad));
        seq.Insert(duration,transform.DOMoveY(target.position.y, duration).SetEase(Ease.InQuad));
        seq.Insert(duration,transform.DOMoveX(target.position.x, duration).SetEase(Ease.Linear));
        seq.SetLoops(-1, LoopType.Restart);
        */
        //transform.DOMoveX(target.position.x, duration).SetEase(Ease.INTERNAL_Custom);
        //DOVirtual.DelayedCall(duration, () => seq.Play());
        //transform.DORotate(new Vector3(360,360,360), duration, RotateMode.LocalAxisAdd).SetLoops(-1, LoopType.Restart).SetEase(Ease.OutQuad);

        //DOVirtual.Float(0f, 1f, duration, Process);

        transform.DOJump(target.position, _jumpPower, 1, duration); //Exp pick a coin on ground and it jump to UI that contain amount of coin
        //Or jump from enemy to ground
        //NEw Problem
        //with more object? more tween?
        //No
        //Grab all Object to 1 Array/List
        //and add all originPos aff all Object to 1 Array/List of Vector3
        //and use loops to Tween for each one
        //DoVirtual.Float (DoTween.To) (0f,1f, duration, Move)
        for (int i=0;i<objs.Count;i++)
        {
            _originPos[i] = objs[i].position;
        }
        DOVirtual.Float(0f, 1f, duration, Move);
    }

    private void Move(float t)
    {
        for(int i=0;i<objs.Count;i++)
        {
            Vector3 pos = Vector3.Lerp(_originPos[i], target.position, t);
            objs[i].transform.position = pos;
        }
    }
    private void Process(float t)
    {
        float v = Mathf.Lerp(0, 100f, t);
        text.text = ((int)v).ToString();
    }
}
