using UnityEngine;
using TMPro;

public class SkydKnap : MonoBehaviour
{
    private Shoot shootScript;

    public TMP_InputField inputField;

    public void OnClickFire()
    {
        Shoot.Instance.SetGraf(inputField.text);
    Shoot.Instance.FireBullet();
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