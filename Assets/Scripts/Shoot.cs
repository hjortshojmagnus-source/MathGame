using UnityEngine;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public class Shoot : MonoBehaviour
{
    public GameObject Bullet;
    
    // Foruddefinerede formler
    private Dictionary<string, string> formulas = new Dictionary<string, string>
    {
        { "Linear", "a*x + b" },
        { "Quadratic", "a*x*x + b*x + c" },
        { "Cubic", "a*x*x*x + b*x*x + c*x + d" },
        { "Sine", "a*sin(b*x)" },
        { "Cosine", "a*cos(b*x)" },
        { "Exponential", "a*exp(b*x)" },
        { "Square Root", "a*sqrt(x) + b" }
    };
    
    private string[] formulaNames;
    private int currentFormulaIndex = 0;
    private string currentFormula = "a*x + b";
    
    public float startX = 0f;      // Start x-værdi
    public float endX = 10f;       // Slut x-værdi
    public int pointCount = 50;    // Antal punkt på linjen
    
    // Parameterværdier som spilleren kan ændre
    public float paramA = 1f;      // Parameter a
    public float paramB = 0f;      // Parameter b
    public float paramC = 0f;      // Parameter c
    public float paramD = 0f;      // Parameter d
    
    private string inputBuffer = "";
    private int currentParameter = 0; // 0=a, 1=b, 2=c, 3=d
    private bool inputMode = false;
    private bool formulaSelectionMode = false;

    void Start()
    {
        formulaNames = new string[formulas.Count];
        formulas.Keys.CopyTo(formulaNames, 0);
        currentFormula = formulas[formulaNames[0]];
        
        ShowMenu();
    }

    void Update()
    {
        if (formulaSelectionMode)
        {
            HandleFormulaSelection();
        }
        else if (inputMode)
        {
            HandleParameterInput();
        }
        else
        {
            // Håndter parameterinput
            if (Input.GetKeyDown(KeyCode.Alpha1)) StartInputMode(0);
            if (Input.GetKeyDown(KeyCode.Alpha2)) StartInputMode(1);
            if (Input.GetKeyDown(KeyCode.Alpha3)) StartInputMode(2);
            if (Input.GetKeyDown(KeyCode.Alpha4)) StartInputMode(3);
            
            // Vælg formel
            if (Input.GetKeyDown(KeyCode.F)) SelectFormulaMode();
            
            // Skyd
            if (Input.GetKeyDown(KeyCode.K))
            {
                FireBullet();
            }
        }
    }
    
    void ShowMenu()
    {
        Debug.Log("=== MATEMATIK SPIL ===");
        Debug.Log("Nuværende formel: " + formulaNames[currentFormulaIndex] + " = " + currentFormula);
        Debug.Log("\nDu kan redigere disse parametre:");
        
        // Vis kun de parametre der bruges i formlen
        if (currentFormula.Contains("a")) Debug.Log("Tryk '1' for at ændre parameter a");
        if (currentFormula.Contains("b")) Debug.Log("Tryk '2' for at ændre parameter b");
        if (currentFormula.Contains("c")) Debug.Log("Tryk '3' for at ændre parameter c");
        if (currentFormula.Contains("d")) Debug.Log("Tryk '4' for at ændre parameter d");
        
        Debug.Log("\nTryk 'F' for at vælge anden formel");
        Debug.Log("Tryk 'K' for at skyde");
    }
    
    void SelectFormulaMode()
    {
        formulaSelectionMode = true;
        Debug.Log("\n=== VÆLG FORMEL ===");
        for (int i = 0; i < formulaNames.Length; i++)
        {
            Debug.Log($"Tryk '{i}' for {formulaNames[i]}: {formulas[formulaNames[i]]}");
        }
    }
    
    void HandleFormulaSelection()
    {
        foreach (char c in Input.inputString)
        {
            if (char.IsDigit(c))
            {
                int index = int.Parse(c.ToString());
                if (index < formulaNames.Length)
                {
                    currentFormulaIndex = index;
                    currentFormula = formulas[formulaNames[index]];
                    Debug.Log($"Formel valgt: {formulaNames[index]} = {currentFormula}");
                    formulaSelectionMode = false;
                    ShowMenu();
                }
                else
                {
                    Debug.LogError("Ugyldig valg!");
                }
            }
        }
    }
    
    void StartInputMode(int paramIndex)
    {
        string paramName = new string[] { "a", "b", "c", "d" }[paramIndex];
        
        // Tjek om parameteren bruges i den valgte formel
        if (!currentFormula.Contains(paramName))
        {
            Debug.LogError($"Parameteren '{paramName}' bruges ikke i denne formel!");
            return;
        }
        
        inputMode = true;
        currentParameter = paramIndex;
        inputBuffer = "";
        
        float currentValue = GetParameterValue(paramIndex);
        Debug.Log($"Redigerer parameter {paramName} (nuværende værdi: {currentValue}). Skriv værdi og tryk Enter.");
    }
    
    void HandleParameterInput()
    {
        // Håndter tastetryk for numerisk input
        foreach (char c in Input.inputString)
        {
            if (c == '\b') // Backspace
            {
                if (inputBuffer.Length > 0)
                    inputBuffer = inputBuffer.Substring(0, inputBuffer.Length - 1);
            }
            else if (c == '\n' || c == '\r') // Enter
            {
                if (float.TryParse(inputBuffer, out float value))
                {
                    SetParameterValue(currentParameter, value);
                    string paramName = new string[] { "a", "b", "c", "d" }[currentParameter];
                    Debug.Log($"Parameter {paramName} sat til {value}");
                }
                else
                {
                    Debug.LogError("Ugyldig værdi. Prøv igen.");
                }
                inputMode = false;
            }
            else if (char.IsDigit(c) || c == '-' || c == '.')
            {
                inputBuffer += c;
            }
        }
    }
    
    void FireBullet()
    {
        Debug.Log("Shoot");
        if (Bullet != null)
        {
            GameObject newBullet = Instantiate(Bullet, transform.position, transform.rotation);
            
            // Generer waypoints fra matematisk forskrift
            Vector3[] path = GeneratePathFromFormula();
            
            // Gi kulen stien
            BulletScript bulletScript = newBullet.GetComponent<BulletScript>();
            if (bulletScript != null)
            {
                bulletScript.waypoints = path;
            }
        }
    }
    
    Vector3[] GeneratePathFromFormula()
    {
        Vector3[] path = new Vector3[pointCount];
        float step = (endX - startX) / pointCount;
        
        for (int i = 0; i < pointCount; i++)
        {
            float x = startX + (i * step);
            float y = EvaluateFormula(currentFormula, x);
            path[i] = new Vector3(x, y, 0);
        }
        
        return path;
    }
    
    float EvaluateFormula(string formula, float x)
    {
        try
        {
            // Erstat variabler med deres værdier ved hjælp af regex
            // Dette sikrer at vi kun erstatter hele ord, ikke dele af funktionsnavne
            string expression = formula;
            
            // Erstat trigonometriske og andre funktioner først
            // Dette skal gøres før variablerne erstattes
            expression = Regex.Replace(expression, @"\bsin\b", "Math.Sin", RegexOptions.IgnoreCase);
            expression = Regex.Replace(expression, @"\bcos\b", "Math.Cos", RegexOptions.IgnoreCase);
            expression = Regex.Replace(expression, @"\btan\b", "Math.Tan", RegexOptions.IgnoreCase);
            expression = Regex.Replace(expression, @"\bsqrt\b", "Math.Sqrt", RegexOptions.IgnoreCase);
            expression = Regex.Replace(expression, @"\babs\b", "Math.Abs", RegexOptions.IgnoreCase);
            expression = Regex.Replace(expression, @"\bexp\b", "Math.Exp", RegexOptions.IgnoreCase);
            expression = Regex.Replace(expression, @"\blog\b", "Math.Log", RegexOptions.IgnoreCase);
            expression = Regex.Replace(expression, @"\bpow\b", "Math.Pow", RegexOptions.IgnoreCase);
            
            // Erstat variabler med parenteser omkring værdier for sikkerhed
            expression = Regex.Replace(expression, @"\bx\b", $"({x})");
            expression = Regex.Replace(expression, @"\ba\b", $"({paramA})");
            expression = Regex.Replace(expression, @"\bb\b", $"({paramB})");
            expression = Regex.Replace(expression, @"\bc\b", $"({paramC})");
            expression = Regex.Replace(expression, @"\bd\b", $"({paramD})");
            
            // Brug DataTable til at evaluere matematisk udtryk
            System.Data.DataTable dt = new System.Data.DataTable();
            var result = dt.Compute(expression, null);
            
            return float.Parse(result.ToString());
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Fejl i matematisk formel: {formula} - {ex.Message}");
            return 0f;
        }
    }
    
    float GetParameterValue(int paramIndex)
    {
        return paramIndex switch
        {
            0 => paramA,
            1 => paramB,
            2 => paramC,
            3 => paramD,
            _ => 0f
        };
    }
    
    void SetParameterValue(int paramIndex, float value)
    {
        switch (paramIndex)
        {
            case 0: paramA = value; break;
            case 1: paramB = value; break;
            case 2: paramC = value; break;
            case 3: paramD = value; break;
        }
    }
}
