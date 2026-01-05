using System;
using UnityEngine;

public class ShadowNode : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spr;
    [SerializeField] private float _threshold;

    private int _id;
    public int Id => _id;

    public void SetUp(NodeData data)
    {
        _spr.sprite = data.Spr;
        _id = data.MatchId;
    }

    public bool InsideShadow(Vector3 pos, int id)
    {
        bool correctId = id == _id;
        bool d = Vector3.Distance(pos, transform.position) < _threshold;

        return correctId && d;
    }

}
