using UnityEngine;

public class TimeManager : MonoBehaviour
{
    private Timeline timeline; //access to rewind
    private Hero hero;

    // Start is called before the first frame update
    void Start()
    {
        timeline= GetComponent<Timeline>();
        hero = GameObject.Find("Beball").GetComponent<Hero>();
    }

    // Update is called once per frame
    void Update()
    {
        //control time rewind
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            timeline.StartRewind();
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            if (hero.IsAlive())
                timeline.StartRecord();
            else
                timeline.Stop();
        }

    }
}
