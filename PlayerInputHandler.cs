using UnityEngine;
using KinematicCharacterController;

/*
 * Changelog
 * 3/10 - Added joystick support, added support for 2 player (one on mouse+keyboard, other on controller)
 * 2/23 - Added shield functionality. Now can't attack with shield up.
 */

public class PlayerInputHandler : MonoBehaviour{

    public PlayerCharacterController Character;
    public bool isUsingController;
    public PlayerMeleeAttack meleeAttack;
    public DirectionalEnergyShield directionalEnergyShield;
    public PlayerRangedAttack rangedAttack;
    public PlayerAimedShot aimedShot;
    public PlayerFireballAttack fireballAttack;


    //TODO move to its own script
    public GameObject animatorContainer;
    private Animator animator;
    public RuntimeAnimatorController idle;
    public RuntimeAnimatorController walking;
    //True when the shield or aim button is pressed, false when released;
    private bool shieldOrAimButtonDown = false;

    private void Start(){

        //TODO lock cursor when building, annoying for testing
        //Cursor.lockState = CursorLockMode.None;
        //Cursor.visible = false;

        animator = animatorContainer.GetComponent<Animator>();
        
    }

    private void Update(){
        HandleCharacterInput();

    }

    private void LateUpdate(){

    }

    private void HandleCharacterInput(){

        PlayerCharacterInputs characterInputs = new PlayerCharacterInputs();

        float vertical;
        float horizontal;
        float verticalLook;
        float horizontalLook;

        if (isUsingController) {
            vertical = Input.GetAxisRaw("VerticalP2");
            horizontal = Input.GetAxisRaw("HorizontalP2");
            verticalLook = Input.GetAxisRaw("VerticalRightStickP2");
            horizontalLook = Input.GetAxisRaw("HorizontalRightStickP2");
            //characterInputs.JumpDown = Input.GetButtonDown("JumpP2");
            characterInputs.Dash = Input.GetButtonDown("JumpP2");
        } else {
            vertical = Input.GetAxisRaw("Vertical");
            horizontal = Input.GetAxisRaw("Horizontal");
            verticalLook = Input.mousePosition.y;
            horizontalLook = Input.mousePosition.x;
            //characterInputs.JumpDown = Input.GetButtonDown("Jump");
            characterInputs.Dash = Input.GetButtonDown("Jump");
        }

        if (aimedShot == null || (aimedShot != null && !aimedShot.aimedShotActive)) {
            characterInputs.MoveAxisForward = vertical;
            characterInputs.MoveAxisRight = horizontal;
        } else {
            characterInputs.MoveAxisForward = 0;
            characterInputs.MoveAxisRight = 0;
        }
        characterInputs.LookAxisForward = verticalLook;
        characterInputs.LookAxisRight = horizontalLook;


        changeAnimatorState(vertical, horizontal);

        // characterInputs.JumpDown = Input.GetKeyDown(KeyCode.Space);
   //     characterInputs.Dash = Input.GetKeyDown(KeyCode.LeftControl);
  //      characterInputs.CrouchDown = Input.GetKeyDown(KeyCode.C);
   //     characterInputs.CrouchUp = Input.GetKeyUp(KeyCode.C);

        // Apply inputs to character
        Character.SetInputs(ref characterInputs);

        //Process other inputs not handled by the character controller
        if ((!isUsingController && Input.GetButton("Fire1")) || (isUsingController && Input.GetButton("Fire1P2")) ) { // GetButton fires as long as the button is held
         //   Debug.Log("is controller: " + isUsingController + " " + Input.GetButton("Fire1P2"));
            //shield player can't attack if shield is raised
            if (directionalEnergyShield != null && !directionalEnergyShield.shieldActive) {
                if (meleeAttack != null) {
                    meleeAttack.Attack();
                }
            }

            if (aimedShot == null || (aimedShot != null && !aimedShot.aimedShotActive)) {
                if(rangedAttack != null) {
                    rangedAttack.shoot();
                }
            }
        }

        if (!shieldOrAimButtonDown && ((!isUsingController && Input.GetButtonDown("Fire2")) || (isUsingController && Input.GetAxis("Fire2AxisP2")> .1f))) {
            if (directionalEnergyShield != null) {
                shieldOrAimButtonDown = true;
                directionalEnergyShield.activateShield();
            }
            if (aimedShot != null) {
                shieldOrAimButtonDown = true;
                aimedShot.initiateShot();
            }
        }
        if ((!isUsingController && Input.GetButtonUp("Fire2")) || (isUsingController && Input.GetAxis("Fire2AxisP2") < .1f)) {
            if (directionalEnergyShield != null) {
                shieldOrAimButtonDown = false;
                directionalEnergyShield.deactivateShield();
            }
            if (aimedShot != null) {
                shieldOrAimButtonDown = false;
                aimedShot.disengageShot();
            }
        }

        // TODO test with controller
        if(!isUsingController && Input.GetButton("Fireback") || (isUsingController && Input.GetButton("FirebackP2"))){
            fireballAttack.fire();
        }
    }

    //TODO move to its own script
    private void changeAnimatorState(float VerticalInput, float HorizontalInput) {
        const float MIN_MOVEMENT = .02f;
        float VertMag = Mathf.Abs(VerticalInput);
        float HorzMag = Mathf.Abs(HorizontalInput);
        if (VertMag > MIN_MOVEMENT || HorzMag > MIN_MOVEMENT) {
            animator.runtimeAnimatorController = walking;
        } else {
            animator.runtimeAnimatorController = idle;
        }
    }
}
