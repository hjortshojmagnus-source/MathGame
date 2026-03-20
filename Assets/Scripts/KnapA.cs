using UnityEngine;

public class KnapA : MonoBehaviour
{
    public Shoot shootScript;  // Reference to Shoot script

    // This method will be called when the button is pressed
    public void ChangeA(float newValue)
    {
        if (shootScript != null)
        {
            shootScript.paramA = newValue;
            UnityEngine.Debug.Log("Parameter a changed to: " + newValue);
            shootScript.ShowMenu(); // Optional: update menu display
        }
        else
        {
            UnityEngine.Debug.LogError("Shoot script is not assigned in Inspector!");
        }
    }
}