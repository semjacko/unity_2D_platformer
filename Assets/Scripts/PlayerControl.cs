using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    // objects atributes
    private Rigidbody2D rb2d;
    private SpriteRenderer spriteRenderer;

    // adjustable atributes
    public int playerId = 0;
    public int teamId = 0;

    // freez
    public bool isAvailable { get; private set; }
    private float enableTime = 0f;

    // controller
    private float _leftStickX;
    private float _leftStickY;
    private bool _buttonJumpDown;
    private bool _buttonJump;
    private bool _buttonThrowDown;
    private bool _buttonThrow;
    private bool _buttonAttackDown;
    private bool _buttonStrongAttackDown;
    private bool _buttonStrongAttack;
    private bool _buttonFastMoveDown;
    private bool _buttonDefend;

    public float leftStickX
    {
        get { return this._leftStickX; }
        set
        {
            if (!this.buttonDefend && !this.buttonStrongAttack && !this.buttonAttackDown)
                this._leftStickX = value;
        }
    }

    public float leftStickY
    {
         get { return this._leftStickY; }
         set
         {
             if (!this.buttonDefend && !this.buttonStrongAttack)
                 this._leftStickY = value;
         }
    }

    public bool buttonJumpDown
    {
        get { return this._buttonJumpDown; }
        set
        {
            if (!this.buttonDefend && !this.buttonStrongAttack)
                this._buttonJumpDown = value;
        }
    }

    public bool buttonJump
    {
        get { return this._buttonJump; }
        set
        {
            if (!this.buttonDefend && !this.buttonStrongAttack)
                this._buttonJump = value;
        }
    }

    public bool buttonThrowDown
    {
        get { return this._buttonThrowDown; }
        set
        {
            if (!this.buttonDefend && !this.buttonStrongAttack)
                this._buttonThrowDown = value;
        }
    }

    public bool buttonThrow
    {
        get { return this._buttonThrow; }
        set
        {
            if (!this.buttonDefend && !this.buttonStrongAttack)
                this._buttonThrow = value;
        }
    }

    public bool buttonAttackDown
    {
        get { return this._buttonAttackDown; }
        set
        {
            if (value)
                this.leftStickX = 0;

            if (!this.buttonDefend && !this.buttonStrongAttack)
                this._buttonAttackDown = value;
        }
    }

    public bool buttonStrongAttackDown
    {
        get { return this._buttonStrongAttackDown; }
        set
        {
            if (!this.buttonDefend && !this.buttonThrow)
                this._buttonStrongAttackDown = value;
        }
    }

    public bool buttonStrongAttack
    {
        get { return this._buttonStrongAttack; }
        set
        {
            if (this.buttonDefend || this.buttonThrow)
                return;

            if (value)
            {
                this.leftStickX = 0;
                this.leftStickY = 0;
                this.buttonJump = false;
            }
            this._buttonStrongAttack = value;
        }
    }

    public bool buttonFastMoveDown
    {
        get { return this._buttonFastMoveDown; }
        set
        {
            if (!this.buttonDefend && !this.buttonStrongAttack)
                this._buttonFastMoveDown = value;
        }
    }

    public bool buttonDefend
    {
        get { return this._buttonDefend; }
        set
        {
            if (this.buttonThrow || this.buttonStrongAttack)
                return;

            if (value)
            {
                this.leftStickX = 0;
                this.leftStickY = 0;
                this.buttonJump = false;
            }
            this._buttonDefend = value;
        }
    }


    void Awake()
    {
        // objects initialization
        this.rb2d = this.gameObject.GetComponent<Rigidbody2D>();
        this.spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
        this.isAvailable = true;

        // outline initialization
        spriteRenderer.material.SetColor("_SolidOutline", this.SetColor(this.playerId));
        spriteRenderer.material.SetFloat("_Thickness", 2f);
    }


    void Update()
    {
        if (this.enableTime <= Time.time)
            this.isAvailable = true;
    }
    

    private Color SetColor(int playerId)
    {
        switch (playerId)
        {
            case 0: return Color.magenta;
            case 1: return Color.green;
            case 2: return Color.red;
            case 3: return Color.yellow;
        }

        return Color.white;
    }


    public void DisableForSeconds(float secs)
    {
        this.isAvailable = false;
        if (this.enableTime < Time.time + secs)
            this.enableTime = Time.time + secs;
    }


    /*
    void Grow()
    {
        transform.localScale *= 1.1f;
    }
    */

}
