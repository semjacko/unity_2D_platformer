using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ThrowTrajectory : MonoBehaviour
{

    // constants
    private const float defaultSpeedPartition = 0.3f;
    private const float rotationSpeed = 5f;
    private const float maxChargeSec = 0.7f;
    private const float minSpeedToDamage = 4f;

    // adjustable variables
    public int damage = 10;
    public float maxHorizontalSpeed = 15f; 
    public float maxVerticalSpeed = 6f;

    // auxiliary variables
    private Rigidbody2D rb2d;
    public float chargeSec { get; set; }
    public float playerHorizontalSpeed { get; set; }
    public float playerVerticalSpeed { get; set; }
    public int teamId { get; set; }


    void Awake()
    {
        this.rb2d = this.gameObject.GetComponent<Rigidbody2D>();
    }


    void Start()
    {
        float chargeMultiplier = this.chargeSec / maxChargeSec;
        if (chargeMultiplier > 1)   
            chargeMultiplier = 1;
        chargeMultiplier *= 1 - defaultSpeedPartition;

        float horizontalSpeed = this.gameObject.transform.right.x * (this.maxHorizontalSpeed * (defaultSpeedPartition + chargeMultiplier) + Mathf.Abs(this.playerHorizontalSpeed));
        float verticalSpeed = this.maxVerticalSpeed * (defaultSpeedPartition + chargeMultiplier) + this.playerVerticalSpeed;
        this.rb2d.velocity = new Vector2(horizontalSpeed, verticalSpeed);
        this.rb2d.AddTorque(this.gameObject.transform.right.x * -rotationSpeed, ForceMode2D.Force);
    }


    void FixedUpdate()
    {
        if (this.rb2d.velocity == Vector2.zero)
            Destroy(this.gameObject);
    }

    
    void OnCollisionEnter2D(Collision2D collision)
    {
        Collider2D collider = collision.collider;
        print(collider.gameObject.layer);
        if (collider.gameObject.layer != 12)   // player layer == 12
            return;

        Combat player = collider.GetComponent<Combat>();
        float velocity_vector = Mathf.Abs(this.rb2d.velocity.x) + Mathf.Abs(this.rb2d.velocity.y);
        if (player.gameObject.GetComponent<PlayerControl>().teamId != this.teamId && velocity_vector > minSpeedToDamage)
        {
            player.TakeDamage(this.damage, Vector2.zero, 0.5f);
            Destroy(this.gameObject);
        }
    }
}
