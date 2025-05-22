using UnityEngine;

public class SceneTimerManager : MonoBehaviour
{
    public static float SceneTime { get; private set; }

    void Update()
    {
        SceneTime += Time.deltaTime;
    }

    public static void ResetTimer()
    {
        SceneTime = 0f;
    }
}
