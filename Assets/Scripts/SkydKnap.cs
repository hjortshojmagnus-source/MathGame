using UnityEngine;

public class SkydKnap : MonoBehaviour
{
    private Shoot shootScript;

    public void OnClickFire()
    {
        if (shootScript != null)
        {
            shootScript.FireBullet();
        }
        else
        {
            Debug.LogError("Shoot script ikke sat på knappen!");
        }
    }

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
}