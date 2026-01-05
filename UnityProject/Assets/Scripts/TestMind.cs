using UnityEngine;

public class TestMind : MonoBehaviour
{
    int? x = 5;
    int? y = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(x.HasValue && y.HasValue)
        {
            Debug.Log(x + y);
        }
        else
        {
            Debug.Log("Can't caculate!");
        }
    }

    public int GetValidScore(int? score)
    {
        if (score.HasValue)
        {
            return score.Value;
        }
        else
        {
            return 0;
        }
    }
}
