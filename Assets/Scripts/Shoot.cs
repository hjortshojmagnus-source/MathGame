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
    new Formula("Square Root", "a*sqrt(x) + b"),
    new Formula("BrugerInput", "g") // Denne formel bruger graf-parameteren direkte som y-værdi
};

    private string[] formulaNames;
    public string graf;
    private Formula currentFormula;
    public string userExpression = "g";

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
    private bool formulaSelectionMode = false;

    void Start()
    {
        playerPrefab = GameObject.Find("Player(Clone)");

        formulaNames = new string[formulas.Count];
        Debug.Log("GENERERER PATH MED GRAF: " + graf);

        for (int i = 0; i < formulas.Count; i++)
        {
            formulaNames[i] = formulas[i].name;
        }

        currentFormula = formulas[0]; // default

        ShowMenu();
    }

    public void SetGraf(string grafInput)
{
    graf = grafInput;
    ActivateUserGraph();

    Debug.Log("Ny graf sat: " + graf);
}
    public void changeParameter(int paramIndex, float newValue)
    {
        switch (paramIndex)
        {
            case 0: paramA = newValue; break;
            case 1: paramB = newValue; break;
            case 2: paramC = newValue; break;
            case 3: paramD = newValue; break;
            case 4: graf = newValue.ToString(); break;
        }

    }


    public void ShowMenu()
{
    Debug.Log("=== MATEMATIK SPIL ===");
    Debug.Log("Nuværende formel: " + currentFormula.name + " = " + currentFormula.expression);
}




    void StartInputMode(int paramIndex)
    {
        string paramName = new string[] { "a", "b", "c", "d" }[paramIndex];

        // Tjek om parameteren bruges i den valgte formel
        if (!currentFormula.expression.Contains(paramName))
        {
            Debug.LogError($"Parameteren '{paramName}' bruges ikke i denne formel!");
            return;
        }

        currentParameter = paramIndex;
        inputBuffer = "";

        float currentValue = GetParameterValue(paramIndex);
        Debug.Log($"Redigerer parameter {paramName} (nuværende værdi: {currentValue}). Skriv værdi og tryk Enter.");
    }


    public void FireBullet()
    {
        if (Bullet != null)
        {
            Debug.Log("SKYDER MED GRAF: " + graf);
            Debug.Log("SKUD MED GRAF: " + graf);
            Debug.Log("FORMULA INDEX: " + currentFormula);
            playerPrefab = GameObject.Find("Player(Clone)");

            Vector3 spawnPos = playerPrefab.transform.position;

            GameObject newBullet = Instantiate(Bullet, spawnPos, transform.rotation);

            Vector3[] path = GeneratePathFromFormula(spawnPos);
            Debug.Log("GENERERER PATH MED GRAF: " + graf);

            BulletScript bulletScript = newBullet.GetComponent<BulletScript>();
            if (bulletScript != null)
            {
                bulletScript.waypoints = path;
            }
        }

        if (enemy != null)
            enemy.NextRound();
    }
    public void RefreshFormula()
    {
        Debug.Log("FORMEL OPDATERET");
    }
    public void ActivateUserGraph()
{
    currentFormula = null;

    foreach (var f in formulas)
    {
        if (f.name == "BrugerInput")
        {
            currentFormula = f;
            break;
        }
    }

    if (currentFormula == null)
    {
        Debug.LogError("BrugerInput ikke fundet i formulas-listen!");
        return;
    }

    Debug.Log("Aktiv formel: " + currentFormula.name + " | expr: " + currentFormula.expression);
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
        if (currentFormula == null)
{
    Debug.LogError("Ingen formel valgt!");
    return 0f;
}

        string name = currentFormula.name;
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
            case "BrugerInput":

                string expr = currentFormula.expression;

                // 🔥 1. implicit multiplication (skal være først)
                expr = InsertImplicitMultiplication(expr);

                // 🔥 2. powers
                expr = ConvertPowerOperator(expr);

                // 🔥 3. variabler
                expr = expr.Replace("x", x.ToString(System.Globalization.CultureInfo.InvariantCulture));
                expr = expr.Replace("a", paramA.ToString(System.Globalization.CultureInfo.InvariantCulture));
                expr = expr.Replace("b", paramB.ToString(System.Globalization.CultureInfo.InvariantCulture));
                expr = expr.Replace("c", paramC.ToString(System.Globalization.CultureInfo.InvariantCulture));
                expr = expr.Replace("d", paramD.ToString(System.Globalization.CultureInfo.InvariantCulture));

                // 🔥 4. math functions
                expr = EvaluateMathFunctions(expr);

                try
                {
                    System.Data.DataTable dt = new System.Data.DataTable();
                    var result = dt.Compute(expr, null);

                    y = result is double dVal ? (float)dVal : float.Parse(result.ToString());
                }
                catch
                {
                    Debug.LogError("Fejl i bruger-input graf!");
                    y = 0f;
                }

                break;


            default:
                y = 0f;
                break;
        }

        return y;
    }
    string ConvertPowerOperator(string expr)
    {
        // finder a^b mønstre (enkle cases)
        System.Text.RegularExpressions.Regex regex =
            new System.Text.RegularExpressions.Regex(@"(\w+|\d+(\.\d+)?)\s*\^\s*(\w+|\d+(\.\d+)?)");

        while (regex.IsMatch(expr))
        {
            expr = regex.Replace(expr, "pow($1,$3)");
        }

        return expr;
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

    string InsertImplicitMultiplication(string expr)
    {
        expr = Regex.Replace(expr, @"sin", "sin");
        expr = Regex.Replace(expr, @"cos", "cos");
        expr = Regex.Replace(expr, @"tan", "tan");
        expr = Regex.Replace(expr, @"sqrt", "sqrt");
        expr = Regex.Replace(expr, @"log", "log");
        expr = Regex.Replace(expr, @"exp", "exp");
        // Fjern mellemrum først (gør parsing stabil)
        expr = expr.Replace(" ", "");

        // Regel 1: tal/variabel/")" efterfulgt af "("
        expr = Regex.Replace(expr, @"(\d|\)|[a-zA-Z])\(", "$1*(");

        // Regel 2: ")" efterfulgt af tal/variabel
        expr = Regex.Replace(expr, @"\)(\d|[a-zA-Z])", ")*$1");

        // Regel 3: tal efter variabel (x2 → x*2)
        expr = Regex.Replace(expr, @"([a-zA-Z])(\d)", "$1*$2");

        // Regel 4: variabel efter variabel (xy → x*y)
        expr = Regex.Replace(expr, @"([a-zA-Z])([a-zA-Z])", "$1*$2");

        return expr;
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
