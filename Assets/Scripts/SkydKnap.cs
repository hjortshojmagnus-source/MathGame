using UnityEngine;

public class SkydKnap : MonoBehaviour
{
    public Shoot shootScript;  // Reference to Shoot script
    public void skyd()
    {
        UnityEngine.Debug.Log("Skyd button pressed!");
        shootScript.FireBullet();
        
    }
}
