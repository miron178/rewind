using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    private Timeline timeline; //access to rewind
    private PortalManager portalManager;
    private List<GameObject> ignored = new List<GameObject>();

    void Awake()
    {
        portalManager = GameObject.Find("PortalManager").GetComponent<PortalManager>();
        timeline = GameObject.Find("TimeManager").GetComponent<Timeline>();
    }

    public void Ignore(GameObject item)
    {
        if (timeline.GetReplay())
            return;

        ignored.Add(item);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (timeline.GetReplay())
            return;
        if (!collision.CompareTag("Player") && !collision.CompareTag("PortalBullet"))
        {
            return;
        }
        if (ignored.Contains(collision.gameObject))
        {
            ignored.Remove(collision.gameObject);
        }
        else
        {
            portalManager.Teleport(collision.gameObject, transform);
        }
    }
}
