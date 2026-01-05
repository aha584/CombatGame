using UnityEngine;

public class UITest : MonoBehaviour
{
    public void ValueChange(string input)
    {
        Debug.Log($"Value Change " + input);
    }
    public void EndEdit(string input)
    {
        Debug.Log($"End Edit: " + input);
    }
    public void Select(string input)
    {
        Debug.Log($"Select: " + input);
    }
    public void DeSelect(string input)
    {
        Debug.Log($"DeSelect: " + input);
    }
}
