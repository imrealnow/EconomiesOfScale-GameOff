using System;
using UnityEngine;
namespace prototyping
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float jumpForce = 5f;
        [SerializeField] private float rampUpTime = 0.5f;
        [SerializeField] private float rampDownTime = 0.5f;
        [SerializeField] private float maxSpeed = 10f;

        [Header("Resizing")]
        [SerializeField] private float scaleSpeed = 0.01f;
        [SerializeField] private float minScale = 0.3f;
        [SerializeField] private float maxScale = 3f;

        [Header("Physics")]
        public LayerMask groundLayer;
        [SerializeField] private Transform groundCheckTransform;

        [Header("References")]
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private Animator animator;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private InputMaster inputActions;

        private float targetSpeed = 0f;
        private float currentSpeed = 0f;
        private float currentScale = 1f;
        private float targetScale = 1f;

        private bool isMoving = false;
        private Vector2 velocity = Vector2.zero;

        public Boolean IsGrounded
        {
            get
            {
                return Physics2D.Raycast(groundCheckTransform.position, Vector2.down, 0.1f, groundLayer).collider != null;
            }
        }

        private void Awake()
        {
            inputActions = new InputMaster();
            inputActions.Player.Jump.performed += ctx => Jump();
            inputActions.Player.Move.performed += ctx => Move(ctx.ReadValue<Vector2>().x);
            inputActions.Player.Scale.performed += ctx => Scale(ctx.ReadValue<float>());
        }

        private void OnEnable()
        {
            inputActions.Enable();
        }

        private void OnDisable()
        {
            inputActions.Disable();
        }

        public void FixedUpdate()
        {
            ApplyMovement();
            ApplyScale();
        }

        private void ApplyMovement()
        {
            // If we are moving, we want to reach the target speed using the ramp-up time
            if (isMoving)
            {
                currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, Time.deltaTime / rampUpTime * maxSpeed);
            }
            // If we are not moving, we want to slow down to a stop using the ramp-down time
            else
            {
                currentSpeed = Mathf.MoveTowards(currentSpeed, 0, Time.deltaTime / rampDownTime * maxSpeed);
            }

            // Apply the calculated velocity to the rigidbody
            velocity.x = currentSpeed;
            Vector2 currentVelocity = rb.velocity;
            currentVelocity.x = velocity.x;
            rb.velocity = currentVelocity;
        }

        private void Scale(float amount)
        {
            targetScale = Math.Clamp(currentScale + amount * scaleSpeed, minScale, maxScale);

        }

        private void ApplyScale()
        {
            currentScale = Mathf.Lerp(currentScale, targetScale, 0.5f);
            transform.localScale = new Vector3(currentScale, currentScale, currentScale);
        }

        private void Move(float direction)
        {
            // If there is no input, set isMoving to false
            if (direction == 0)
            {
                isMoving = false;
            }
            else // If there is input, set the target speed and isMoving to true
            {
                targetSpeed = maxSpeed * direction;
                isMoving = true;
                spriteRenderer.flipX = direction < 0f;
            }

            // Set the animator speed parameter
            animator.SetFloat("Speed", Mathf.Abs(currentSpeed));
        }

        private void Jump()
        {
            if (IsGrounded)
            {
                // Ensures that we're not carrying over any downward velocity when we jump
                rb.velocity = new Vector2(rb.velocity.x, 0f);
                rb.AddForce(Vector2.up * jumpForce * rb.mass, ForceMode2D.Impulse);
                animator.SetTrigger("Jump");
            }
        }

        private void ApplyGravity()
        {
            if (IsGrounded && rb.velocity.y < 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0);
            }
        }
    }

}

