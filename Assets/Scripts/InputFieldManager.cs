using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InputFieldManager : MonoBehaviour
{
    public TMP_InputField inputField;

    public void ChangeText(string newText)
    {
        if (inputField != null)
        {
            inputField.text = newText;
            inputField.ActivateInputField(); 
        }
    }
}
