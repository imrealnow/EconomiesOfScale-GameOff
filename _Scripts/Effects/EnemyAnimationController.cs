using UnityEngine;
using UnityEngine.Events;

public class EnemyAnimationController : MonoBehaviour
{
    public SVector3 playerPosition;
    public float moveThreshold = 0.5f;

    [Space] public UnityEvent onBeginMove;
    [Space] public UnityEvent onEndMove;

    private Animator animator;
    private VelocityInferencer velocityInferencer;
    private new SpriteRenderer renderer;
    private bool isMoving = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        velocityInferencer = GetComponent<VelocityInferencer>();
        renderer = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        Vector2 velocity = velocityInferencer.GetAverageVelocity();
        Vector2 targetDirection = playerPosition.Value - transform.position;
        bool facingUp = targetDirection.y > 0;
        bool facingLeft = targetDirection.x < 0;
        float animationSpeed = Vector2.Dot(velocity.normalized, targetDirection.normalized);
        animator.SetFloat("Speed", animationSpeed);
        animator.SetBool("FacingUp", facingUp);
        animator.SetBool("FacingLeft", facingLeft);
        renderer.flipX = velocity.x < 0;
        bool isMoving = velocity.magnitude > moveThreshold / Time.deltaTime;
        if (isMoving && !this.isMoving)
        {
            if (onBeginMove != null) onBeginMove.Invoke();
        }
        else if (!isMoving && this.isMoving)
        {
            if (onEndMove != null) onEndMove.Invoke();
        }
        this.isMoving = isMoving;
    }

}
