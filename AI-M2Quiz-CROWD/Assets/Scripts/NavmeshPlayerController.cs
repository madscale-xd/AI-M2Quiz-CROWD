using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class NavmeshPlayerController : MonoBehaviour
{
    [SerializeField]
    private NavMeshAgent agent;

    private bool isPaused = false;
    private Coroutine pauseCoroutine; // track the current pause coroutine

    private void Update()
    {
        if (isPaused)
            return;

        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                agent.SetDestination(hit.point);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            if (pauseCoroutine != null)
            {
                StopCoroutine(pauseCoroutine);  // cancel current pause coroutine
            }

            isPaused = true;
            agent.isStopped = true;
            Debug.Log("NavMesh Player hit by bullet! Movement paused.");

            // Restart pause coroutine (reset timer)
            pauseCoroutine = StartCoroutine(ResumeAfterDelay(1f));
        }
    }

    private IEnumerator ResumeAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        isPaused = false;
        agent.isStopped = false;
        pauseCoroutine = null;
        Debug.Log("NavMesh Player movement resumed.");
    }
}
