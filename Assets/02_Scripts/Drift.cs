using UnityEngine;
using System.Collections;

public class Drift : MonoBehaviour
{
    [SerializeField] float accleration = 80.0f;         // 전진 / 후진 가속도
    [SerializeField] float steering = 3.0f;               // 조항 속도
    [SerializeField] float maxSpeed = 30.0f;
    [SerializeField] float driftFactor = 0.95f;         // 낮을 수록 더 미끄러짐

    private float currentAccel = 0f;
    public float accelRate = 10f; // 가속 증가 속도

    [SerializeField] float slowAccleraionRatio = 0.5f;
    [SerializeField] float boostAccleraionRatio = 1.5f;

    // 이펙트
    [SerializeField] ParticleSystem smokeLeft;
    [SerializeField] ParticleSystem smokeRight;
    [SerializeField] TrailRenderer leftTrail;
    [SerializeField] TrailRenderer rightTrail;

    Rigidbody2D rb;
    AudioSource audioSource;

    float defaultAccleration;
    float slowAccleration;
    float boostAccleration;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = rb.GetComponent<AudioSource>();
        
        defaultAccleration = accleration;
        slowAccleration = accleration * slowAccleration;
        boostAccleration *= boostAccleration;

    } 
    void FixedUpdate()
    {
        float verticalInput = Input.GetAxis("Vertical");
        float speed = Vector2.Dot(rb.linearVelocity, transform.up);

        // 🔥 뒷방향키 누르면 탁! 감속
        if (verticalInput < 0)
        {
            // 속도를 확 줄임 (감속 비율 조정 가능)
            rb.linearVelocity *= 0.9f;

            // 바로 후진 힘을 주고 싶으면 아래도 같이 사용
            currentAccel = Mathf.MoveTowards(currentAccel, -accleration, accelRate * Time.fixedDeltaTime);
        }
        else if (verticalInput > 0)
        {
            // 전진은 부드럽게
            currentAccel = Mathf.MoveTowards(currentAccel, accleration, accelRate * Time.fixedDeltaTime);
        }
        else
        {
            currentAccel = Mathf.MoveTowards(currentAccel, 0, accelRate * Time.fixedDeltaTime);
        }

        if (Mathf.Abs(speed) < maxSpeed)
        {
            rb.AddForce(transform.up * currentAccel);
        }

        float turnAmount = Input.GetAxis("Horizontal") * steering * Mathf.Clamp(speed / maxSpeed, 0.4f, 1.0f);
        rb.MoveRotation(rb.rotation - turnAmount);

       // Drift
        Vector2 fowardVelocity = transform.up * Vector2.Dot(rb.linearVelocity, transform.up);
        Vector2 sideVelocity = transform.right * Vector2.Dot(rb.linearVelocity,transform.right);
        rb.linearVelocity = fowardVelocity + sideVelocity*driftFactor;    

    }

    void Update()
    {
        float sidewayVelocity= Vector2.Dot(rb.linearVelocity, transform.right);

        bool isDriftting = rb.linearVelocity.magnitude > 2.0f && Mathf.Abs(sidewayVelocity) > 2.0f;
        if(isDriftting)
        {
            if(!audioSource.isPlaying) audioSource.Play();
            if(!smokeLeft.isPlaying) smokeLeft.Play();
            if(!smokeRight.isPlaying) smokeRight.Play();
        }
        else
        {
            if (audioSource.isPlaying) audioSource.Stop();
            if (smokeLeft.isPlaying) smokeLeft.Stop();
            if(smokeRight.isPlaying) smokeRight.Stop();
        }

        leftTrail.emitting = isDriftting;
        rightTrail.emitting = isDriftting;

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Boost"))
            {
                accleration = boostAccleration;
                Debug.Log("속도 상승!");
                Destroy(other.gameObject);

                Invoke(nameof(ResetAccleration), 5f);
            }
        }
        void ResetAccleration()
        {
            accleration = defaultAccleration;
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            accleration = slowAccleration;
            Debug.Log("속도 감소");
            Invoke(nameof(ResetAccleration), 3f);
        }

    }
}
