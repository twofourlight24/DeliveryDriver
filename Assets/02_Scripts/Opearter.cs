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
            Debug.Log("�ǰ��ؿ�");
        }
        else if (health > 30)
        {
            Debug.Log("�ణ ���ƾ��");
        }
        else if (health > 0)
        {
            Debug.Log("�����ؿ�..");
        }
        else
        {
            Debug.Log("���");
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
                Debug.Log("��� �հ�");
            }
            else
                Debug.Log("�հ�");
        }
        else
            Debug.Log("���հ�");
    }

    void Ex3()
    {
        int level = 5;
        bool hasSpecialItem = true;
        bool isInBattle = true;

        if (level >= 5 && hasSpecialItem && isInBattle)
        {
            Debug.Log("������ ��� ����");
        }
        else
            Debug.Log("������ ��� �Ұ�");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
