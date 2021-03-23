using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Movement : MonoBehaviour
{
    [Header("State")]
    [SerializeField] float actualMovementSpeed = 2f;
    [SerializeField] [Range(-1,1)]float modifier = 0f;

    [Header("Configure")]
    [SerializeField] float movementSpeed = 2f;

    [Header("Reference")]
    [SerializeField] internal NavMeshAgent nvm = null;
    [SerializeField] internal Animator animator = null;



    public bool currentlyMoving = false;
    public bool waiting = false;
    Vector3 currentDestination = new Vector3();

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
            if (Vector3.Distance(transform.position, currentDestination) <= 3f)
            {
                animator.SetFloat("speed", 0f);
                currentlyMoving = false;
            }
        }
    }

    public void MoveRelativeToTransform(float h, float v, Transform relativeTransform)
    {
        if (relativeTransform != null)
        {
            Vector3 forward = relativeTransform.forward;
            Vector3 right = relativeTransform.right;

            // zero out y-axis
            forward.y = 0f;
            right.y = 0f;

            Vector3 relativeDireciton = forward.normalized * v + right.normalized * h;
            // Adjust normalized directional vectors by the magnitude of input
            MoveInDirection(relativeDireciton);
        }
    }

    public void MoveInDirection(Vector3 direction)
    {
        if (nvm != null && nvm.enabled)
        {
            nvm.speed = actualMovementSpeed;
            UpdateAnimatorSpeed();
            nvm.Move(direction.normalized * actualMovementSpeed * Time.deltaTime);
        }
    }

    public void MoveToDestination(Vector3 destination)
    {
        if (nvm != null && nvm.enabled)
        {
            nvm.speed = actualMovementSpeed;
            //nvm.isStopped = false;

            UpdateAnimatorSpeed();

            currentDestination = destination;
            currentlyMoving = true;

            nvm.SetDestination(destination);
        }
    }

    public void UpdateAnimatorSpeed()
    {
        if(isGrounded)
        {
            actualMovementSpeed = movementSpeed * (1f + modifier);
            animator.SetFloat("speed", actualMovementSpeed);
        }
        else
        {
            animator.SetFloat("speed", 0f);
        }
    }


    public void MoveToDestination(Transform destination)
    {
        MoveToDestination(destination.position);
    }

    public void MoveToDestination(GameObject destination)
    {
        MoveToDestination(destination.transform.position);
    }

    public void CancelMovement()
    {
        nvm.ResetPath();
    }


    public bool isGrounded = true;
    [SerializeField] Rigidbody rb = null;
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
