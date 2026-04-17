using UnityEngine;

public class ToggleVisibility : MonoBehaviour
{
    public GameObject objectToToggle; // Træk objektet herhen i Inspector

    public void ToggleActive()
    {
        if (objectToToggle != null)
        {
            // Skifter mellem at være aktiv/inaktiv
            bool isActive = objectToToggle.activeSelf;
            objectToToggle.SetActive(!isActive);
        }
    }
}
