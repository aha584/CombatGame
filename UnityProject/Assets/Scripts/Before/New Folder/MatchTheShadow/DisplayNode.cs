using System;
using Unity.VisualScripting;
using UnityEngine;

public class DisplayNode : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spr;
    [SerializeField] private BoxCollider2D _box;

    private Vector3 _originPos;
    private int _id;
    private bool _isCorrect;

    public int Id => _id;
    public bool IsCorrect => _isCorrect;

    public UnityEngine.Events.UnityEvent<DisplayNode> onClick;

    public void SetUp(NodeData data)
    {
        _spr.sprite = data.Spr;
        _id = data.MatchId;
        _originPos = transform.position;
        _isCorrect = false;
    }

    private void OnMouseDown()
    {
        //Debug.Log(name);
        onClick?.Invoke(this);  
    }

    public void Correct(ShadowNode node)
    {
        _isCorrect = true;
        _box.enabled = false;
        transform.position = node.transform.position;
    }

    public void Wrong()
    {
        transform.position = _originPos;
    }
}
