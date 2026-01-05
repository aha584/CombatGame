using UnityEngine;
using System.Collections.Generic;
using UnityEditor.Build;
using System.Collections;
using System;
using Random = UnityEngine.Random;
using Unity.VisualScripting;

public class TestMind3 : MonoBehaviour
{
    //B4
    private int count = 5;
    private string star = "";
    //B5
    private int number5 = 12341;
    //B6
    private List<int> giaithua = new();
    private int number6 = 145;
    //B7
    private int number7 = 49360489;
    //B9
    private List<int> parentList = new() { 1,3,-3,10,-2,-3,-1};
    private void Start()
    {
        //B4
        //B4(count);
        //B5
        //Debug.Log(B5(number5));
        //B6
        //giaithua.Add(1);
        //for(int i = 1; i < 10; i++)
        //{
        //    giaithua.Add(i * giaithua[i - 1]);
        //}
        //Debug.Log(B6(number6));
        //B7
        //Debug.Log(B7(number7));
        
        MaxSumChildren(parentList);

    }
    public void B4(int count)
    {
        //B4
        for (int i = 0; i < count; i++)
        {
            star += "*";
            Debug.Log(star);
        }
    }
    public bool B5(int number5)
    {
        //B5
        int cloneNum = number5;
        int numCount = 0;
        while (cloneNum > 0)
        {
            cloneNum /= 10;
            numCount++;
        }
        int cloneNum1;
        int cloneNum2;
        if (numCount % 2 == 0)
        {
            int multiplier = (int)Mathf.Pow(10, (numCount / 2));
            cloneNum1 = number5 / multiplier;
            cloneNum1 = cloneNum1 * multiplier;
            cloneNum2 = number5 - cloneNum1;
            cloneNum1 /= multiplier;

            cloneNum = cloneNum1;
            List<int> digit1 = new();
            while (cloneNum > 0)
            {
                int dit = cloneNum % 10;
                cloneNum %= 10;
                digit1.Add(dit);
            }
            cloneNum = cloneNum2;
            List<int> digit2 = new();
            while (cloneNum > 0)
            {
                int dit = cloneNum % 10;
                cloneNum %= 10;
                digit2.Add(dit);
            }
            int k = digit2.Count - 1;
            int checkCount = 0;
            for(int i = 0; i < digit1.Count; i++)
            {
                if (digit1[i] == digit2[k]) checkCount++;
                k--;
            }
            if(checkCount == digit2.Count)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        else
        {
            int multiplier = (int)Mathf.Pow(10, ((numCount - 1) / 2));
            cloneNum1 = number5 / multiplier;
            cloneNum1 = cloneNum1 * multiplier;
            cloneNum2 = number5 - cloneNum1;
            cloneNum1 /= multiplier;

            cloneNum = cloneNum1;
            List<int> digit1 = new();
            while (cloneNum > 0)
            {
                int dit = cloneNum % 10;
                cloneNum %= 10;
                digit1.Add(dit);
            }
            cloneNum = cloneNum2;
            List<int> digit2 = new();
            while (cloneNum > 0)
            {
                int dit = cloneNum % 10;
                cloneNum %= 10;
                digit2.Add(dit);
            }
            int k = digit2.Count - 1;
            int checkCount = 0;
            for (int i = 0; i < digit1.Count; i++)
            {
                if (digit1[i] == digit2[k]) checkCount++;
                k--;
            }
            if (checkCount == digit2.Count)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    public bool B6(int number6)
    {
        int cloneNum = number6;
        List<int> digit = new();
        while (cloneNum > 0)
        {
            int dit = cloneNum % 10;
            cloneNum %= 10;
            digit.Add(dit);
        }
        double sum = 0;
        for(int i = 0; i < digit.Count; i++)
        {
            sum += giaithua[digit[i]];
        }
        if(sum == number6)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public int B7(int number7)
    {
        int cloneNum = number7;
        List<int> digit = new();
        while (cloneNum > 0)
        {
            int dit = cloneNum % 10;
            cloneNum %= 10;
            digit.Add(dit);
        }
        List<int> suffleIndex = new();
        while (digit.Count > suffleIndex.Count)
        {
            int index = Random.Range(0, digit.Count);
            if (!suffleIndex.Contains(index))
            {
                suffleIndex.Add(index);
            }
        }
        List<int> suffleNum = new();
        for(int i = 0; i < suffleIndex.Count; i++)
        {
            suffleNum.Add(digit[suffleIndex[i]]);
        }
        int sum = 0;
        for(int i = 0; i < suffleNum.Count; i++)
        {
            sum += suffleNum[i] * (int)Mathf.Pow(10, i);
        }

        return sum;
    }
    public void MaxSumChildren(List<int> parentList)
    {
        List<int> maxSumForIndex = new();
        Dictionary<int, int> indexRange = new();
        int maxIndex = 0;
        for (int i = 0; i < parentList.Count; i++)
        {
            maxSumForIndex.Add(0);
        }
        for (int i=0;i < parentList.Count; i++)
        {
            if (parentList[i] > maxSumForIndex[0])
            {
                maxSumForIndex[0] = parentList[i];
                maxIndex = i;
            }
        }

        indexRange.Add(0, maxIndex);
        int countMax;
        if(maxIndex >= parentList.Count - maxIndex)
        {
            countMax = parentList.Count - maxIndex;
        }
        else
        {
            countMax = maxIndex;
        }
        int countOfSub = 0;
        while (countMax > 0)
        {
            countOfSub++;
            if (countOfSub % 2 != 0)
            {
                int sum1 = maxSumForIndex[0];
                for (int i = maxIndex; i < maxIndex + countOfSub; i++)
                {
                    sum1 += parentList[i + 1];
                }
                int sum2 = maxSumForIndex[0];
                for (int i = maxIndex; i > maxIndex - countOfSub; i--)
                {
                    sum2 += parentList[i - 1];
                }
                if (sum1 >= sum2)
                {
                    maxSumForIndex[countOfSub] = sum1;
                    indexRange.Add(countOfSub, maxIndex + countOfSub);
                }
                else
                {
                    maxSumForIndex[countOfSub] = sum2;
                    indexRange.Add(countOfSub, maxIndex - countOfSub);
                }
                countMax--;
            }
            else
            {
                int sum1 = maxSumForIndex[0];
                for (int i = maxIndex; i < maxIndex + countOfSub; i++)
                {
                    sum1 += parentList[i + 1];
                }
                int sum2 = maxSumForIndex[0];
                for (int i = maxIndex; i > maxIndex - countOfSub; i--)
                {
                    sum2 += parentList[i - 1];
                }
                int sum3 = 0;
                for (int i = maxIndex - countOfSub; i <= maxIndex + countOfSub; i++)
                {
                    sum3 += parentList[i];
                }
                if (sum1 >= sum2 && sum1 >= sum3)
                {
                    maxSumForIndex[countOfSub] = sum1;
                    indexRange.Add(countOfSub, maxIndex + countOfSub);
                }
                else if (sum2 >= sum1 && sum2 >= sum3)
                {
                    maxSumForIndex[countOfSub] = sum2;
                    indexRange.Add(countOfSub, maxIndex - countOfSub);
                }
                else if(sum3 >= sum1 && sum3 >= sum2)
                {
                    maxSumForIndex[countOfSub] = sum3;
                    indexRange.Add(countOfSub, maxIndex + countOfSub - 1);
                }
                countMax--;
            }
        }
        int maxSubSum = 0;
        int indexRoot = 0;
        for(int i=0; i< maxSumForIndex.Count; i++)
        {
            if (maxSumForIndex[i] > maxSubSum)
            {
                maxSubSum = maxSumForIndex[i];
                indexRoot = i;
            }
        }
        if (indexRoot % 2 == 0 && indexRoot > 1) 
        {
            if (indexRange[indexRoot] > indexRange[0] && indexRange[indexRoot] - indexRange[0] == indexRoot)
            {
                for (int i = indexRange[0]; i <= indexRange[indexRoot]; i++)
                {
                    Debug.Log(parentList[i]);
                }
            }
            else if (indexRange[indexRoot] < indexRange[0] && indexRange[0] - indexRange[indexRoot] == indexRoot)
            {
                for (int i = indexRange[indexRoot]; i <= indexRange[0]; i++)
                {
                    Debug.Log(parentList[i]);
                }
            }
            else if (MathF.Abs(indexRange[0] - indexRange[indexRoot]) < indexRoot)
            {
                if (indexRange[indexRoot] < indexRange[0])
                {
                    for (int i = indexRange[indexRoot]; i <= indexRange[0]; i++)
                    {
                        Debug.Log(parentList[i]);
                    }
                }
                else
                {
                    for (int i = indexRange[0]; i <= indexRange[indexRoot]; i++)
                    {
                        Debug.Log(parentList[i]);
                    }
                }
            }
        }
        else if(indexRoot % 2 != 0 && indexRoot > 1)
        {
            if (indexRange[indexRoot] > indexRange[0])
            {
                for (int i = indexRange[0]; i <= indexRange[indexRoot]; i++)
                {
                    Debug.Log(parentList[i]);
                }
            }
            else
            {
                for (int i = indexRange[indexRoot]; i <= indexRange[0]; i++)
                {
                    Debug.Log(parentList[i]);
                }
            }
        }
    }
    
}
