using UnityEngine;

public class Rewindable : MonoBehaviour
{
    protected Timeline timeline; //access to rewind

    // Start is called before the first frame update
    protected void Start()
    {
        timeline = GameObject.Find("TimeManager").GetComponent<Timeline>();
    }

}
