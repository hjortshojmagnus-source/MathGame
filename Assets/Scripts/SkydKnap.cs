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
            Debug.Log("Skyd button pressed!");
            shootScript.FireBullet();
        }
        else
        {
            Debug.LogError("Shoot script not set!");
        }
    }
}