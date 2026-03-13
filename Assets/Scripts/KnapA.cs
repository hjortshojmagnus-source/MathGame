using UnityEngine;

public class KnapA : MonoBehaviour
{
    public Shoot shootScript;  // Reference to the Shoot script in Inspector

    // This method is called when the UI button is pressed
    public void ChangeA(float newValue)
    {
        if (shootScript == null || !shootScript.isReady)
        {
            Debug.LogError("Shoot script is not assigned in Inspector!");
            return;
        }

        shootScript.paramA = newValue;
        Debug.Log("Parameter A changed to: " + newValue);

        // Call ShowMenu() to update the console output
        shootScript.ShowMenu();
    }
}