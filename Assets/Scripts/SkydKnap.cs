using System.Transactions;
using UnityEngine;

public class SkydKnap : MonoBehaviour
{
    public Shoot shootScript;  // Reference to Shoot script
    public Transform playerTransform;  // Reference to player's transform

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }
    public void skyd()
    {
        UnityEngine.Debug.Log("Skyd button pressed!");
        shootScript.FireBullet();

    }
}
