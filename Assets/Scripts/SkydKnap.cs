using UnityEngine;

public class SkydKnap : MonoBehaviour
{
    private Shoot shootScript;

    public void SetPlayer(GameObject player)
    {
        if (player != null)
        {
            shootScript = player.GetComponent<Shoot>();
        }
        else
        {
            Debug.LogError("SetPlayer fik null!");
        }
    }

    public void skyd()
    {
        if (shootScript != null)
        {
            Debug.Log("=== SKYD KNAP TRYKKET ===");
            // Læs den aktuelle formel fra inputfeltet
            InputFieldVarReplace inputHandler = FindFirstObjectByType<InputFieldVarReplace>();
            if (inputHandler != null && inputHandler.inputFieldSkyd != null)
            {
                string formula = inputHandler.inputFieldSkyd.text.Trim();
                if (!string.IsNullOrEmpty(formula))
                {
                    Debug.Log($"📝 Opdaterer formel til: {formula}");
                    shootScript.SetGraf(formula);
                }
            }
            Debug.Log("🎯 Kalder FireBullet()...");
            shootScript.FireBullet();
        }
        else
        {
            Debug.LogError("❌ Shoot script not set!");
        }
    }
}