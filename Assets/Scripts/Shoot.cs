using UnityEngine;
using System.Text.RegularExpressions;
using System.Collections.Generic;

[System.Serializable]
public class Formula
{
    public string name;
    public string expression;

    public Formula(string name, string expression)
    {
        this.name = name;
        this.expression = expression;
    }
}
public class Shoot : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject Bullet;
    public EnemyScript enemy;

    // Foruddefinerede formler
    private List<Formula> formulas = new List<Formula>
{
    new Formula("Linear", "a*x + b"),
    new Formula("Quadratic", "a*x*x + b*x + c"),
    new Formula("Cubic", "a*x*x*x + b*x*x + c*x + d"),
    new Formula("Sine", "a*sin(b*x + c) + d"),
    new Formula("Cosine", "a*cos(b*x+c)+d"),
    new Formula("Exponential", "a*exp(b*x)"),
    new Formula("Square Root", "a*sqrt(x) + b")
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
    playerPrefab = GameObject.Find("Player(Clone)");

    formulaNames = new string[formulas.Count];

    for (int i = 0; i < formulas.Count; i++)
    {
        formulaNames[i] = formulas[i].name;
    }

    currentFormula = formulas[0].expression;

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
    public void changeParameter(int paramIndex, float newValue)
    {
        switch (paramIndex)
        {
            case 0: paramA = newValue; break;
            case 1: paramB = newValue; break;
            case 2: paramC = newValue; break;
            case 3: paramD = newValue; break;
        }

    }

    public void ShowMenu()
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
        Debug.Log("=== VÆLG FORMEL ===");

        for (int i = 0; i < formulas.Count; i++)
        {
            Debug.Log($"Tryk '{i}' for {formulas[i].name}: {formulas[i].expression}");
        }
    }

    void HandleFormulaSelection()
    {
        foreach (char c in Input.inputString)
        {
            if (char.IsDigit(c))
            {
                int index = int.Parse(c.ToString());

                if (index >= 0 && index < formulas.Count)
                {
                    currentFormulaIndex = index;
                    currentFormula = formulas[index].expression;

                    paramA = 1f;
                    paramB = 0f;
                    paramC = 0f;
                    paramD = 0f;

                    Debug.Log($"Formel valgt: {formulas[index].name} = {currentFormula}");
                    formulaSelectionMode = false;
                    ShowMenu();
                }
                else
                {
                    Debug.LogError("Ugyldigt valg!");
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
        void HandleFormulaSelection()
        {
            foreach (char c in Input.inputString)
            {
                if (char.IsDigit(c))
                {
                    int index = int.Parse(c.ToString());

                    if (index >= 0 && index < formulas.Count)
                    {
                        currentFormulaIndex = index;
                        currentFormula = formulas[index].expression;

                        paramA = 1f;
                        paramB = 0f;
                        paramC = 0f;
                        paramD = 0f;

                        Debug.Log($"Formel valgt: {formulas[index].name} = {currentFormula}");
                        formulaSelectionMode = false;
                        ShowMenu();
                    }
                    else
                    {
                        Debug.LogError("Ugyldigt valg!");
                    }
                }
            }
        }
    }

    public void FireBullet()
{
    if (Bullet != null)
    {
        playerPrefab = GameObject.Find("Player(Clone)");

        Vector3 spawnPos = playerPrefab.transform.position;

        GameObject newBullet = Instantiate(Bullet, spawnPos, transform.rotation);

        Vector3[] path = GeneratePathFromFormula(spawnPos);

        BulletScript bulletScript = newBullet.GetComponent<BulletScript>();
        if (bulletScript != null)
        {
            bulletScript.waypoints = path;
        }
    }

    if (enemy != null)
        enemy.NextRound();
}

    Vector3[] GeneratePathFromFormula(Vector3 startPos)
    {
        Vector3[] path = new Vector3[pointCount];
        float step = (endX - startX) / (pointCount - 1);

        float minY = float.MaxValue;
        float maxY = float.MinValue;

        for (int i = 0; i < pointCount; i++)
        {
            float x = startX + (i * step);
            float y = EvaluateCurrentFormula(x);

            if (y < minY) minY = y;
            if (y > maxY) maxY = y;

            y = Mathf.Clamp(y, -100f, 100f);
            path[i] = new Vector3(startPos.x + x, startPos.y + y, startPos.z);
        }

        return path;
    }


    float EvaluateCurrentFormula(float x)
    {
        if (currentFormulaIndex < 0 || currentFormulaIndex >= formulas.Count)
        {
            Debug.LogError("Index fejl!");
            return 0f;
        }

        string name = formulas[currentFormulaIndex].name;
        float y = 0f;

        switch (name)
        {
            case "Linear":
                y = paramA * x + paramB;
                break;

            case "Quadratic":
                y = paramA * x * x + paramB * x + paramC;
                break;

            case "Cubic":
                y = paramA * x * x * x + paramB * x * x + paramC * x + paramD;
                break;

            case "Sine":
                y = paramA * Mathf.Sin(paramB * x + paramC) + paramD;
                break;

            case "Cosine":
                y = paramA * Mathf.Cos(paramB * x + paramC) + paramD;
                break;

            case "Exponential":
                y = paramA * Mathf.Exp(paramB * x);
                break;

            case "Square Root":
                y = x < 0f ? 0f : paramA * Mathf.Sqrt(x) + paramB;
                break;

            default:
                y = 0f;
                break;
        }

        return y;
    }

    string EvaluateMathFunctions(string expression)
    {
        // Håndter sin(), cos(), tan(), sqrt(), abs(), exp(), log()
        System.Globalization.CultureInfo invariant = System.Globalization.CultureInfo.InvariantCulture;
        // Regexmønster for at finde funktioner som sin(x), cos(x) osv.
        string pattern = @"(sin|cos|tan|sqrt|abs|exp|log)\s*\(";

        // Find matches og erstat dem iterativt indtil der ikke er flere
        while (true)
        {
            var matches = Regex.Matches(expression, pattern, RegexOptions.IgnoreCase);
            if (matches.Count == 0) break;

            // Process matches from end to start to preserve indices when replacing
            for (int mi = matches.Count - 1; mi >= 0; mi--)
            {
                var match = matches[mi];
                string funcName = match.Groups[1].Value.ToLower();
                int funcStart = match.Index;

                int parenStart = funcStart + match.Value.Length - 1; // position of '('
                int parenCount = 1;
                int parenEnd = parenStart + 1; // tæller frem til den matchende ')'

                while (parenEnd < expression.Length && parenCount > 0)
                {
                    if (expression[parenEnd] == '(') parenCount++;
                    else if (expression[parenEnd] == ')') parenCount--;
                    parenEnd++;
                }

                if (parenCount != 0) continue; // skip unbalanced

                // parenEnd is at position after the matching ')'
                string innerExpression = expression.Substring(parenStart + 1, parenEnd - parenStart - 2 + 1);
                string fullCall = expression.Substring(funcStart, parenEnd - funcStart);

                // Evaluér det indre udtryk først
                System.Data.DataTable dt = new System.Data.DataTable();
                try
                {
                    var innerResult = dt.Compute(innerExpression.Replace(',', '.'), null);

                    float value;
                    if (innerResult is double d)
                        value = (float)d;
                    else
                        value = float.Parse(innerResult.ToString(), System.Globalization.NumberStyles.Float);

                    float result = funcName switch
                    {
                        "sin" => Mathf.Sin(value),
                        "cos" => Mathf.Cos(value),
                        "tan" => Mathf.Tan(value),
                        "sqrt" => Mathf.Sqrt(value),
                        "abs" => Mathf.Abs(value),
                        "exp" => (float)System.Math.Exp(value),
                        "log" => (float)System.Math.Log(value),
                        _ => 0f
                    };

                    string rep = result.ToString(invariant);
                    expression = expression.Substring(0, funcStart) + rep + expression.Substring(parenEnd);
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"Fejl i {funcName}({innerExpression}): {ex.Message}");
                    // leave as-is
                }
            }
        }

        // Håndel pow() særskilt da det har to parametre
        pattern = @"pow\s*\(";
        while (true)
        {
            var matches = Regex.Matches(expression, pattern, RegexOptions.IgnoreCase);
            if (matches.Count == 0) break;

            for (int mi = matches.Count - 1; mi >= 0; mi--)
            {
                var match = matches[mi];
                int powStart = match.Index;

                int parenStart = powStart + match.Value.Length - 1; // Position af "("
                int parenCount = 1;
                int parenEnd = parenStart + 1;

                while (parenEnd < expression.Length && parenCount > 0)
                {
                    if (expression[parenEnd] == '(') parenCount++;
                    else if (expression[parenEnd] == ')') parenCount--;
                    parenEnd++;
                }

                if (parenCount != 0) continue;

                string allArgs = expression.Substring(parenStart + 1, parenEnd - parenStart - 2 + 1);
                string fullCall = expression.Substring(powStart, parenEnd - powStart);

                // Split by comma at top level
                int depth = 0;
                int commaPos = -1;
                for (int i = 0; i < allArgs.Length; i++)
                {
                    if (allArgs[i] == '(') depth++;
                    else if (allArgs[i] == ')') depth--;
                    else if (allArgs[i] == ',' && depth == 0)
                    {
                        commaPos = i;
                        break;
                    }
                }

                if (commaPos < 0) continue;

                string base1 = allArgs.Substring(0, commaPos);
                string exponent = allArgs.Substring(commaPos + 1);

                System.Data.DataTable dt = new System.Data.DataTable();
                try
                {
                    var baseResult = dt.Compute(base1.Replace(',', '.'), null);
                    var expResult = dt.Compute(exponent.Replace(',', '.'), null);

                    float baseValue = baseResult is double bdd ? (float)bdd : float.Parse(baseResult.ToString(), System.Globalization.NumberStyles.Float);
                    float expValue = expResult is double edd ? (float)edd : float.Parse(expResult.ToString(), System.Globalization.NumberStyles.Float);

                    float result = (float)System.Math.Pow(baseValue, expValue);
                    string rep = result.ToString(invariant);
                    expression = expression.Substring(0, powStart) + rep + expression.Substring(parenEnd);
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"Fejl i pow({base1}, {exponent}): {ex.Message}");
                    // leave as-is
                }
            }
        }

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
