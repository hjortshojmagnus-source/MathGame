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
    public float endX = 10f;       // Slut x-værdi - skal være større end startX
    public int pointCount = 100;   // Antal punkt på linjen - øget for glatere linje
    
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
        float step = (endX - startX) / (pointCount - 1);
        
        Vector3 bulletStartPos = transform.position;
        
        Debug.Log($"=== GENERERER STI ===");
        Debug.Log($"Formel: {currentFormula}");
        Debug.Log($"Parametre: a={paramA}, b={paramB}, c={paramC}, d={paramD}");
        Debug.Log($"X-område: {startX} til {endX}, {pointCount} punkter, step={step}");
        Debug.Log($"Kugle starter på: ({bulletStartPos.x:F2}, {bulletStartPos.y:F2})");
        
        for (int i = 0; i < pointCount; i++)
        {
            float x = startX + (i * step);
            float y = EvaluateFormula(currentFormula, x);
            
            path[i] = new Vector3(bulletStartPos.x + x, bulletStartPos.y + y, 0);
            
            // Debug log for første 5 og sidste 5 punkter
            if (i < 5 || i >= pointCount - 5)
            {
                Debug.Log($"Punkt {i}: x={x:F2}, y={y:F2} (Formel med a={paramA}, b={paramB})");
            }
        }
        
        return path;
    }
    
    float EvaluateFormula(string formula, float x)
    {
        try
        {
            // Erstat variabler med deres værdier
            string expression = formula;
            
            // Brug invariant culture for at sikre . som decimalseparator
            System.Globalization.CultureInfo invariant = System.Globalization.CultureInfo.InvariantCulture;
            
            // Erstat variabler først - omgiv med parenteser for sikkerhed
            expression = Regex.Replace(expression, @"\bx\b", $"({x.ToString(invariant)})");
            expression = Regex.Replace(expression, @"\ba\b", $"({paramA.ToString(invariant)})");
            expression = Regex.Replace(expression, @"\bb\b", $"({paramB.ToString(invariant)})");
            expression = Regex.Replace(expression, @"\bc\b", $"({paramC.ToString(invariant)})");
            expression = Regex.Replace(expression, @"\bd\b", $"({paramD.ToString(invariant)})");
            
            // Evaluér matematiske funktioner
            expression = EvaluateMathFunctions(expression);
            
            // Brug DataTable til at evaluere det endelige udtryk
            System.Data.DataTable dt = new System.Data.DataTable();
            var result = dt.Compute(expression, null);
            
            float yValue = float.Parse(result.ToString(), invariant);
            
            return yValue;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Fejl i matematisk formel: {formula} - {ex.Message}");
            return 0f;
        }
    }
    
    string EvaluateMathFunctions(string expression)
    {
        // Håndter sin(), cos(), tan(), sqrt(), abs(), exp(), log()
        System.Globalization.CultureInfo invariant = System.Globalization.CultureInfo.InvariantCulture;
        
        // Regexmønster for at finde funktioner som sin(x), cos(x) osv.
        string pattern = @"(sin|cos|tan|sqrt|abs|exp|log|pow)\s*\(\s*([^()]+)\s*\)";
        
        while (Regex.IsMatch(expression, pattern, RegexOptions.IgnoreCase))
        {
            expression = Regex.Replace(expression, pattern, match =>
            {
                string funcName = match.Groups[1].Value.ToLower();
                string innerExpression = match.Groups[2].Value;
                
                // Evaluér det indre udtryk først
                System.Data.DataTable dt = new System.Data.DataTable();
                try
                {
                    var innerResult = dt.Compute(innerExpression, null);
                    float value = float.Parse(innerResult.ToString(), invariant);
                    
                    float result = funcName switch
                    {
                        "sin" => (float)System.Math.Sin(value),
                        "cos" => (float)System.Math.Cos(value),
                        "tan" => (float)System.Math.Tan(value),
                        "sqrt" => (float)System.Math.Sqrt(value),
                        "abs" => System.Math.Abs(value),
                        "exp" => (float)System.Math.Exp(value),
                        "log" => (float)System.Math.Log(value),
                        _ => 0f
                    };
                    
                    return result.ToString(invariant);
                }
                catch
                {
                    return match.Value; // Returner original hvis det fejler
                }
            }, RegexOptions.IgnoreCase);
        }
        
        // Håndel pow() særskilt da det har to parametre
        pattern = @"pow\s*\(\s*([^,]+)\s*,\s*([^)]+)\s*\)";
        expression = Regex.Replace(expression, pattern, match =>
        {
            string base1 = match.Groups[1].Value;
            string exponent = match.Groups[2].Value;
            
            System.Data.DataTable dt = new System.Data.DataTable();
            try
            {
                var baseResult = dt.Compute(base1, null);
                var expResult = dt.Compute(exponent, null);
                
                float baseValue = float.Parse(baseResult.ToString(), invariant);
                float expValue = float.Parse(expResult.ToString(), invariant);
                
                float result = (float)System.Math.Pow(baseValue, expValue);
                return result.ToString(invariant);
            }
            catch
            {
                return match.Value;
            }
        }, RegexOptions.IgnoreCase);
        
        return expression;
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
