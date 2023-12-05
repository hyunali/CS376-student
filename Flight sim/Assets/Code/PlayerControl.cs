﻿using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Control code for the the player's game object.
/// Very approximate simulation of flight dynamics.
/// </summary>
public class PlayerControl : MonoBehaviour {
    /// <summary>
    /// Coefficient of draft for head winds
    /// </summary>
    [Header("Aerodynamic coefficients")]
    public float ForwardDragCoefficient = 0.01f;
    /// <summary>
    /// Drag coefficient for winds blowing up/down across wings
    /// </summary>
    public float VerticalDragCoefficient = 0.5f;
    /// <summary>
    /// Lift generated by the wings
    /// </summary>
    public float LiftCoefficient = 0.01f;

    /// <summary>
    /// How far the plane can tilt around the X axis
    /// </summary>
    [Header("Movement Speeds")]
    public float PitchRange = 45f;
    /// <summary>
    /// How far the plane can rotate about the Z axis
    /// </summary>
    public float RollRange = 45;
    /// <summary>
    /// How fast the plane yaws for a given degree of roll.
    /// </summary>
    public float RotationalSpeed = 5f;
    /// <summary>
    /// Thrust generated when the throttle is pulled back all the way.
    /// </summary>
    public float MaximumThrust = 20f;

    // can change this
    [Range(0.01f, 1.0f)] 
    public float lerp_weight = 0.5f;

    /// <summary>
    /// Text element for displaying status information
    /// </summary>
    [Header("HUD")]
    public Text StatusDisplay;
    /// <summary>
    /// Text element for displaying game-over text
    /// </summary>
    public Text GameOverText;

    /// <summary>
    /// Cached copy of the player's RigidBody component
    /// </summary>
    private Rigidbody playerRB;


    /// <summary>
    /// Magic layer mask code for the updraft(s)
    /// </summary>
    const int UpdraftLayerMask = 1 << 8;


    #region Internal flight state
    /// <summary>
    /// Current yaw (rotation about the Y axis)
    /// </summary>
    private float yaw;
    /// <summary>
    /// Current pitch (rotation about the X axis)
    /// </summary>
    private float pitch;
    /// <summary>
    /// Current roll (rotation about the Z axis)
    /// </summary>
    private float roll;
    /// <summary>
    /// Current thrust (forward force provided by engines
    /// </summary>
    private float thrust;
#endregion

    /// <summary>
    /// Initialize component
    /// </summary>
    internal void Start() {
        playerRB = GetComponent<Rigidbody>();
        playerRB.velocity = transform.forward*3;

        pitch = 0f; // initialising values...
        yaw = 0f;
        roll = 0f;
        thrust = 0f;

    }

    private void FixedUpdate()
    {
        // update pitch and roll
            // update *roll* using 'horizontal' axis ranging from (-rollRange,+rollRange)
            // update *pitch* using 'vertical' axis ranging from (-pitchRange,+pitchRange)
            
        float joystickRoll = Input.GetAxis("Horizontal");
        float joystickPitch = Input.GetAxis("Vertical");
        float thrustJoystick = Input.GetAxis("Thrust");
        

        joystickRoll = joystickRoll * RollRange;
        roll = Mathf.Lerp(roll, joystickRoll, lerp_weight);
        roll = Mathf.Clamp(roll, -1 * RollRange, RollRange);

        joystickPitch = joystickPitch * PitchRange;
        pitch = Mathf.Lerp(pitch, pitch + joystickPitch, lerp_weight);
        pitch = Mathf.Clamp(pitch, -1 * PitchRange, PitchRange);
        
        // calculate yaw -- note: d/dt(yaw) = roll * rotationSpeed
        
        // yaw = -1*(roll * RotationalSpeed);
        yaw = Mathf.Lerp(yaw, yaw + (-1*roll * RotationalSpeed), lerp_weight);
        
        // use moveRotation() method to update where the plane is pointing
        // i.e. rigidBody.MoveRotation(rigidBody.rotation * change)
        // first, make the quaternion for the desired rotation, then update
        // i.e. Quaternion deltaRotation = Quaternion.Euler(pitch,yaw,roll)
        
        Quaternion deltaRotation = Quaternion.Euler(pitch, yaw, roll);
        playerRB.MoveRotation(deltaRotation);   // updating rotation
        
        // calculate thrust
        // apply a force to forward direction of plane proportional to value
        //  of the 'thrust' axis
        // note: keep this clamped above 0 (no negative thrust)

        thrust += thrustJoystick;
        thrust = Mathf.Clamp(thrust, 0, MaximumThrust);

        playerRB.velocity = transform.forward * thrust;

    }

    /// <summary>
    /// Show game-over display
    /// </summary>
    /// <param name="safe">True if we won, false if we crashed</param>
    private void OnGameOver(bool safe) {
        playerRB.velocity = Vector3.zero;
        playerRB.useGravity = false;
        playerRB.constraints = RigidbodyConstraints.FreezeAll;
        if (safe) {
            GameOverText.text = "You Win!";
        } else {
            GameOverText.text = "OOPS";
        }
    }

    /// <summary>
    /// Display status information
    /// </summary>
    internal void OnGUI()
    {
        StatusDisplay.text = string.Format("Speed: {0:00.00}    altitude: {1:00.00}    Thrust {2:0.0}",
            playerRB.velocity.magnitude,
            transform.position.y,
            thrust);
    }
}
