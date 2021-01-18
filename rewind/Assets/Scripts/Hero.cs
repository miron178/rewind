using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : Rewindable
{
    //[HideInInspector] 
    public float run = 0f; //horizontal axis from controller (-1 to 1)
    //[HideInInspector] 
    public bool jump = false; //jump button pressed 

    public float moveForce = 365f;  //horizontal movement force
    public float maxSpeed = 5f;     //horizontal speed limit
    public float jumpForce = 1000f; //vertical movement force

    public Transform groundCheck; //check if player is on ground or in the air

    private Animator anim;     //access to animation (Unity)
    private Rigidbody2D rb2d;  //access to physics engine (Unity)
    private bool alive = true;

    [SerializeField]
    private GameObject rewindSignPrefab; //"Shift" prefab
    private GameObject rewindSign; //"Shift" object, if present
    [SerializeField]
    private Vector3 rewindSignPosition = new Vector3(0.4f, 0.1f, 0.0f);

    [SerializeField]
    private PortalGun portalGun;

    new void Start()
    {
        base.Start();
        anim = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
     }

    // Update is called once per frame
    void Update()
    {
        //control player if not in time repaly
        if (!timeline.GetReplay() && alive)
        {
            run = Input.GetAxis("Horizontal"); //run speed
            if (Input.GetButtonDown("Jump"))   //record jump button press
                jump = true;
            if (Input.GetButtonDown("Fire1"))   //record fire button press
                portalGun.Fire(Vector3.right * transform.localScale.x);
        }
        else
        {
            run = 0f;
            jump = false;
        }
    }

    //FixedUpdate is called 50 times per second, regardless of Update speed
    private void FixedUpdate()
    {
        if (timeline.GetReplay() || !alive)
            return; //nothing to do, just replying the past

        //check if on ground using the "Ground" layer
        bool grounded = Physics2D.Linecast(
            transform.position,
            groundCheck.position,
            1 << LayerMask.NameToLayer("Ground"));

        if (!grounded)
        {
            jump = false; //no double jumping
            run *= 0.2f; //slower turn speed when in air
        }

        //switch to run animation if moving
        anim.SetFloat("Speed", Mathf.Abs(run));

        Flip(run);
        Run(run);
        if (jump) //jump button press was seen in Update()
        {
            Jump();
            jump = false; //wait for the next jump press
        }
    }

    private void LateUpdate()
    {
        if (rewindSign)
            rewindSign.transform.position = transform.position + rewindSignPosition;
    }

    //move horizontally
    void Run(float speed)
    {
        rb2d.AddForce(Vector2.right * speed * moveForce);

        //limit max speed
        if (Mathf.Abs(rb2d.velocity.x) > maxSpeed)
            rb2d.velocity = new Vector2(
                Mathf.Sign(rb2d.velocity.x) * maxSpeed, //keep the direction, sign = +1 or -1
                rb2d.velocity.y);
    }

    //move vertically
    void Jump()
    {
        anim.SetTrigger("Jump");
        rb2d.AddForce(new Vector2(0, jumpForce));
    }

    //flip player character left or right
    void Flip(float direction)
    {
        if (System.Math.Abs(direction) > 0f)
        {
            Vector3 scale = transform.localScale;
            //change sign, keep value
            scale.x = Mathf.Abs(scale.x) * Mathf.Sign(direction); 
            transform.localScale = scale;
        }
    }

    public bool IsAlive()
    {
        return alive;
    }

    public void Kill()
    {
        if (alive && !timeline.GetReplay())
        {
            anim.SetTrigger("Die");
            alive = false;

            // show the rewind sign
            rewindSign = Instantiate(rewindSignPrefab, 
                transform.position + rewindSignPosition, 
                Quaternion.identity);
            // can't make the rewindSign child of the hero beause when moving
            // left the hero uses a mirror image so letters would be unreadable
            //rewindSignObject.transform.SetParent(transform);
        }
    }

    public void Dead()
    {
        timeline.Stop();
        Time.timeScale = 0;
    }

    public void Revive()
    {
        if (!alive)
        {
            Time.timeScale = 1;
            anim.SetTrigger("Rewind");
            alive = true;

            // Make the rewind sign disappear
            rewindSign.GetComponent<Animator>().SetTrigger("Rewind");
        }
    }
}
