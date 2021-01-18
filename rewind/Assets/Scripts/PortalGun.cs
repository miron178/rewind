using UnityEngine;

public class PortalGun : Rewindable
{
    [SerializeField]
    private PortalBullet bulletPrefab;
    [SerializeField]
    private Vector3 bulletPosition = new Vector3(0f, 0f, 0f);
    private bool canFire = true;

    public bool Fire(Vector3 direction)
    {
        if (timeline.GetReplay() || !canFire)
            return false;

        PortalBullet bullet;
        bullet = Instantiate<PortalBullet>(bulletPrefab, 
            transform.position + bulletPosition, 
            Quaternion.identity);
        bullet.transform.SetParent(transform);
        bullet.Fire(direction);
        return true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall"))
        {
            canFire = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall"))
        {
            canFire = true;
        }
    }
}
