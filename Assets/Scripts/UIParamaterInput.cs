using UnityEngine;

public class UIParameterInput : MonoBehaviour
{
    public int parameterIndex; // 0=a, 1=b, 2=c, 3=d

    public void UpdateParameter(string value)
    {
        Shoot shoot = FindAnyObjectByType<Shoot>();

        if (shoot != null)
        {
            shoot.SetParameterFromInput(value, parameterIndex);
        }
    }
}