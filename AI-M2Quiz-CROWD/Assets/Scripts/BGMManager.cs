using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BGMManager : MonoBehaviour
{
    public AudioClip bgm1; // Music before 30s
    public AudioClip bgm2; // Music from 30s onward

    [Range(0f, 1f)] public float bgm1Volume = 1f;
    [Range(0f, 1f)] public float bgm2Volume = 1f;

    public Renderer[] objectsToChangeMaterial; // Assign renderers here
    public Material newMaterial; // Material to switch to for the array

    public Renderer specialObject; // Single special object to change
    public Material specialMaterial; // Special material to assign to specialObject

    public GameObject panelToActivate; // Assign panel GameObject here

    private AudioSource audioSource;
    private bool switchedToBGM2 = false;
    private bool materialAndPanelChanged = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;

        if (bgm1 != null)
        {
            audioSource.clip = bgm1;
            audioSource.volume = bgm1Volume;
            audioSource.Play();
        }

        // Make sure panel is inactive at start
        if (panelToActivate != null)
            panelToActivate.SetActive(false);
    }

    void Update()
    {
        // Update volume in case sliders changed at runtime
        if (!switchedToBGM2 && audioSource.clip == bgm1)
        {
            audioSource.volume = bgm1Volume;
        }
        else if (switchedToBGM2 && audioSource.clip == bgm2)
        {
            audioSource.volume = bgm2Volume;
        }

        if (!switchedToBGM2 && SceneTimerManager.SceneTime >= 30f)
        {
            SwitchToBGM2();
        }

        if (!materialAndPanelChanged && SceneTimerManager.SceneTime >= 30f)
        {
            ChangeMaterialsAndActivatePanel();
        }
    }

    private void SwitchToBGM2()
    {
        if (bgm2 != null)
        {
            audioSource.Stop();
            audioSource.clip = bgm2;
            audioSource.volume = bgm2Volume;
            audioSource.loop = true;
            audioSource.Play();
            switchedToBGM2 = true;
        }
    }

    private void ChangeMaterialsAndActivatePanel()
    {
        if (objectsToChangeMaterial != null && newMaterial != null)
        {
            foreach (var rend in objectsToChangeMaterial)
            {
                if (rend != null)
                    rend.material = newMaterial;
            }
        }

        if (specialObject != null && specialMaterial != null)
        {
            specialObject.material = specialMaterial;
        }

        if (panelToActivate != null)
        {
            panelToActivate.SetActive(true);
        }

        materialAndPanelChanged = true;
    }
}
