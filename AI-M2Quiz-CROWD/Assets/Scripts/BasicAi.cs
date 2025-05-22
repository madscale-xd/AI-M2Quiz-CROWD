using UnityEngine;

/*
Piang
Endaya
Almanzor
Tabuno
Flores
Onella
*/

public class BasicAi : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float sidestepSpeed = 3f;
    public float detectionDistance = 2f;
    public LayerMask obstacleLayer;

    private Rigidbody rb;
    private PlayerState currentState = PlayerState.MovingForward;

    enum PlayerState
    {
        MovingForward,
        MovingLeft,
        MovingRight
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        switch (currentState)
        {
            case PlayerState.MovingForward:
                MoveForward();
                if (IsObstacleInFront())
                {
                    DecideSidestepDirection();
                }
                break;

            case PlayerState.MovingLeft:
                rb.MovePosition(transform.position - transform.right * sidestepSpeed * Time.deltaTime);
                if (!IsObstacleInFront())
                {
                    currentState = PlayerState.MovingForward;
                }
                break;

            case PlayerState.MovingRight:
                rb.MovePosition(transform.position + transform.right * sidestepSpeed * Time.deltaTime);
                if (!IsObstacleInFront())
                {
                    currentState = PlayerState.MovingForward;
                }
                break;
        }
    }

    void MoveForward()
    {
        rb.MovePosition(transform.position + transform.forward * moveSpeed * Time.deltaTime);
    }

    bool IsObstacleInFront()
    {
        return Physics.Raycast(transform.position, transform.forward, detectionDistance, obstacleLayer);
    }

    void DecideSidestepDirection()
    {
        float distanceToLeft = CheckObstacleDistance(-transform.right);
        float distanceToRight = CheckObstacleDistance(transform.right);

        if (distanceToLeft > distanceToRight)
        {
            currentState = PlayerState.MovingLeft;
        }
        else
        {
            currentState = PlayerState.MovingRight;
        }
    }

    float CheckObstacleDistance(Vector3 direction)
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, direction, out hit, detectionDistance, obstacleLayer))
        {
            return hit.distance;
        }
        else
        {
            return float.MaxValue;
        }
    }
}
