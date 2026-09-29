using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;

[RequireComponent(typeof(NavMeshAgent))]
public class DragonEnemy : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Animator animator;
    [SerializeField] private VisualEffect fireBreathEffect;

    [SerializeField] private GameObject FireAttack;
    [SerializeField] private GameObject TailAttack;
    [SerializeField] private GameObject FlyAttack;


    [SerializeField] private float fireCooldown = 7f;
    [SerializeField] private float tailCooldown = 5f;
    [SerializeField] private float flyCooldown = 9f;

    public BoxCollider FireDetect;
    public BoxCollider TailDetect;
    public BoxCollider FlyDetect;

    public int Health = 100;

    [Header("Settings")]
    [SerializeField] private float chaseRange = 25f;

    private NavMeshAgent agent;

    [SerializeField] private float globalCooldown = 2f;

    private float nextAttackTime;

    public bool isAttacking;

    public bool canFire;
    public bool canTail;
    public bool canFly;

    public bool isFlameActive;
    public bool isTailActive;
    public bool isFlyActive;

    private float nextFireTime;
    private float nextTailTime;
    private float nextFlyTime;



    private enum AttackType
    {
        None,
        Fire,
        Tail,
        Fly
    }

    private AttackType currentAttack = AttackType.None;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        fireBreathEffect.Stop();

        FireAttack.SetActive(false);
        TailAttack.SetActive(false);
        FlyAttack.SetActive(false);
    }

    private void Update()
    {

        canFire = Time.time >= nextAttackTime && Time.time >= nextFireTime;

        canTail = Time.time >= nextAttackTime && Time.time >= nextTailTime;

        canFly = Time.time >= nextAttackTime && Time.time >= nextFlyTime;


        FireAttack.SetActive(isFlameActive);
        TailAttack.SetActive(isTailActive);
        FlyAttack.SetActive(isFlyActive);

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > chaseRange)
        {
            agent.isStopped = true;
            animator.SetBool("Walk", false);
            return;
        }
        if (isAttacking)
        {
            agent.isStopped = true;
            animator.SetBool("Walk", false);
            return;
        }

        // Always look at the player
        Vector3 look = player.position - transform.position;
        look.y = 0;

        if (look.sqrMagnitude > 0.01f)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(look),
                8f * Time.deltaTime);
        }


        currentAttack = AttackType.None;

        if (canFire && IsPlayerInside(FireDetect))
        {
            currentAttack = AttackType.Fire;
        }
        else if (canTail && IsPlayerInside(TailDetect))
        {
            currentAttack = AttackType.Tail;
        }
        else if (canFly && IsPlayerInside(FlyDetect))
        {
            currentAttack = AttackType.Fly;
        }

        if (currentAttack != AttackType.None && !isAttacking)
        {
            // Attack
            agent.isStopped = true;
            animator.SetBool("Walk", false);

            AttackRoutine();
        }
        else
        {
            // Chase player
            agent.isStopped = false;
            agent.SetDestination(player.position);
            animator.SetBool("Walk", true);
        }

    }

    private void AttackRoutine()
    {
        isAttacking = true;

        switch (currentAttack)
        {
            case AttackType.Fire:

                animator.SetTrigger("Flame");
                StartCoroutine(PlayFire());

                nextFireTime = Time.time + fireCooldown;
                nextAttackTime = Time.time + globalCooldown;
                break;

            case AttackType.Tail:

                animator.SetTrigger("Tail");

                nextTailTime = Time.time + tailCooldown;
                nextAttackTime = Time.time + globalCooldown;
                break;

            case AttackType.Fly:

                animator.SetTrigger("Fly");

                nextFlyTime = Time.time + flyCooldown;
                nextAttackTime = Time.time + globalCooldown;
                break;
        }
    }


    IEnumerator PlayFire()
    {
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Flame"))
            yield return null;

        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.1967f)
            yield return null;

        fireBreathEffect.Play();

        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < (50f / 60f))
            yield return null;

        isFlameActive = true;

        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
            yield return null;

        isFlameActive = false;
        isAttacking = false;
        fireBreathEffect.Stop();
    }

    // Animation Events
    public void tailActive() => isTailActive = true;
    public void tailDeactive()
    {
        isTailActive = false;
        isAttacking = false;
    }

    public void flyActive() => isFlyActive = true;
    public void flyDeactive()
    {
        isFlyActive = false;
        isAttacking = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        DrawBoxCollider(FireDetect, Color.red);
        DrawBoxCollider(TailDetect, Color.green);
        DrawBoxCollider(FlyDetect, Color.cyan);
    }

    private void DrawBoxCollider(BoxCollider col, Color color)
    {
        if (col == null) return;

        Gizmos.color = color;

        Matrix4x4 oldMatrix = Gizmos.matrix;

        Gizmos.matrix = Matrix4x4.TRS(
            col.transform.position,
            col.transform.rotation,
            col.transform.lossyScale);

        Gizmos.DrawWireCube(col.center, col.size);

        Gizmos.matrix = oldMatrix;
    }

    private bool IsPlayerInside(BoxCollider box)
    {
        Collider[] hits = Physics.OverlapBox(
            box.bounds.center,
            box.bounds.extents,
            box.transform.rotation);

        Debug.Log($"{box.name} detected {hits.Length} colliders");

        foreach (Collider hit in hits)
        {
            Debug.Log(hit.name);

            if (hit.CompareTag("Player"))
            {
                Debug.Log("PLAYER FOUND");
                return true;
            }
        }

        return false;
    }
}