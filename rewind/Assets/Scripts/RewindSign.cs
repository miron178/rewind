using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewindSign : MonoBehaviour
{
    public void Destroy()
    {
        // destroy the game object that is parent to this behaviour
        Object.Destroy(this.gameObject);
    }
}
