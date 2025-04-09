using UnityEngine;

public class Drift : MonoBehaviour
{
    public float accleration = 60.0f;         // 전진 / 후진 가속도
    public float steering = 5.0f;               // 조항 속도
    public float maxSpeed = 30.0f;
    public float driftFactor = 0.95f;         // 낮을 수록 더 미끄러짐

    private float currentAccel = 0f;
    public float accelRate = 30f; // 가속 증가 속도

    public float slowAccleraionRatio = 0.5f;
    public float boostAccleraionRatio = 1.5f;

    // 이펙트
    public ParticleSystem smokeLeft;
    public ParticleSystem smokeRight;
    public TrailRenderer leftTrail;
    public TrailRenderer rightTrail;

    Rigidbody2D rb;
    AudioSource audioSource;

    float defaultAccleration;
    float slowAccleration;
    float boostAccleration;

    public int boosterCount = 0;

    private float driftTimer = 0f;
    private float driftThreshold = 2f;
    private bool boostGiven = false;

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

        if (verticalInput > 0)
            currentAccel = Mathf.MoveTowards(currentAccel, accleration, accelRate * Time.fixedDeltaTime);
        else if (verticalInput < 0)
            currentAccel = Mathf.MoveTowards(currentAccel, -accleration * 5, accelRate * 1.5f * Time.fixedDeltaTime);
        else
            currentAccel = Mathf.MoveTowards(currentAccel, 0, accelRate * Time.fixedDeltaTime);

        if (Input.GetAxis("Vertical") == 0)
        {
            rb.linearVelocity *= 0.9f; // 관성 줄이기 - 1에 가까울수록 감속이 느림
        }

        float speed = Vector2.Dot(rb.linearVelocity, transform.up);
        if (Mathf.Abs(speed) < maxSpeed || Mathf.Sign(speed) != Mathf.Sign(currentAccel)) // 후진은 반대방향 가능
        {
            rb.AddForce(transform.up * currentAccel);
        }

        if (Mathf.Abs(speed) < maxSpeed)
        {
            rb.AddForce(transform.up * currentAccel);
        }

        float turnAmount = Input.GetAxis("Horizontal") * steering * Mathf.Clamp(speed / maxSpeed, 0.4f, 1.0f);
        rb.MoveRotation(rb.rotation - turnAmount);

        // Drift
        Vector2 fowardVelocity = transform.up * Vector2.Dot(rb.linearVelocity, transform.up);
        Vector2 sideVelocity = transform.right * Vector2.Dot(rb.linearVelocity, transform.right);
        rb.linearVelocity = fowardVelocity + sideVelocity * driftFactor;

    }

    void Update()
    {
        float sidewayVelocity = Vector2.Dot(rb.linearVelocity, transform.right);

        bool isDriftting = rb.linearVelocity.magnitude > 2.0f && Mathf.Abs(sidewayVelocity) > 2.0f;
        if (isDriftting)
        {
            driftTimer += Time.deltaTime;

            if (boosterCount < 2)
            {
                if (driftTimer >= driftThreshold && !boostGiven)
                {
                    boosterCount++;
                    boostGiven = true;
                    driftTimer = 0f;
                    GameMgr.Instance.UpdateDriftGauge(0f); // 리셋
                    Debug.Log($" 부스터 획득! 현재: {boosterCount}");
                }
                else
                {
                    GameMgr.Instance.UpdateDriftGauge(driftTimer);
                }
            }
            else
            {
                // 부스터 2개 찼으면: 게이지는 2초까지만 보여주고 그 이상이면 0
                if (driftTimer < driftThreshold)
                    GameMgr.Instance.UpdateDriftGauge(driftTimer);
                else
                    GameMgr.Instance.UpdateDriftGauge(0f);
            }


            if (!audioSource.isPlaying) audioSource.Play();
            if (!smokeLeft.isPlaying) smokeLeft.Play();
            if (!smokeRight.isPlaying) smokeRight.Play();
        }
        else
        {
            if (audioSource.isPlaying) audioSource.Stop();
            if (smokeLeft.isPlaying) smokeLeft.Stop();
            if (smokeRight.isPlaying) smokeRight.Stop();
        }

        leftTrail.emitting = isDriftting;
        rightTrail.emitting = isDriftting;

        float dot = Vector2.Dot(transform.up, rb.linearVelocity.normalized);

        bool isWrongWay = dot < -0.5f && rb.linearVelocity.magnitude > 2f;

        GameMgr.Instance.ShowWrongWay(isWrongWay);

    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("StartLine"))
        {
            GameMgr.Instance.LabCount++;
            Debug.Log("랩 증가: " + GameMgr.Instance.LabCount);
        }

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
        accleration = defaultAccleration * slowAccleraionRatio;
        rb.linearVelocity *= 0.3f;
        Debug.Log("속도 감소");
        Invoke(nameof(ResetAccleration), 3f);
    }
}
