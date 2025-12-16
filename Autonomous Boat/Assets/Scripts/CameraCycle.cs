using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public Camera[] cameras;   // assign in Inspector
    private int currentIndex = 0;

    void Start()
    {
        // Safety check
        if (cameras == null || cameras.Length == 0)
        {
            Debug.LogWarning("CameraSwitcher: No cameras assigned!");
            return;
        }

        // Enable only the first camera at start
        for (int i = 0; i < cameras.Length; i++)
        {
            bool active = (i == 0);
            cameras[i].gameObject.SetActive(active);

            // If cameras have AudioListeners, enable only on active one
            AudioListener listener = cameras[i].GetComponent<AudioListener>();
            if (listener != null)
                listener.enabled = active;
        }
    }

    void Update()
    {
        if (cameras == null || cameras.Length == 0)
            return;

        // Press C to cycle cameras
        if (Input.GetKeyDown(KeyCode.C))
        {
            currentIndex++;
            if (currentIndex >= cameras.Length)
                currentIndex = 0;

            SwitchToCamera(currentIndex);
        }
    }

    void SwitchToCamera(int index)
    {
        for (int i = 0; i < cameras.Length; i++)
        {
            bool active = (i == index);
            cameras[i].gameObject.SetActive(active);

            AudioListener listener = cameras[i].GetComponent<AudioListener>();
            if (listener != null)
                listener.enabled = active;
        }

        Debug.Log("Switched to camera: " + cameras[index].name);
    }
}
