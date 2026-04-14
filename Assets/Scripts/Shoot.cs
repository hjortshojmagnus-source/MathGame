using UnityEngine;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Globalization;
using System;

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

            y = Mathf.Clamp(y, -10f, 10f);
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

    string expr = graf;

    y = EvaluateExpression(expr, x);

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
        string pattern = @"(sin|cos|tan|sqrt|abs|exp|log)\(";

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

                // Evaluér det indre udtryk først ved at kalde SimpleEval
                try
                {
                    float value = SimpleEval(innerExpression);
                    float result = 0f;

                    if (float.IsNaN(value) || float.IsInfinity(value))
                    {
                        Debug.LogError($"Ugyldig værdi til {funcName}: {value}");
                        result = 0f;
                    }
                    else
                    {
                        result = funcName switch
                        {
                            "sin" => Mathf.Sin(value),
                            "cos" => Mathf.Cos(value),
                            "tan" => Mathf.Tan(value),
                            "sqrt" => value < 0f ? 0f : Mathf.Sqrt(value),
                            "log" => value <= 0f ? 0f : Mathf.Log(value),
                            "exp" => Mathf.Exp(value),
                            "abs" => Mathf.Abs(value),
                            _ => 0f
                        };

                        if (float.IsNaN(result) || float.IsInfinity(result))
                        {
                            Debug.LogError($"{funcName}({value}) gav ugyldig resultat: {result}");
                            result = 0f;
                        }
                    }

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
        pattern = @"pow\(";
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

                try
                {
                    float baseValue = SimpleEval(base1);
                    float expValue = SimpleEval(exponent);
                    float result = 0f;

                    if (float.IsNaN(baseValue) || float.IsInfinity(baseValue) || float.IsNaN(expValue) || float.IsInfinity(expValue))
                    {
                        Debug.LogError($"Ugyldige værdier i pow: base={baseValue}, exp={expValue}");
                        result = 0f;
                    }
                    else
                    {
                        result = (float)System.Math.Pow(baseValue, expValue);
                        if (float.IsNaN(result) || float.IsInfinity(result))
                        {
                            Debug.LogError($"pow gav ugyldig resultat: pow({baseValue},{expValue}) = {result}");
                            result = 0f;
                        }
                    }

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
    expr = expr.Replace(" ", "");

    expr = expr.Replace("sin", "@SIN@");
    expr = expr.Replace("cos", "@COS@");
    expr = expr.Replace("tan", "@TAN@");
    expr = expr.Replace("sqrt", "@SQRT@");
    expr = expr.Replace("log", "@LOG@");
    expr = expr.Replace("exp", "@EXP@");

    expr = Regex.Replace(expr, @"(\d|\))\(", "$1*(");   
    expr = Regex.Replace(expr, @"\)(\d|[a-zA-Z])", ")*$1");
    expr = Regex.Replace(expr, @"([a-zA-Z])(\d)", "$1*$2");

    expr = expr.Replace("@SIN@", "sin");
    expr = expr.Replace("@COS@", "cos");
    expr = expr.Replace("@TAN@", "tan");
    expr = expr.Replace("@SQRT@", "sqrt");
    expr = expr.Replace("@LOG@", "log");
    expr = expr.Replace("@EXP@", "exp");

    return expr;
}

string ConvertToUnityMath(string expr)
{
    expr = expr.Replace(" ", "");

    expr = Regex.Replace(expr, @"(\w+|\d+)\^(\w+|\d+)", "Math.Pow($1,$2)");

    expr = expr.Replace("sin", "Sin");
    expr = expr.Replace("cos", "Cos");
    expr = expr.Replace("tan", "Tan");
    expr = expr.Replace("sqrt", "Sqrt");

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
    string HandleFunctions(string expr)
{
    expr = Regex.Replace(expr, @"sin\(", "Sin(");
    expr = Regex.Replace(expr, @"cos\(", "Cos(");
    expr = Regex.Replace(expr, @"tan\(", "Tan(");
    expr = Regex.Replace(expr, @"sqrt\(", "Sqrt(");

    expr = expr.Replace("Sin", "Sin");
expr = expr.Replace("Cos", "Cos");
expr = expr.Replace("Tan", "Tan");
expr = expr.Replace("Sqrt", "Sqrt");

    return expr;
}
    float EvaluateExpression(string expr, float xValue)
    {
        expr = expr.Replace(" ", "").ToLowerInvariant();

// STEP 1: implicit multiplication
expr = Regex.Replace(expr, @"(\d)\s*\(", "$1*(");
expr = Regex.Replace(expr, @"\)\s*\(", ")*(");
expr = Regex.Replace(expr, @"\)\s*(\d)", ")*$1");
expr = Regex.Replace(expr, @"(\d)\s*(sin|cos|tan|sqrt|exp|log|abs)", "$1*$2");
expr = Regex.Replace(expr, @"(sin|cos|tan|sqrt|exp|log|abs)\s*\(", "$1(");
expr = Regex.Replace(expr, @"(\d)\s*x", "$1*x");
expr = Regex.Replace(expr, @"x\s*(\d)", "x*$1");

// STEP 2: safe x replace
expr = Regex.Replace(
    expr,
    @"(?<![a-zA-Z0-9_])x(?![a-zA-Z0-9_])",
    xValue.ToString(CultureInfo.InvariantCulture)
);
Debug.Log("AFTER x replace: " + expr);

    expr = Regex.Replace(expr, @"(\d+(\.\d+)?|\))\s*\^\s*(\d+(\.\d+)?|\()", "pow($1,$3)");

    expr = EvaluateMathFunctions(expr);
Debug.Log("AFTER math functions: " + expr);
if (Regex.IsMatch(expr, @"[a-zA-Z]"))
{
    Debug.LogError("Uforløste tokens i expr: " + expr);
    return 0f;
}

Debug.Log($"xValue = {xValue} | expr = {expr}");
    return SimpleEval(expr);
}
float SimpleEval(string expr)
{
    try
    {
        // Brug en simpel parser i stedet for DataTable
        return ParseAndEvaluate(expr);
    }
    catch (Exception e)
    {
        Debug.LogError("Eval fejl: " + expr + " | " + e.Message);
        return 0f;
    }
}

float ParseAndEvaluate(string expr)
{
    // Simpel rekursiv descent parser for grundlæggende matematik
    expr = expr.Replace(" ", "").Replace(",", ".");
    return EvaluateExpressionRecursive(expr, 0, out _);
}

float EvaluateExpressionRecursive(string expr, int start, out int end)
{
    float result = EvaluateTerm(expr, start, out end);

    while (end < expr.Length && (expr[end] == '+' || expr[end] == '-'))
    {
        char op = expr[end];
        int nextStart = end + 1;
        float nextTerm = EvaluateTerm(expr, nextStart, out end);
        if (op == '+') result += nextTerm;
        else result -= nextTerm;
    }

    return result;
}

float EvaluateTerm(string expr, int start, out int end)
{
    float result = EvaluateFactor(expr, start, out end);

    while (end < expr.Length && (expr[end] == '*' || expr[end] == '/'))
    {
        char op = expr[end];
        int nextStart = end + 1;
        float nextFactor = EvaluateFactor(expr, nextStart, out end);
        if (op == '*') result *= nextFactor;
        else if (nextFactor != 0) result /= nextFactor;
        else throw new DivideByZeroException();
    }

    return result;
}

float EvaluateFactor(string expr, int start, out int end)
{
    end = start;

    // Håndter unære operatorer (+ og -)
    if (expr[start] == '-' || expr[start] == '+')
    {
        char op = expr[start];
        float factor = EvaluateFactor(expr, start + 1, out end);
        return op == '-' ? -factor : factor;
    }

    // Håndter parenteser
    if (expr[start] == '(')
    {
        int parenEnd;
        float inner = EvaluateExpressionRecursive(expr, start + 1, out parenEnd);
        if (parenEnd < expr.Length && expr[parenEnd] == ')')
        {
            end = parenEnd + 1;
            return inner;
        }
        throw new Exception("Ubalancerede parenteser");
    }

    // Håndter tal
    if (char.IsDigit(expr[start]) || expr[start] == '.')
    {
        int numEnd = start;
        while (numEnd < expr.Length && (char.IsDigit(expr[numEnd]) || expr[numEnd] == '.'))
            numEnd++;
        string numStr = expr.Substring(start, numEnd - start);
        if (float.TryParse(numStr, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float num))
        {
            end = numEnd;
            return num;
        }
        throw new Exception("Ugyldigt tal: " + numStr);
    }

    throw new Exception("Ugyldigt udtryk ved position " + start);
}

}
