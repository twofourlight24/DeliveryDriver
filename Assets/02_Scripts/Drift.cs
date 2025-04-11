using JetBrains.Annotations;
using UnityEngine;

public class Drift : MonoBehaviour
{
    public float accleration = 60.0f;         // 전진 / 후진 가속도
    public float steering = 5.0f;               // 조항 속도
    public float maxSpeed = 20.0f;
    public float driftFactor = 0.95f;         // 낮을 수록 더 미끄러짐

    private float currentAccel = 0f;
    public float accelRate = 30f; // 가속 증가 속도

    public float slowAccleraionRatio = 0.5f;
    public float boostAccleraionRatio = 1.5f;

    // 부스터
    public float boostDuration = 4.0f;
    public float boostMultiplier = 3.0f;

    private bool isBoosting = false;
    int boosterCount = 0;

    // 이펙트
    public ParticleSystem smokeLeft;
    public ParticleSystem smokeRight;
    public Transform boostImage1;
    public Transform boostImage2;
    public TrailRenderer leftTrail;
    public TrailRenderer rightTrail;


    Rigidbody2D rb;
    public AudioSource driftSound;
    public AudioSource boostSound;

    float defaultAccleration;
    float slowAccleration;
    float boostAccleration;

    // 드리프트 부스터 관련
    float driftTimer = 0f;
    float driftThreshold = 2f;
    bool isDrifting = false;

    // Lab 체크 변수
    bool passedMiddle = false;

    //뺑뺑이 체크
    public GameObject Gate;
    bool passGate = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        defaultAccleration = accleration;
        slowAccleration = accleration * slowAccleration;
        boostAccleration *= boostAccleration;

        boostImage1.gameObject.SetActive(false);
        boostImage2.gameObject.SetActive(false);

        smokeLeft.Stop();
        smokeRight.Stop();
    }
    void FixedUpdate()
    {
        float verticalInput = Input.GetAxis("Vertical");

        if (verticalInput > 0)
            currentAccel = Mathf.MoveTowards(currentAccel, accleration, accelRate * Time.fixedDeltaTime);
        else if (verticalInput < 0)
            currentAccel = Mathf.MoveTowards(currentAccel, -accleration, accelRate  * Time.fixedDeltaTime);
        else
            currentAccel = Mathf.MoveTowards(currentAccel, 0, accelRate * Time.fixedDeltaTime);

        if (Input.GetAxis("Vertical") == 0)
        {
            rb.linearVelocity *= 0.7f; // 관성 줄이기 - 1에 가까울수록 감속이 느림
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
        isDrifting = rb.linearVelocity.magnitude > 10.0f && Mathf.Abs(sidewayVelocity) > 3.0f;

        // 드리프트 중일 때
        if (isDrifting)
        {
            driftTimer += Time.deltaTime;

            // 드리프트 시간이 2초 이상이고, 부스터 개수가 2개 미만일떄
            if (driftTimer >= driftThreshold && boosterCount < 2)
            {
                boosterCount++;
                driftTimer = 0f; // 2초 넘으면 리셋 (다시 모으려면 새로)
                Debug.Log($"🔥 부스터 충전! 현재: {boosterCount}");
                GameMgr.Instance.UpdateBoosterUI(boosterCount); // UI 줄이기
            }
            GameMgr.Instance.UpdateDriftGauge(driftTimer);

            if (!driftSound.isPlaying) driftSound.Play();
            if (!smokeLeft.isPlaying) smokeLeft.Play();
            if (!smokeRight.isPlaying) smokeRight.Play();
        }
        else
        {
            if (driftTimer < driftThreshold)
                GameMgr.Instance.UpdateDriftGauge(driftTimer);
            else
            {
                driftTimer = 0f;
                GameMgr.Instance.UpdateDriftGauge(0f);
            }
            if (driftSound.isPlaying) driftSound.Stop();
            if (smokeLeft.isPlaying) smokeLeft.Stop();
            if (smokeRight.isPlaying) smokeRight.Stop();
        }
      
        leftTrail.emitting = isDrifting;
        rightTrail.emitting = isDrifting;

        if (Input.GetKeyDown(KeyCode.LeftControl)) // 혹은 UI 버튼이든
        {
            UseBooster();
        }

    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Middle"))
        {
            passedMiddle = true;
            Debug.Log("중간지점 통과");
        }
        if (other.CompareTag("StartLine") && passedMiddle)
        {
            GameMgr.Instance.LabCount++;
            Debug.Log(" 랩 증가: " + GameMgr.Instance.LabCount);

            passedMiddle = false; // 다음 랩 위해 초기화
        }
        if (other.CompareTag("Check"))
        {
            passGate = true;
            Debug.Log("뺑뺑이 통과");
        }
        if(passGate)
        {
            Gate.gameObject.SetActive(false);
            passGate = false;
            Debug.Log("dd");
        }
        else
        {
            Gate.gameObject.SetActive(true);
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
        maxSpeed = 20f;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // 충돌한 면의 법선 벡터
        Vector2 collisionNormal = collision.contacts[0].normal;

        // 내 앞 방향
        Vector2 forward = transform.up;

        // 얼마나 정면으로 박았는지 (1에 가까울수록 정면)
        float impact = Vector2.Dot(-collisionNormal, forward);

        // 정면 충돌로 간주되는 기준값 (0.8 이상이면 거의 정면)
        if (impact > 0.8f)
        {
            // 전진속도만 날리고 측면은 살림
            Vector2 sideVelocity = transform.right * Vector2.Dot(rb.linearVelocity, transform.right);
            rb.linearVelocity = sideVelocity * 0.3f;

            currentAccel = 0f;
            accleration = 0f;
            maxSpeed = 0f;
            Debug.Log(" 정면 충돌! 전진 속도 제거");
            Invoke(nameof(ResetAccleration), 0.5f);
        }
        else
        {
            Debug.Log("측면 긁힘 → 속도 유지");
            // 필요하면 여기서 가벼운 마찰만 주는 것도 가능
        }
    }

    void UseBooster()
    {
        if (boosterCount <= 0 || isBoosting) return;

        boosterCount--;
        isBoosting = true;
        maxSpeed = 40f;
        accleration *= boostMultiplier;

        // 부스터 효과
        GameMgr.Instance.UpdateBoosterUI(boosterCount); // UI 줄이기
        PlayBoosterEffect(); // 이펙트 처리 (파티클, 사운드 등)

        Invoke(nameof(EndBoost), boostDuration);
        Invoke(nameof(EndBoosterEffect), boostDuration);
    }
    void EndBoost()
    {
        accleration = defaultAccleration;
        maxSpeed = 20f;
        isBoosting = false;
    }
    void PlayBoosterEffect()
    {
        boostSound.Play();
        boostImage1.gameObject.SetActive(true);
        boostImage2.gameObject.SetActive(true);
    }
    void EndBoosterEffect()
    {
        boostSound.Stop();
        boostImage1.gameObject.SetActive(false);
        boostImage2.gameObject.SetActive(false);
    }
}
