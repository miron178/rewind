using UnityEngine;

public class Spikes : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Hero hero = other.GetComponent<Hero>();
            hero.Kill();
        }
    }
}