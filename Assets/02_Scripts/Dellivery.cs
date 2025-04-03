using UnityEngine;

public class Dellivery : MonoBehaviour
{
    [SerializeField] float delayDestory = 1.0f;
    [SerializeField] Color noChickenColor =new Color (1, 1, 1, 1 );
    [SerializeField] Color hasChickenColor = new Color(0.9f, 1.0f, 0.6f, 1.0f);

    bool hasChicken = false;
    SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        Debug.Log(spriteRenderer);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Chicken") && !hasChicken)
        {
            Debug.Log("Ä¡Å² ÇÈ¾÷µÊ");
            hasChicken = true;
            spriteRenderer.color = hasChickenColor;
            Destroy(collision.gameObject,delayDestory);
        }
        if (collision.gameObject.CompareTag("Customer") && hasChicken)
        {
            Debug.Log("Ä¡Å² ¹è´ÞµÊ");
            hasChicken = false;
            spriteRenderer.color = noChickenColor;
        }
    }
}
