using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.VFX;

[RequireComponent(typeof(Rigidbody))]
public class DragonMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private Animator animator;
    [SerializeField] private VisualEffect fireBreathEffect;
    [SerializeField] private GameObject FireAttack;
    [SerializeField] private GameObject TailAttack;
    [SerializeField] private GameObject FlyAttack;
    [SerializeField] private Image FireUI;
    [SerializeField] private Image TailUI;
    [SerializeField] private Image FlyUI;

    public int Health = 100;

    private Rigidbody rb;
    private Camera mainCamera;

    public bool isFlameActive = false;
    public bool isTailActive = false;
    public bool isFlyActive = false;

    [SerializeField] private float fireCooldown = 7f;
    [SerializeField] private float tailCooldown = 5f;
    [SerializeField] private float flyCooldown = 9f;

    [SerializeField] private float globalCooldown = 2f;

    private float nextFireTime;
    private float nextTailTime;
    private float nextFlyTime;
    private float nextAttackTime;

    public bool isAttacking;

    // Read-only status booleans
    public bool canFire;
    public bool canTail;
    public bool canFly;
    public bool canAttack;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;

        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }
    private void Start()
    {
        fireBreathEffect.Stop();
    }

    private void Update()
    {
        canAttack = Time.time >= nextAttackTime;

        canFire = canAttack && Time.time >= nextFireTime;
        canTail = canAttack && Time.time >= nextTailTime;
        canFly = canAttack && Time.time >= nextFlyTime;

        if (canFire)
        {
            FireUI.color = Color.white;
        }
        else
        {
            FireUI.color = Color.gray;
        }

        if (canTail)
        {
            TailUI.color = Color.white;
        }
        else
        {
            TailUI.color = Color.gray;
        }

        if (canFly)
        {
            FlyUI.color = Color.white;
        }
        else
        {
            FlyUI.color = Color.gray;
        }

        FireAttack.SetActive(isFlameActive);
        TailAttack.SetActive(isTailActive);
        FlyAttack.SetActive(isFlyActive);

        if (canAttack && !isAttacking)
        {
            if (Keyboard.current.digit1Key.wasPressedThisFrame && canFire)
            {
                isAttacking = true;
                animator.SetTrigger("Flame");
                StartCoroutine(PlayFire());
                Invoke(nameof(flameClose), 1.5f);

                nextFireTime = Time.time + fireCooldown;
                nextAttackTime = Time.time + globalCooldown;
            }

            if (Keyboard.current.digit2Key.wasPressedThisFrame && canTail)
            {
                isAttacking = true;
                animator.SetTrigger("Tail");

                nextTailTime = Time.time + tailCooldown;
                nextAttackTime = Time.time + globalCooldown;
            }

            if (Keyboard.current.digit3Key.wasPressedThisFrame && canFly)
            {
                isAttacking = true;
                animator.SetTrigger("Fly");

                nextFlyTime = Time.time + flyCooldown;
                nextAttackTime = Time.time + globalCooldown;
            }
        }   
    }
    private void FixedUpdate()
    {
        Vector2 input = Vector2.zero;
        if (!isFlameActive)
        {
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) input.y += 1;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) input.y -= 1;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) input.x += 1;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) input.x -= 1;
        }


        input = Vector2.ClampMagnitude(input, 1);

        Vector3 forward = mainCamera.transform.forward;
        Vector3 right = mainCamera.transform.right;

        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = forward * input.y + right * input.x;

        rb.linearVelocity = new Vector3(
            moveDirection.x * moveSpeed,
            rb.linearVelocity.y,
            moveDirection.z * moveSpeed
        );

        // Walk Animation
        if (animator != null)
        {
            animator.SetBool("Walk", moveDirection.sqrMagnitude > 0.01f);
        }

        // Rotation
        if (moveDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.fixedDeltaTime
            );
        }
    }
    public void flameClose()
    {
        fireBreathEffect.Stop();
        isFlameActive = false;
        isAttacking = false;
    }
    private IEnumerator PlayFire()
    {
        // Wait until the Animator has actually entered the Flame state.
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Flame"))
            yield return null;

        // Wait until the animation reaches the desired point.
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.1967f)
            yield return null;
        fireBreathEffect.Play();
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < (50f / 60f))
            yield return null;
        isFlameActive = true;

       
    }

    //private void OnTriggerStay(Collider other)
    //{
    //    if (isFlameActive && other.CompareTag("Enemy"))
    //    {
    //        Destroy(other.gameObject);
    //    }

    //    if (isTailActive && other.CompareTag("Enemy"))
    //    {
    //        Destroy(other.gameObject);
    //    }
    //    if (isTailActive && other.CompareTag("Enemy"))
    //    {
    //        Destroy(other.gameObject);
    //    }
    //}
    public void tailActive()
    {
        isTailActive = true;
    }
    public void tailDeactive()
    {
        isTailActive = false;
        isAttacking = false;
    }
    public void flyActive()
    {
        isFlyActive = true;
    }   
    public void flyDeactive()
    {
        isFlyActive = false;
        isAttacking = false;
    }
}