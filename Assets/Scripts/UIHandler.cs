using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIHandler : MonoBehaviour
{
    public TMP_InputField inputFieldA;
    public TMP_InputField inputFieldB;
    public TMP_InputField inputFieldC;
    public TMP_InputField inputFieldD;
    public Shoot shoot;

    public void SetParam()
    {
        
            if (float.TryParse(inputField.text, out float value))
            {
                shoot.changeParameter(0, value); // 0 = paramA
                Debug.Log("paramA sat til: " + value);
            }
            else
            {
                Debug.LogError("Ugyldigt tal!");
            }
        

        if (inputFieldB)
        {
            if (float.TryParse(inputField.text, out float value))
            {
                shoot.changeParameter(0, value); // 0 = paramB
                Debug.Log("paramB sat til: " + value);
            }
            else
            {
                Debug.LogError("Ugyldigt tal!");
            }
        }

        if (inputFieldC)
        {
            if (float.TryParse(inputField.text, out float value))
            {
                shoot.changeParameter(0, value); // 0 = paramC
                Debug.Log("paramC sat til: " + value);
            }
            else
            {
                Debug.LogError("Ugyldigt tal!");
            }
        }

        if (inputFieldD)
        {
            if (float.TryParse(inputField.text, out float value))
            {
                shoot.changeParameter(0, value); // 0 = paramD
                Debug.Log("paramD sat til: " + value);
            }
            else
            {
                Debug.LogError("Ugyldigt tal!");
            }
        }
    }
}