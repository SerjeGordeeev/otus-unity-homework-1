using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public enum State
    {
        Idle,
        RunningToEnemy,
        RunningFromEnemy,
        BeginAttack,
        Attack,
        BeginShoot,
        Shoot,
        Dead
    }

    public enum Weapon
    {
        Pistol,
        Bat,
        Hand
    }

    Animator animator;
    State state;

    public Weapon weapon;
    public Character target;
    public float runSpeed;
    public float distanceFromEnemy;
    Vector3 originalPosition;
    Quaternion originalRotation;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        state = State.Idle;
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    public void SetState(State newState)
    {
        state = newState;
    }

    [ContextMenu("Attack")]
    void AttackEnemy()
    {
        if (target == null)
        {
            Debug.LogWarning("Target enemy is not found");
            return;
        }

        if (target.state == State.Dead)
        {
            Debug.LogWarning("Character is already dead");
            return;
        }

        if (state == State.Dead)
        {
            Debug.LogWarning("Character is dead");
            return;
        }

        switch (weapon) {
            case Weapon.Bat:
            case Weapon.Hand:
                state = State.RunningToEnemy;
                break;
            case Weapon.Pistol:
                state = State.BeginShoot;
                break;
        }
    }

    void FixedUpdate()
    {
        switch (state) {
            case State.Idle:
                transform.rotation = originalRotation;
                animator.SetFloat("Speed", 0.0f);
                break;

            case State.RunningToEnemy:
                animator.SetFloat("Speed", runSpeed);
                if (RunTowards(target.transform.position, distanceFromEnemy))
                    state = State.BeginAttack;
                break;

            case State.RunningFromEnemy:
                animator.SetFloat("Speed", runSpeed);
                if (RunTowards(originalPosition, 0.0f))
                    state = State.Idle;
                break;

            case State.BeginAttack:
                if (weapon == Weapon.Bat)
                {
                    animator.SetTrigger("MeleeAttack");
                } 

                if (weapon == Weapon.Hand) {
                    animator.SetTrigger("HandAttack");
                }
               
                state = State.Attack;
                break;

            case State.BeginShoot:
                animator.SetTrigger("Shoot");
                state = State.Shoot;
                break;

            case State.Attack:
                break;
            case State.Shoot:
                break;

            case State.Dead:
                animator.SetBool("Dead", true);
                break;
        }
    }

    public void KillTargetEnemy()
    {
        if(target.state != State.Dead)
        {
            target.SetState(State.Dead);
        }
    }
    
    bool RunTowards(Vector3 targetPosition, float distanceFromTarget)
        {
            Vector3 distance = targetPosition - transform.position;
            if (distance.magnitude < 0.00001f) {
                transform.position = targetPosition;
                return true;
            }
    
            Vector3 direction = distance.normalized;
            transform.rotation = Quaternion.LookRotation(direction);
    
            targetPosition -= direction * distanceFromTarget;
            distance = (targetPosition - transform.position);
    
            Vector3 step = direction * runSpeed;
            if (step.magnitude < distance.magnitude) {
                transform.position += step;
                return false;
            }
    
            transform.position = targetPosition;
            return true;
        }
}
