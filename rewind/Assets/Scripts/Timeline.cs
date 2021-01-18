using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timeline : MonoBehaviour
{

    enum State
    {
        Idle, 
        Record,
        Replay
    }

    [SerializeField]
    private State state = State.Record;  //recording snapshots

    public List<Snapshot> timeline = new List<Snapshot>(); //used as stack

    private Rigidbody2D rb2d;  //physics engine access
    private Animator animator; //animation engine access

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        //Important step: copy the timeline recording to all children
        foreach (Transform child in transform)
            child.gameObject.AddComponent<Timeline>();
    }

    public void Stop()
    {
        state = State.Idle;
        foreach (Transform child in transform)
            if (child.GetComponent<Timeline>())
                child.GetComponent<Timeline>().Stop(); //do the same to all children
    }

    public bool GetReplay()
    {
        return state == State.Replay;
    }

    public void StartRewind()
    {
        state = State.Replay;

        if (animator != null)
            animator.enabled = false; //pause animation engine, use recorded position

        if (rb2d != null)
            rb2d.isKinematic = true; //pause physics engine, use recorded velocity

        foreach (Transform child in transform)
            if (child.GetComponent<Timeline>())
                child.GetComponent<Timeline>().StartRewind(); //do the same to all children
    }

    public void StartRecord()
    {
        state = State.Record;

        if (animator != null)
            animator.enabled = true; //re-enable animation engine

        if (rb2d != null)
            rb2d.isKinematic = false; //re-enable physics engine

        foreach (Transform child in transform)
            if (child.GetComponent<Timeline>())
                child.GetComponent<Timeline>().StartRecord(); //do the same to all children
    }

    // Update is called once per frame, after all other updates
    private void LateUpdate()
    {
        if (state == State.Replay)
            Rewind(); //pop recorded snaphots
        else if (state == State.Record)
            Record(); //push new snapshots
    }

    private void Record()
    {
        bool alive = false;
        if (tag == "Player")
        {
            Hero hero = transform.GetComponent<Hero>();
            alive = hero.IsAlive();
        }

        //create a new snapshot
        Snapshot s = new Snapshot
        {
            scale = transform.localScale,
            position = transform.position,
            rotation = transform.rotation,

            //for objects that don't use physics engine record velocity=0
            velocity = rb2d != null ? rb2d.velocity : Vector2.zero,

            alive = alive
        };

        //pust to the top of the stack
        timeline.Add(s);
    }

    private void Rewind()
    {
        if (timeline.Count > 0)
        {
            //get snapshot from the top of the stack
            int last = timeline.Count - 1;
            Snapshot s = timeline[last];

            //applay state from snapshot
            transform.position = s.position;
            transform.rotation = s.rotation;
            transform.localScale = s.scale;

            //applay recorded velositty
            if (rb2d != null)
                rb2d.velocity = s.alive ? s.velocity : Vector2.zero;
                

            if (tag == "Player")
            {
                Hero hero = transform.GetComponent<Hero>();
                if (s.alive)
                    hero.Revive();
                else
                    hero.Kill();
            }
            //remove the snapshot from the top of the stack - pop
            if (timeline.Count > 1)
                timeline.RemoveAt(last);
        }
    }
}
