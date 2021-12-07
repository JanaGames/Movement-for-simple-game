using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public List<Transform> walkPoints;
    
    [HideInInspector]
    public List<Transform> innerWalkPoints;
    Rigidbody rigi;
    public int currentPoint;
    public int currentInnerPoint;
    public bool readyWalk;
    public bool readyAttack;
    bool catched;
    bool readyDestroy;
    public int newPoint;
    public int newInnerPoint;
    public float speed;
    public float timeAttack;
    private IEnumerator coroutine;
    Animator animator;
    void Start()
    {
        rigi = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!readyDestroy) 
        {
            if (!readyAttack) 
               {
                  if (currentPoint != newPoint) Movement(newPoint);
                  if (currentPoint == newPoint && currentInnerPoint != newInnerPoint) MovementInner(newInnerPoint); 
                  //no move;
                  if (currentPoint == newPoint && currentInnerPoint == newInnerPoint) Actions();
               }
            if (GetComponent<FieldOfView>().CheckTargets()) Attack();
            if (!GetComponent<FieldOfView>().CheckTargets()) readyAttack = false;
        }
        if (readyDestroy) 
        {
            if(!catched) 
            {
                GetComponent<Projectile>().Jump();
                animator.SetTrigger("Jump");
            }
        }
    }
    void Actions() 
    {
        //start any animation or readyWalk;
        //set anim[i].length to coroutine;
        if (Random.Range(0, 2) == 1 && !readyWalk) 
        {
            readyWalk = false;
            string currentAnimation = "SitFoot";
            animator.Play(currentAnimation);
            coroutine = SetParentIndex(Random.Range(3, 6));
            StartCoroutine(coroutine);
        }
        else if (!readyWalk)
        {
            coroutine = SetParentIndex(0.2f);
            StartCoroutine(coroutine);
        }
    }
    void Attack() 
    {
        readyAttack = true;
        coroutine = AttackPause(timeAttack);
        StartCoroutine(coroutine);
    }
    public void MoveTo(int indexPoint) 
    {
        newPoint = indexPoint;
        newInnerPoint = 0;
    }
    public void MoveToInner(int indexPoint, List<Transform> inners) 
    {
        if (indexPoint != 0) 
        {
            newInnerPoint = indexPoint;
            innerWalkPoints = inners;
        }
        else 
        {
            //returnParentPoint = true;
            newInnerPoint = 0;
        }
    }
    void Movement(int indexPoint) 
    {
        readyWalk = false;
        animator.SetTrigger("Move");
        if (currentPoint < indexPoint) 
        {
            Vector3 direction = (walkPoints[currentPoint + 1].position - transform.position).normalized;
            FaceTarget(walkPoints[currentPoint + 1].position);
            rigi.MovePosition(transform.position + direction * speed * Time.deltaTime);
        }
        if (currentPoint > indexPoint) 
        {
            Vector3 direction = (walkPoints[currentPoint - 1].position - transform.position).normalized;
            FaceTarget(walkPoints[currentPoint - 1].position);
            rigi.MovePosition(transform.position + direction * speed * Time.deltaTime);
        }
        if (currentPoint == indexPoint) newPoint = currentPoint;
    }
    void MovementInner(int indexPoint) 
    {
        readyWalk = false;
        animator.SetTrigger("Move");
        if (currentInnerPoint < indexPoint) 
        {
            Vector3 direction = (innerWalkPoints[currentInnerPoint].position - transform.position).normalized;
            FaceTarget(innerWalkPoints[currentInnerPoint].position);
            rigi.MovePosition(transform.position + direction * speed * Time.deltaTime);
        }
        if (currentInnerPoint > indexPoint) 
        {
            if (indexPoint == 0) 
            {
                Vector3 direction = (walkPoints[currentPoint].position - transform.position).normalized;
            FaceTarget(walkPoints[currentPoint].position);
            rigi.MovePosition(transform.position + direction * speed * Time.deltaTime);
            }
            else 
            {
                Vector3 direction = (innerWalkPoints[currentInnerPoint-1].position - transform.position).normalized;
            FaceTarget(innerWalkPoints[currentInnerPoint-1].position);
            rigi.MovePosition(transform.position + direction * speed * Time.deltaTime);
            }
        }
        if (currentInnerPoint == indexPoint) newInnerPoint = currentInnerPoint;
    }
    private IEnumerator SetParentIndex(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        readyWalk = true;
    }
    private IEnumerator AttackPause(float waitTime)
    {
        if (GetComponent<FieldOfView>().CheckTargets()) 
        {
            Vector3 target = GetComponent<FieldOfView>().GetNearestTargets(transform.position).position;
            FaceTarget(target);
            animator.SetTrigger("AttackDown");
        }
        yield return new WaitForSeconds(waitTime);
        if (GetComponent<FieldOfView>().CheckTargets()) 
        {
            if (GetComponent<FieldOfView>().GetNearestTargets(transform.position).GetComponent<PlayerController>()) 
            {
                GetComponent<FieldOfView>().GetNearestTargets(transform.position).GetComponent<PlayerController>().stopped = true;
                GetComponent<FieldOfView>().GetNearestTargets(transform.position).GetComponent<PlayerController>().Dead();
            }
        }
        yield return new WaitForSeconds(2.0f);
        if (GetComponent<FieldOfView>().CheckTargets()) 
        {
            Vector3 target = GetComponent<FieldOfView>().GetNearestTargets(transform.position).position;
            GetComponent<Projectile>().SetData(target, transform.position);
            readyDestroy = true;
        }
        if (!GetComponent<FieldOfView>().CheckTargets()) animator.SetTrigger("AttackReset");
    }
    void FaceTarget(Vector3 point)
    {
        Vector3 direction = (point - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player"))  catched = true;
    }
}
