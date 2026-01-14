using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class TakeHUDInfo : MonoBehaviour
{
    public Health playerHealth;
    public Slider healthSlider;
    public TMP_Text playerName;
    public TMP_Text healthPercentage;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        healthSlider.maxValue = playerHealth.maxHealth;
        healthSlider.value = playerHealth.maxHealth;
        playerName.text = playerHealth.gameObject.name;
        playerName.ForceMeshUpdate();
        healthPercentage.text = $"{playerHealth.maxHealth}/{playerHealth.maxHealth}";
        playerHealth.onHealthChange += OnUpdateSlider;
    }

    public void OnUpdateSlider()
    {
        //Debug.Log("Minus Health");
        healthSlider.value = playerHealth.currentHealth;
        healthPercentage.text = $"{Math.Round(playerHealth.currentHealth, 3)}/{playerHealth.maxHealth}";
        healthPercentage.ForceMeshUpdate();
        if(healthSlider.value <= 0)
        {
            GameObject fillArea = healthSlider.transform.Find("Fill Area").gameObject;
            fillArea.SetActive(false);
        }
    }
}
