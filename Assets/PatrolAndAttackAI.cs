using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PatrolAndAttackAI : MonoBehaviour
{
    [Header("移動")]
    private Vector3[] _patrolPoints;
    [SerializeField] private float moveSpeed = 3.0f;
    [SerializeField] private float turnSpeed = 10.0f;
    [SerializeField] private float arrivalDistance = 0.01f;

    private Animator animator;
    private int currentPointIndex = 0;
    private bool isAttacking = false;
    private float attackTimer = 0f;

    void Start()
    {
        animator = GetComponent<Animator>();
        float randomX = Random.Range(-5.0f,5.0f);
        float randomZ = Random.Range(-5.0f,5.0f);
        _patrolPoints =  new Vector3[2];
        _patrolPoints[0] = this.transform.position;
        _patrolPoints[1] = this.transform.position + new Vector3(randomX, transform.position.y, randomZ);
    }

    void Update()
    {
        if (isAttacking) return; 
        Patrol();
    }

    private void Patrol()
    {
        Vector3 targetPoint = _patrolPoints[currentPointIndex];
        
        Vector3 targetPos = new Vector3(targetPoint.x, transform.position.y, targetPoint.z);
        float distance = Vector3.Distance(transform.position, targetPos);

        if (distance > arrivalDistance)
        {
            Vector3 direction = (targetPos - transform.position).normalized;
            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * turnSpeed);
            }
            
            transform.Translate(Vector3.forward * (moveSpeed * Time.deltaTime));
        }
        else
        {
            StartCoroutine(AttackRoutine());
            isAttacking = true;
        }
    }

    private IEnumerator AttackRoutine()
    {
        animator.SetTrigger("Attack");
        
        yield return new WaitForSeconds(3.0f); 
        
        SwitchToNextPointImmediately();

        isAttacking = false;
    }

    private void SwitchToNextPointImmediately()
    {
        currentPointIndex = (currentPointIndex + 1) % _patrolPoints.Length;
        Vector3 nextTarget = _patrolPoints[currentPointIndex];
        Vector3 targetPos = new Vector3(nextTarget.x, transform.position.y, nextTarget.z);
        
        Vector3 direction = (targetPos - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}