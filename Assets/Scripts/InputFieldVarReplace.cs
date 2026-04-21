using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;

public class InputFieldVarReplace : MonoBehaviour
{
    public TMP_InputField inputFieldA;      // Input field hvor du skriver værdien for 'a'
    public TMP_InputField inputFieldSkyd;   // Input field med formlen (fx "ax+b")
    public TMP_InputField inputFieldB;
    public TMP_InputField inputFieldC;
    public TMP_InputField inputFieldD;

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
    public void ReplaceBInFormula()
    {
        if (inputFieldB == null)
        {
            Debug.LogError("InputfieldA er ikke assigned!");
            return;
        }

        if (inputFieldSkyd == null)
        {
            Debug.LogError("InputfieldSkyd er ikke assigned!");
            return;
        }

        string valueText = inputFieldB.text.Trim();
        string formula = inputFieldSkyd.text;

        if (string.IsNullOrEmpty(valueText))
        {
            Debug.LogWarning("InputfieldB er tomt!");
            return;
        }

        if (string.IsNullOrEmpty(formula))
        {
            Debug.LogWarning("InputfieldSkyd er tomt!");
            return;
        }

        // Erstat 'b' med værdien fra inputFieldB
        string updatedFormula = formula.Replace("b", valueText);
        inputFieldSkyd.text = updatedFormula;

        Debug.Log("Formel opdateret: " + updatedFormula);
    }
    public void ReplaceCInFormula()
    {
        if (inputFieldC == null)
        {
            Debug.LogError("InputfieldC er ikke assigned!");
            return;
        }

        if (inputFieldSkyd == null)
        {
            Debug.LogError("InputfieldSkyd er ikke assigned!");
            return;
        }

        string valueText = inputFieldC.text.Trim();
        string formula = inputFieldSkyd.text;

        if (string.IsNullOrEmpty(valueText))
        {
            Debug.LogWarning("InputfieldC er tomt!");
            return;
        }

        if (string.IsNullOrEmpty(formula))
        {
            Debug.LogWarning("InputfieldSkyd er tomt!");
            return;
        }

        // Erstat 'c' med værdien fra inputFieldC, men beskyt 'cos'
        string temp = Regex.Replace(formula, @"\bcos\b", "COS_PLACEHOLDER");
        string updatedFormula = temp.Replace("c", valueText);
        updatedFormula = updatedFormula.Replace("COS_PLACEHOLDER", "cos");
        inputFieldSkyd.text = updatedFormula;

        Debug.Log("Formel opdateret: " + updatedFormula);
    }
    public void ReplaceDInFormula()
    {
        if (inputFieldD == null)
        {
            Debug.LogError("InputfieldD er ikke assigned!");
            return;
        }

        if (inputFieldSkyd == null)
        {
            Debug.LogError("InputfieldSkyd er ikke assigned!");
            return;
        }

        string valueText = inputFieldD.text.Trim();
        string formula = inputFieldSkyd.text;

        if (string.IsNullOrEmpty(valueText))
        {
            Debug.LogWarning("InputfieldD er tomt!");
            return;
        }

        if (string.IsNullOrEmpty(formula))
        {
            Debug.LogWarning("InputfieldSkyd er tomt!");
            return;
        }

        // Erstat 'd' med værdien fra inputFieldD
        string updatedFormula = formula.Replace("d", valueText);
        inputFieldSkyd.text = updatedFormula;

        Debug.Log("Formel opdateret: " + updatedFormula);
    }
}
