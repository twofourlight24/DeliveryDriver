using UnityEngine;

public class Drift : MonoBehaviour
{
    [SerializeField] float accleration = 20.0f;         // 전진 / 후진 가속도
    [SerializeField] float steering = 3.0f;               // 조항 속도
    [SerializeField] float maxSpeed = 30.0f;
    [SerializeField] float driftFactor = 0.95f;         // 낮을 수록 더 미끄러짐

    // 이펙트
    [SerializeField] ParticleSystem smokeLeft;
    [SerializeField] ParticleSystem smokeRight;

    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    } 
    void FixedUpdate()
    {
        float speed = Vector2.Dot(rb.linearVelocity, transform.up);
        if (speed < maxSpeed)
        {
            rb.AddForce(transform.up * Input.GetAxis("Vertical") * accleration);
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

        bool isDriftting = rb.linearVelocity.magnitude > 2.0f && Mathf.Abs(sidewayVelocity) > 1.0f;
        if(isDriftting)
        {
            if(!smokeLeft.isPlaying) smokeLeft.Play();
            if(!smokeRight.isPlaying) smokeRight.Play();
        }
        else
        {
            if(smokeLeft.isPlaying) smokeLeft.Stop();
            if(smokeRight.isPlaying) smokeRight.Stop();
        }


    }
}
