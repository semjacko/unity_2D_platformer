using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBox : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 12)   // player layer
        {
            Destroy(this.gameObject);
            collision.gameObject.GetComponent<Combat>().Heal(10);
        }
    }
}
