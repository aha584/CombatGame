using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class InputController : MonoBehaviour
{
    //Control
    public InputControl walkControl;

    //string Bind
    public KeyCode walkKeyName;
    public KeyCode jumpKeyName;
    public KeyCode attackKeyName;
    public KeyCode strikeKeyName;
    public KeyCode flyKickKeyName;
    public KeyCode crouchKeyName;
    public KeyCode blockKeyName;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(CheckString(walkKeyName.ToString()));
        walkControl = InputSystem.FindControl("<Keyboard>/" + CheckString(walkKeyName.ToString()));
        if(walkControl.IsPressed())
        {
            Debug.Log("Is not null");
        }
        if(walkControl is ButtonControl button)
        {
            Debug.Log("press this frame");
        }
    }

    private string CheckString(string keyName)
    {
        if (keyName.Count(char.IsUpper) > 1)
        {
            //Debug.Log("Has 2 Upper!!");
            keyName = Regex.Replace(keyName, "(\\B[A-Z])", " $1");
            string[] parts = keyName.Split(" ");
            parts[0] = parts[0].ToLower();
            keyName = parts[0] + parts[1];
        }
        else
        {
            //Debug.Log("Less than 1 Upper!!!");
            keyName = keyName.ToLower();
        }
        //Can update with mouse input (later)
        if (keyName.Contains("alpha"))
        {
            //Debug.Log("Has alpha!!");
            return keyName.Replace("alpha", ""); // "Alpha 1" -> " 1"
        }
        if (keyName.Contains("keypad"))
        {
            //Debug.Log("Has keypad!!");
            return keyName.Replace("keypad", "numpad"); // "Keypad 1" -> "numpad1"
        }
        if (keyName.Contains("Control"))
        {
            //Debug.Log("Has Control!!");
            return keyName.Replace("Control", "Ctrl");
        }
        if (keyName.Contains("return"))
        {
            //Debug.Log("Has return!!");
            return keyName = "enter";
        }
        
        return keyName;
    }
}
