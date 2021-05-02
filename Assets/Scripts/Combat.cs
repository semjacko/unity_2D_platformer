using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combat : MonoBehaviour
{
    // constants
    private int MAX_COMBO = 3;
    private float COMBO_DELAY = 1.3f;
    private float ATTACK_HIT_DELAY = 0.8f;

    // objects atributes
    private PlayerControl playerControl;
    private Rigidbody2D rb2d;
    private Animator animator;
    private SoundEffect soundEffect;

    // adjustable atributes
    public Transform attackPoint;
    public Transform throwPoint;
    public GameObject throwWeapon;
    public GameObject statsBar;
    public float throwRate = 2f;
    public int maxHealth = 100;
    public float attackRate = 0.4f;
    public int attackDamage = 10;
    public float attackRange = 1f;
    public float maxDefensePoints = 50;
    public float defenseRate = 10f;

    // auxiliary variables
    private float lastThrow = 0f;
    private int currentHealth = 0;
    private float throwStarted = 0f;
    private float throwCanceled = 0f;
    private float currentDefensePoints = 0f;
    private int attackNumber = 0;
    private float lastAttack = 0f;
    private float lastAttackHit = 0f;
    private float strongAttackStarted = 0f;
    private float strongAttackCanceled = 0f;
    private float lastStrongAttack = 0f;
    private float defenseBrokenTime = 0f;
    private int animationAttack = 0;
    private float lastCombo = 0f;


    void Awake()
    {
        // objects initialization
        this.playerControl = this.gameObject.GetComponent<PlayerControl>();
        this.rb2d = this.gameObject.GetComponent<Rigidbody2D>();
        this.animator = this.gameObject.GetComponent<Animator>();
        this.soundEffect = this.gameObject.GetComponent<SoundEffect>();

        // variables initialization
        this.currentHealth = this.maxHealth;
        this.currentDefensePoints = this.maxDefensePoints;
        this.lastThrow = -this.throwRate;       // can throw right at the beginning
        this.lastAttack = -this.attackRate;
        this.lastStrongAttack = -this.attackRate;
        this.defenseBrokenTime = -this.defenseRate;
        this.lastCombo = -COMBO_DELAY;
}


    void Update()
    {
        this.animator.SetBool("IsAvailable", this.playerControl.isAvailable);
        this.animator.SetBool("IsDefending", this.playerControl.buttonDefend && (Time.time - this.defenseBrokenTime >= this.defenseRate));

        // throw down time handle
        if (this.playerControl.buttonThrowDown && Time.time - this.lastThrow >= this.throwRate)
        {
            this.throwStarted = Time.time;
            this.animator.SetBool("IsThrowing", true);
        }
        else if (this.throwStarted > 0 && !this.playerControl.buttonThrow)
        {
            this.throwCanceled = Time.time;
            this.animator.SetBool("IsThrowing", false);
            this.soundEffect.PlayAttack(1);
        }
        this.playerControl.buttonThrowDown = false;

        // strong attack down time handle
        if (this.playerControl.buttonStrongAttackDown && Time.time - this.lastStrongAttack >= this.attackRate)
        {
            this.strongAttackStarted = Time.time;
            this.animator.SetBool("IsStrongAttack", true);
        }
        else if (this.strongAttackStarted > 0 && !this.playerControl.buttonStrongAttack)
        {
            this.strongAttackCanceled = Time.time;
            this.animator.SetBool("IsStrongAttack", false);
        }
        this.playerControl.buttonStrongAttackDown = false;
    }


    void FixedUpdate()
    {
        if (!this.playerControl.isAvailable)
            return;

        this.HandleAttack();
        this.HandleStrongAttack();
        this.HandleThrow();
    }


    private void HandleAttack()
    {
        if (Time.time - this.lastCombo < COMBO_DELAY)
            this.playerControl.buttonAttackDown = false;

        if (this.playerControl.buttonAttackDown && Time.time - this.lastAttack >= this.attackRate)
        {
            this.rb2d.velocity = new Vector2(this.gameObject.transform.right.x * 3f, 0f);
            this.StartCoroutine(this.gameObject.GetComponent<Movement>().IgnoreMovement());

            if (Time.time - this.lastAttackHit >= ATTACK_HIT_DELAY)
                this.attackNumber = 0;

            Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(this.attackPoint.position, this.attackRange, 1 << 12); // 12 player layer

            bool flagAttackHit = false;
            foreach (Collider2D player in hitPlayers)
            {
                if (player.gameObject.GetComponent<PlayerControl>().teamId != this.playerControl.teamId)
                {
                    this.lastAttackHit = Time.time;
                    flagAttackHit = true;

                    if (this.attackNumber == MAX_COMBO - 1)
                    {
                        this.attackNumber = 0;
                        this.lastCombo = Time.time;
                        player.GetComponent<Combat>().TakeDamage(this.attackDamage, new Vector2(this.gameObject.transform.right.x * 5, 5), 1f);
                    }
                    else
                        player.GetComponent<Combat>().TakeDamage(this.attackDamage, Vector2.zero, 0.4f);
                }
            }

            this.animationAttack++;
            this.animator.SetTrigger("IsAttacking");
            this.animator.SetInteger("AttackNumber", this.animationAttack % 2);
            this.soundEffect.PlayAttack(this.animationAttack % 2);

            if (flagAttackHit)
                this.attackNumber++;
            else
                this.attackNumber = 0;

            this.lastAttack = Time.time;
            this.playerControl.buttonAttackDown = false;
        }
    }


    private void HandleStrongAttack()
    {
        if (this.strongAttackCanceled > 0)
        {
            float duration = this.strongAttackCanceled - this.strongAttackStarted;
            if (duration > 2f)
                duration = 2f;

            Collider2D[] hitPlayers = Physics2D.OverlapBoxAll(this.attackPoint.position + new Vector3(this.gameObject.transform.right.x * 0.5f, 0, 0), new Vector2(this.attackRange * 2.5f, 1.6f), 0 , 1 << 12); // 12 player layer
            
            foreach (Collider2D player in hitPlayers)
            {
                if (player.gameObject.GetComponent<PlayerControl>().teamId != this.playerControl.teamId)
                    player.GetComponent<Combat>().TakeDamage((int)(this.attackDamage * duration), new Vector2(this.gameObject.transform.right.x * 5 * duration, 5 * duration), 1f * duration);
            }

            this.rb2d.velocity = new Vector2(this.gameObject.transform.right.x * 7f, 4f);
            this.StartCoroutine(this.gameObject.GetComponent<Movement>().IgnoreMovement());
            this.strongAttackStarted = this.strongAttackCanceled = 0;

            this.lastStrongAttack = Time.time;
        }
    }


    private void HandleThrow()
    {
        if (this.throwCanceled > 0)
        {
            float duration = this.throwCanceled - this.throwStarted;
            GameObject thrownObject = Instantiate(this.throwWeapon, throwPoint.position, throwPoint.rotation);
            thrownObject.GetComponent<ThrowTrajectory>().chargeSec = duration;
            thrownObject.GetComponent<ThrowTrajectory>().playerHorizontalSpeed = this.rb2d.velocity.x;
            thrownObject.GetComponent<ThrowTrajectory>().playerVerticalSpeed = this.rb2d.velocity.y;
            thrownObject.GetComponent<ThrowTrajectory>().teamId = this.playerControl.teamId;

            this.lastThrow = Time.time;
            this.throwStarted = this.throwCanceled = 0;
        }
    }


    public void TakeDamage(int damage, Vector2 force, float secondsDisabled)
    {
        if (this.Defense(damage))
            return;

        this.ResetCounters();
        this.rb2d.velocity = force;
        this.currentHealth -= damage;

        if (this.currentHealth < 0)
            this.currentHealth = 0;

        this.statsBar.GetComponent<BarHandler>().SetHealth((float)this.currentHealth, (float)this.maxHealth);

        // disable controls
        this.playerControl.DisableForSeconds(secondsDisabled);
        this.soundEffect.PlayHurt();

        if (this.currentHealth == 0)
          Die();
    }


    public void Heal(int health)
    {
        this.currentHealth += health;
        if (this.currentHealth > this.maxHealth)
            this.currentHealth = this.maxHealth;
        this.statsBar.GetComponent<BarHandler>().SetHealth((float)this.currentHealth, (float)this.maxHealth);

        this.soundEffect.PlayHeal();
    }


    private bool Defense(int damage)
    {
        if (!this.playerControl.buttonDefend || Time.time - this.defenseBrokenTime < this.defenseRate)
            return false;

        this.currentDefensePoints -= damage;

        if (this.currentDefensePoints < 0)
            this.currentDefensePoints = 0;

        this.statsBar.GetComponent<BarHandler>().SetDefense((float)this.currentDefensePoints, (float)this.maxDefensePoints);

        if (this.currentDefensePoints == 0)
        {
            this.animator.SetBool("IsDefending", false);
            this.defenseBrokenTime = Time.time;
            this.statsBar.GetComponent<BarHandler>().loadDefense(this.defenseRate);
            this.currentDefensePoints = this.maxDefensePoints;
            return false;
        }
        return true;
    }


    private void ResetCounters()
    {
        this.throwStarted = 0;
        this.throwCanceled = 0;
        this.strongAttackStarted = 0;
        this.strongAttackCanceled = 0;
        this.attackNumber = 0;
        this.animator.SetBool("IsThrowing", false);
        this.animator.SetBool("IsStrongAttack", false);
    }

    
    private void Die()
    {
        //this.rb2d.velocity = new Vector2(0, 20f);
        //Destroy(this.gameObject, 1f);
        this.gameObject.GetComponent<PlayerControl>().enabled = false;
        this.gameObject.GetComponent<Movement>().enabled = false;
        this.gameObject.GetComponent<Combat>().enabled = false;
        this.gameObject.GetComponent<BoxCollider2D>().enabled = false;
        this.gameObject.GetComponent<SpriteRenderer>().material.SetFloat("_Thickness", 0f);
        this.animator.SetTrigger("IsDead");

    }






    // POMOCNA METODA NA VYKRESLENIE ATTACK RANGU
    public void OnDrawGizmosSelected()
    {
        Transform attackPoint = this.gameObject.transform.Find("AttackPoint");
        Gizmos.DrawWireSphere(this.attackPoint.position, this.attackRange);
        Gizmos.DrawWireCube(this.attackPoint.position + new Vector3(this.gameObject.transform.right.x * 0.5f, 0 , 0), new Vector3(attackRange * 2.5f, 1.6f, 0));
    }
}
