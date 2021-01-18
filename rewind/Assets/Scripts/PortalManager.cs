using UnityEngine;

public class PortalManager : MonoBehaviour
{
    //teleport entry and exit
    public Transform[] portal = new Transform[2];
    private int lastAdded = -1;

    public void AddPortal(Transform portalToAdd)
    {
        if (!portal[0]) //add the 1st portal
        {
            portal[0] = portalToAdd;
            lastAdded = 0;
        }
        else if (!portal[1]) //add the 2nd portal
        {
            portal[1] = portalToAdd;
            lastAdded = 1;
        }
        else //add 3rd and subsequent portal
        { 
            //delete the oldest portal (not last added)
            int replace = lastAdded == 0 ? 1 : 0;
            Destroy(portal[replace].gameObject);

            //add new portal in the freed slot
            portal[replace] = portalToAdd;
            lastAdded = replace;
        }
    }

    public void Teleport(GameObject item, Transform entry)
    {
        //choose the exit portal
        Transform exit = null;
        if (entry == portal[0])
            exit = portal[1];
        else if (entry == portal[1])
            exit = portal[0];

        //do nothing if there's no 2nd portal
        if (entry == exit || !exit)
            return;

        //teleport
        Portal exitPortal = exit.GetComponent<Portal>();
        exitPortal.Ignore(item); //don't teleoprt exiting object

        //change position - use exit portal's position
        item.transform.position = exit.position;

        //change velocity / direction
        Vector3 oldVelocity = Vector3.zero;
        Rigidbody2D rb2d = item.GetComponent<Rigidbody2D>();
        PortalBullet bullet = item.GetComponent<PortalBullet>();
        if (rb2d && !rb2d.isKinematic)
            oldVelocity = rb2d.velocity;
        else if (bullet)
            oldVelocity = bullet.direction;

        //new velocity: use direction of portal but keep magnitude
        Vector3 newVelocity = exit.rotation * Vector3.right;
        newVelocity *= oldVelocity.magnitude;

        //set new velocity
        if (rb2d && !rb2d.isKinematic)
            rb2d.velocity = newVelocity;
        else if (bullet) //no physics here, use direction vector
            bullet.direction = newVelocity;
    }
}
