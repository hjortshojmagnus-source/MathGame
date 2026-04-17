using UnityEngine;
using TMPro;

public class InputFieldVarReplace : MonoBehaviour
{
    public TMP_InputField inputFieldA;      // Input field hvor du skriver værdien for 'a'
    public TMP_InputField inputFieldSkyd;   // Input field med formlen (fx "ax+b")

    // Kaldes når en knap bliver trykket ned
    public void ReplaceAInFormula()
    {
        if (inputFieldA == null)
        {
            Debug.LogError("InputfieldA er ikke assigned!");
            return;
        }

        if (inputFieldSkyd == null)
        {
            Debug.LogError("InputfieldSkyd er ikke assigned!");
            return;
        }

        string valueText = inputFieldA.text.Trim();
        string formula = inputFieldSkyd.text;

        if (string.IsNullOrEmpty(valueText))
        {
            Debug.LogWarning("InputfieldA er tomt!");
            return;
        }

        if (string.IsNullOrEmpty(formula))
        {
            Debug.LogWarning("InputfieldSkyd er tomt!");
            return;
        }

        // Erstat 'a' med værdien fra inputFieldA
        string updatedFormula = formula.Replace("a", valueText);
        inputFieldSkyd.text = updatedFormula;

        Debug.Log("Formel opdateret: " + updatedFormula);
    }
}
