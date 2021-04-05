using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Movement : MonoBehaviour
{
    [Header("State")]
    public bool isGrounded = true;
    public bool currentlyMoving = false;
    public bool waiting = false;
    Vector3? currentDestination = new Vector3?();

    [Header("Configure")]
    [SerializeField] float movementSpeed = 2f;

    [Header("Reference")]
    //[SerializeField] Rigidbody rb = null;
    [SerializeField] internal NavMeshAgent nvm = null;
    [SerializeField] internal Animator animator = null;
    
    private void Awake()
    {
        if(nvm == null)
        {
            nvm = GetComponent<NavMeshAgent>();
        }
        if(nvm == null)
        {
            nvm = gameObject.AddComponent<NavMeshAgent>();
        }
    }

    private void OnEnable()
    {
        if (nvm == null)
        {
            nvm = GetComponent<NavMeshAgent>();
        }
        if (nvm != null)
        {
            nvm.enabled = true;
        }
    }

    private void OnDisable()
    {
        if (nvm != null)
        {
            nvm.enabled = false;
        }
    }

    private void Update()
    {
        if (currentlyMoving)
        {
            if(currentDestination != null)
            {
                if (Vector3.Distance(transform.position, (Vector3)currentDestination) <= 3f)
                {
                    animator.SetFloat("speed", 0f);
                    currentlyMoving = false;
                }
            }
        }
    }

    public void MoveRelativeToTransform(float h, float v, Transform relativeTransform)
    {
        if (relativeTransform != null)
        {

            //Vector3 forward = relativeTransform.forward;
            //Vector3 right = relativeTransform.right;

            //// zero out y-axis
            //forward.y = 0f;
            //right.y = 0f;

            //// Adjust normalized directional vectors by the magnitude of input
            //Vector3 relativeDirection = forward.normalized * v + right.normalized * h;

            Vector3 relativeDirection = relativeTransform.GetRelativeDirectionWithMagnitude(h, v);

            MoveInDirection(relativeDirection);
        }
    }

    public void MoveInDirection(Vector3 direction, bool normalize = true)
    {
        if(!nvm.enabled)
        {
            nvm.enabled = true;
        }

        if (nvm != null)
        {
            nvm.speed = movementSpeed;
            UpdateAnimatorSpeed();
            nvm.Move((normalize ? direction.normalized : direction) * movementSpeed * Time.deltaTime);
        }
    }

    public void MoveToDestination(Vector3 destination, bool automate = true)
    {
        if (nvm != null && nvm.enabled)
        {
            nvm.speed = movementSpeed;
            //nvm.isStopped = false;

            UpdateAnimatorSpeed();

            if(automate)
            {
                currentDestination = destination;
                currentlyMoving = true;
            }

            nvm.SetDestination(destination);
        }
    }

    public void UpdateAnimatorSpeed()
    {
        if(isGrounded)
        {
            animator.SetFloat("speed", movementSpeed);
        }
        else
        {
            animator.SetFloat("speed", 0f);
        }
    }

    /// <summary>
    /// Set NavMesh destination to position of Transform destination
    /// </summary>
    /// <param name="destination"></param>
    public void MoveToDestination(Transform destination)
    {
        MoveToDestination(destination.position);
    }

    /// <summary>
    /// Set NavMesh destination to position of GameObject destination
    /// </summary>
    /// <param name="destination"></param>
    public void MoveToDestination(GameObject destination)
    {
        MoveToDestination(destination.transform.position);
    }

    public void CancelMovement()
    {
        nvm.ResetPath();
    }



    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Terrain" || collision.gameObject.isStatic)
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        //Debug.Log(collision.gameObject);
        //if (collision.collider.GetType() == typeof(TerrainCollider))
        if (collision.gameObject.tag == "Terrain" || collision.gameObject.isStatic)
        {
            //Debug.Log(collision.gameObject.name);
            isGrounded = false;
        }
    }

}
