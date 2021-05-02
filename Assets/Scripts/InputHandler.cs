using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;


public class InputHandler : MonoBehaviour
{
    
    private PlayerInput playerInput;
    private PlayerControl playerControl;


    private void Awake()
    {
        this.playerInput = GetComponent<PlayerInput>();
        var playerControls = FindObjectsOfType<PlayerControl>();
        
        var index = playerInput.playerIndex;
        this.playerControl = playerControls.FirstOrDefault(pc => pc.playerId == index);
    }


    public void OnMoveRight(CallbackContext ctx)
    {
        if (this.playerControl != null)
            this.playerControl.leftStickX = ctx.ReadValue<float>();
    }


    public void OnMoveLeft(CallbackContext ctx)
    {
        if (this.playerControl != null)
            this.playerControl.leftStickX = -ctx.ReadValue<float>();
    }


    public void OnMoveDown(CallbackContext ctx)
    {
        if (this.playerControl != null)
            this.playerControl.leftStickY = -ctx.ReadValue<float>();
    }


    public void OnFastMove(CallbackContext ctx)
    {
        if (this.playerControl != null && ctx.started)
            this.playerControl.buttonFastMoveDown = true;
    }


    public void OnJump(CallbackContext ctx)
    {
        if (this.playerControl != null)
        {
            if (ctx.started)
            {
                this.playerControl.buttonJumpDown = true;
                this.playerControl.buttonJump = true;
            }
            else if (ctx.canceled)
                this.playerControl.buttonJump = false;
        }
    }


    public void OnAttack(CallbackContext ctx)
    {
        if (this.playerControl != null && ctx.started)
            this.playerControl.buttonAttackDown = true;
    }


    public void OnStrongAttack(CallbackContext ctx)
    {
        if (this.playerControl != null)
        {
            if (ctx.started)
            {
                this.playerControl.buttonStrongAttackDown = true;
                this.playerControl.buttonStrongAttack = true;
            }
            else if (ctx.canceled)
                this.playerControl.buttonStrongAttack = false;
        }
    }


    public void OnThrow(CallbackContext ctx)
    {
        if (this.playerControl != null)
        {
            if (ctx.started)
            {
                this.playerControl.buttonThrowDown = true;
                this.playerControl.buttonThrow = true;
            }
            else if (ctx.canceled)
                this.playerControl.buttonThrow = false;
        }
    }


    public void OnDefend(CallbackContext ctx)
    {
        if (this.playerControl != null)
        {
            if (ctx.started)
            {
                this.playerControl.buttonDefend = true;
            }
            else if (ctx.canceled)
                this.playerControl.buttonDefend = false;
        }
    }
    
}