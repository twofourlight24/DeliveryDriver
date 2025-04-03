using UnityEngine;

public class Opearter : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Ex1();
        Ex2();
        Ex3();
    }



    private static void Ex1()
    {
        int health = 0;

        if (health > 70)
        {
            Debug.Log("건강해요");
        }
        else if (health > 30)
        {
            Debug.Log("약간 지쳤어요");
        }
        else if (health > 0)
        {
            Debug.Log("위험해요..");
        }
        else
        {
            Debug.Log("사망");
        }
    }

    void Ex2()
    {
        float mathScore = 90;
        float englishScore = 90;

        if (mathScore >= 60 && englishScore >= 60)
        {
            if ((mathScore + englishScore) / 2 >= 90)
            {
                Debug.Log("우수 합격");
            }
            else
                Debug.Log("합격");
        }
        else
            Debug.Log("불합격");
    }

    void Ex3()
    {
        int level = 5;
        bool hasSpecialItem = true;
        bool isInBattle = true;

        if (level >= 5 && hasSpecialItem && isInBattle)
        {
            Debug.Log("아이템 사용 가능");
        }
        else
            Debug.Log("아이템 사용 불가");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
