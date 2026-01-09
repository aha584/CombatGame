using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class InputController : MonoBehaviour
{
    //Control
    public InputControl walkRightControl;
    public InputControl walkLeftControl;
    public InputControl jumpControl;
    public InputControl attackControl;
    public InputControl strikeControl;
    public InputControl flyKickControl;
    public InputControl crouchControl;
    public InputControl blockControl;
    public InputControl castControl;
    public InputControl dashControl;

    public ButtonControl tempButton;

    //KeyCode Bind
    public KeyCode walkRightKey;
    public KeyCode walkLeftKey;
    public KeyCode jumpKey;
    public KeyCode attackKey;
    public KeyCode strikeKey;
    public KeyCode flyKickKey;
    public KeyCode crouchKey;
    public KeyCode blockKey;
    public KeyCode castKey;
    public KeyCode dashKey;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Debug.Log(CheckString(walkKeyName.ToString()));
        walkRightControl = InputSystem.FindControl("<Keyboard>/" + CheckString(walkRightKey.ToString()));
        walkLeftControl = InputSystem.FindControl("<Keyboard>/" + CheckString(walkLeftKey.ToString()));
        jumpControl = InputSystem.FindControl("<Keyboard>/" + CheckString(jumpKey.ToString()));
        attackControl = InputSystem.FindControl("<Keyboard>/" + CheckString(attackKey.ToString()));
        strikeControl = InputSystem.FindControl("<Keyboard>/" + CheckString(strikeKey.ToString()));
        flyKickControl = InputSystem.FindControl("<Keyboard>/" + CheckString(flyKickKey.ToString()));
        crouchControl = InputSystem.FindControl("<Keyboard>/" + CheckString(crouchKey.ToString()));
        blockControl = InputSystem.FindControl("<Keyboard>/" + CheckString(blockKey.ToString()));
        castControl = InputSystem.FindControl("<Keyboard>/" + CheckString(castKey.ToString()));
        dashControl = InputSystem.FindControl("<Keyboard>/"+CheckString(dashKey.ToString()));
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
