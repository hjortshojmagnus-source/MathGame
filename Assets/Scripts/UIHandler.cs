using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIHandler : MonoBehaviour
{
    public TMP_InputField inputFieldA;
    [SerializeField] private TMP_InputField inputFieldB;
    [SerializeField] private TMP_InputField inputFieldC;
    [SerializeField] private TMP_InputField inputFieldD;
    [SerializeField] private TMP_InputField inputFieldGraf;
    public Shoot shoot;

    public void SetParam()
    {
        
            if (float.TryParse(inputFieldA.text, out float valueA))
            {
                shoot.changeParameter(0, valueA); // 0 = paramA
                Debug.Log("paramA sat til: " + valueA);
            }
            else
            {
                Debug.LogError("Ugyldigt tal!");
            }
        

        if (inputFieldB)
        {
            if (float.TryParse(inputFieldB.text, out float valueB))
            {
                shoot.changeParameter(1, valueB); // 1 = paramB
                Debug.Log("paramB sat til: " + valueB);
            }
            else
            {
                Debug.LogError("Ugyldigt tal!");
            }
        }

        if (inputFieldC)
        {
            if (float.TryParse(inputFieldC.text, out float valueC))
            {
                shoot.changeParameter(2, valueC); // 2 = paramC
                Debug.Log("paramC sat til: " + valueC);
            }
            else
            {
                Debug.LogError("Ugyldigt tal!");
            }
        }

        if (inputFieldD)
        {
            if (float.TryParse(inputFieldD.text, out float valueD))
            {
                shoot.changeParameter(3, valueD); // 3 = paramD
                Debug.Log("paramD sat til: " + valueD);
            }
            else
            {
                Debug.LogError("Ugyldigt tal!");
            }
        }
    }
    void Start()
    {
        if (inputFieldGraf != null)
        {
            inputFieldGraf.onEndEdit.AddListener(delegate { SetGraf(); });
        }
    }

    public void SetGraf()
    {
        string valueGraf = inputFieldGraf.text.Trim();
        
        if (string.IsNullOrEmpty(valueGraf))
        {
            Debug.LogWarning("UIHandler.SetGraf() - Tomt inputfelt, ignoreres");
            return;
        }

        Debug.Log($"UIHandler.SetGraf() kalder shoot.SetGraf() med: {valueGraf}");
        shoot.SetGraf(valueGraf);
        Debug.Log("Formel opdateret via UI");
    }
}