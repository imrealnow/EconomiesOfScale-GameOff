using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [Header("Parameters")]
    public float maxSpeed = 4;
    public float rampUpTime = 0.2f;
    public float rampDownTime = 0.1f;
    [Range(0f, 1f)]
    public float turningFactor = 0.9f;
    [Header("Curves")]
    public AnimationCurve rampUpCurve;
    public AnimationCurve rampDownCurve;


    private Rigidbody2D playerRb;
    private InputMaster inputMaster;
    private float currentSpeed = 0;
    private Vector2 currentDirection = Vector2.zero;
    private float targetSpeed = 0;

    private float timeSpentMoving = 0;
    private float timeSpentNotMoving = 0f;

    void Awake()
    {
        playerRb = GetComponent<Rigidbody2D>();
        inputMaster = new InputMaster();
    }

    private void OnEnable()
    {
        inputMaster.Player.Enable();
    }

    private void OnDisable()
    {
        inputMaster.Player.Disable();
    }

    public void UpdateMove(Vector2 direction)
    {
        // the player is not moving
        if (direction == Vector2.zero)
        {
            timeSpentMoving = 0;
            timeSpentNotMoving += Time.deltaTime;
            // ramp down speed
            if (timeSpentNotMoving <= rampDownTime && currentSpeed >= 0f)
            {
                float scaledTime = timeSpentNotMoving / rampDownTime;
                currentSpeed = Mathf.Max(currentSpeed - rampDownCurve.Evaluate(scaledTime) * maxSpeed, 0);
            }
        }
        // the player is moving
        else
        {
            currentDirection = Vector2.Lerp(direction, currentDirection, turningFactor);
            timeSpentNotMoving = 0;
            timeSpentMoving += Time.deltaTime;
            // ramp up speed
            if (timeSpentMoving <= rampUpTime)
            {
                float scaledTime = timeSpentMoving / rampUpTime;
                currentSpeed = Mathf.Min(rampUpCurve.Evaluate(scaledTime) * maxSpeed, maxSpeed);
            }
            else
            {
                // use max speed
                currentSpeed = maxSpeed;
            }
        }
    }

    private void FixedUpdate()
    {
        UpdateMove(inputMaster.Player.Move.ReadValue<Vector2>());
        playerRb.MovePosition((Vector2)transform.position + currentDirection * currentSpeed * Time.deltaTime);
    }
}
