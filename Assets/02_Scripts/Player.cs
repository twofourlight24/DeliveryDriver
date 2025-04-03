using UnityEngine;

public class Player
{
    public int Health = 100;
    public static int PlayerCount = 0;

    public Player()
    {
        PlayerCount++;
    }

    public void TakeDamage(int  damage)
    {
        Health -= damage;
    }

    public void Defend()
    {
        int damage = 5;
        Debug.Log("���� : " + damage);
    }
    public void Attack()
    {
        int damage = 5;
        Debug.Log("���ݷ� : " + damage);
    }

}
