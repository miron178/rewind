using UnityEngine;

public class PortalBullet : MonoBehaviour
{
    private Rigidbody2D rb2d;  //access to physics engine (Unity)
    public Vector3 direction;  //public so that teleoprter can change it
    private float speed = .5f; //distance per fixed update
    public Portal portalPrefab;
    private PortalManager portalManager;

    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        portalManager = GameObject.Find("PortalManager").GetComponent<PortalManager>();
    }

    public void Fire(Vector3 dir)
    {
        //detach from parent but keep global position
        transform.SetParent(null, true);

        //keep direction, use speed as magnitude
        direction = Vector3.Normalize(dir) * speed;
    }

    private void FixedUpdate()
    {
        //keep moving
        transform.position += direction;
    }

    private void CreatePortal(Collision2D collision)
    {
        //find collision point on the wall
        ContactPoint2D[] contacts = new ContactPoint2D[1];
        int numContacts = collision.GetContacts(contacts);
        if (numContacts == 0)
            return;

        //rotate portal so that it faces away from the wall
        //using normal vector
        float angle = Vector2.Angle(Vector2.right, contacts[0].normal);
        Quaternion rotation = Quaternion.Euler(0f, 0f, angle);

        Portal newPortal = Instantiate<Portal>(portalPrefab, 
            transform.position, //create at the collision point
            rotation);          //face away from the wall

        // add portal as entry or exit point
        portalManager.AddPortal(newPortal.transform);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // hit a wall - create portal and disappear
        // hit (almost) anything else - disappear
        // fly through portal and portal gun
        if (collision.gameObject.CompareTag("Wall"))
            CreatePortal(collision);
        if (!collision.gameObject.CompareTag("Portal") &&
            !collision.gameObject.CompareTag("Player") &&
            !collision.gameObject.CompareTag("PortalGun"))
            Destroy(gameObject);
    }
}
