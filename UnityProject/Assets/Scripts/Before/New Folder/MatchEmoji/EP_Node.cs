using UnityEngine;
using System;
using Unity.VisualScripting;
using TMPro;

public enum NodeState
{
    NotSet,
    Idle,
    Cacul,
    Result
}

public class EP_Node : MonoBehaviour
{
    [SerializeField] private GameObject _borderGreen, _borderRed;
    [SerializeField] private BoxCollider _box;
    [SerializeField] private SpriteRenderer _content;
    [SerializeField] private LineRenderer _line;
    [SerializeField] private Gradient _gradientGreen, _gradientRed;
    [SerializeField] private int _rowId;

    public int _matchId;
    public EP_Node _oppNode;
    public int MatchID => _matchId;
    public int RowId => _rowId;
    private NodeState _state;

    public Action<EP_Node> onClick;
    
    public void SetUp(NodeData data)
    {
        _content.sprite = data.Spr;
        _matchId = data.MatchId;
        _state = NodeState.Idle;
    }

    public void OnClick()
    {
        onClick?.Invoke(this);
    }

    private void OnMouseDown()
    {
        if (_state != NodeState.Idle)
            return;
        
        onClick?.Invoke(this);
    }
    public void SetUpNode1()
    {
        _borderGreen.SetActive(true);
        _line.gameObject.SetActive(true);
    }
    public void ConnectNode2(EP_Node node)
    {
        _line.SetPosition(0, transform.position);
        _line.SetPosition(1, node.transform.position);
        _line.colorGradient = _gradientGreen;
        node._borderGreen.SetActive(true);
        _oppNode = node;
        _oppNode._box.enabled = false;
    }

    public bool IsCorrect()
    {
        return _matchId == _oppNode.MatchID;
    }

    public bool IsConnect()
    {
        return _oppNode != null;
    }

    public void ShowResult()
    {
        if (!IsCorrect())
        {
            _oppNode._borderRed.SetActive(true);
            _line.colorGradient = _gradientRed;
            _borderRed.SetActive(true);

        }
    }
} 
