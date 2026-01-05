using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using System;

public class MatchTheShadow : MonoBehaviour, IMiniGames
{
    [SerializeField] private List<DisplayNode> _displayNode;
    [SerializeField] private List<ShadowNode> _shadowNode;

    private DisplayNode _currentDragNode;
    private Camera _cam;
    private int correctCount = 0;
    public Action onWin { get; set; } 
    public Action onLose { get; set; }

    public bool CheckWin()
    {
        if(correctCount == 3)
        {
            return true;
        }
        return false;
    }

    public void Init(LevelData lvData)
    {
        for(int i = 0; i< _displayNode.Count; i++)
        {
            _displayNode[i].SetUp(lvData.NodeData[i]);
            _displayNode[i].onClick.AddListener(OnClick);
        }
        for (int i = 0; i < _shadowNode.Count; i++)
        {
            _shadowNode[i].SetUp(lvData.NodeData[i]);
        }
        _cam = Camera.main;
    }

    public void Lose()
    {
        onLose?.Invoke();
    }

    public void Win()
    {
        onWin?.Invoke();
        
    }

    private void OnClick(DisplayNode node)
    {
        if(_currentDragNode == null)
        {
            _currentDragNode = node;
        }
    }

    private void Update()
    {
        if(_currentDragNode == null)
        {
            return;
        }
        Vector3 worldPos = _cam.ScreenToWorldPoint(Input.mousePosition);
        worldPos = new Vector3(worldPos.x, worldPos.y, 0);
        _currentDragNode.transform.position = worldPos;

        if(Input.GetMouseButtonUp(0))
        {
            if(CheckDropCorrect(out ShadowNode node))
            {
                _currentDragNode.Correct(node);
                correctCount++;
                if(CheckWin())
                {
                    correctCount = 0;
                    Win();
                }
            }
            else
            {
                _currentDragNode.Wrong();
            }
            _currentDragNode = null;
        }
    }
    private bool CheckDropCorrect(out ShadowNode node)
    {
        node = null;

        foreach (var shadow in _shadowNode)
        {
            if(shadow.InsideShadow(_currentDragNode.transform.position, _currentDragNode.Id))
            {
                node = shadow;
                return true;
            }
        }
        return false;
    }
}
