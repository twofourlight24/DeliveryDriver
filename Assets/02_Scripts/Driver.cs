using UnityEngine;

public class Driver : MonoBehaviour
{
    [Header("=== Speed ===")]
    [SerializeField] float TurnSpeed = 100.0f;
    [SerializeField] float MoveSpeed = 25.0f;
    [SerializeField] float slowSpeedRatio = 0.5f;
    [SerializeField] float boostSpeedRatio = 1.5f;
    float slowSpeed;
    float boostSpeed;

    void Start()
    {
        slowSpeed = MoveSpeed * slowSpeedRatio;
        boostSpeed = MoveSpeed * boostSpeedRatio;
    }

    void Update()
    {
        float TurnAmount = Input.GetAxis("Horizontal") * TurnSpeed * Time.deltaTime;
        float MoveAmount = Input.GetAxis("Vertical") * MoveSpeed * Time.deltaTime;

        transform.Rotate(0, 0, -TurnAmount);
        transform.Translate(0, MoveAmount, 0);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Boost"))
        {
            MoveSpeed *= boostSpeedRatio;
            Debug.Log("속도 상승!");
            Destroy(other.gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        MoveSpeed = slowSpeed;
        Debug.Log("속도 감소");
    }
}

