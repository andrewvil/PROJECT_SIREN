using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    public Vector3 fallVelocity;
    public bool isFalling;

    [SerializeField]
    private float gravMultiplier;

    [SerializeField]
    private CharacterController charController;

    [SerializeField]
    private float walkSpeed = 3f;

    [SerializeField]
    private float runSpeed = 5f;

    [SerializeField]
    private float crouchSpeed = 1.5f;

    private float currMoveSpeed = 0f;

    [SerializeField]
    private float maxStamina=100;
    private float stamina;

    bool canRun;
    bool isCrouching;

    float moveX, moveY;

    [SerializeField]
    private SightController sc;

    [SerializeField]
    private PlayerController pc;
    
    // Start is called before the first frame update
    void Start()
    {
        fallVelocity = Vector3.zero;
        canRun = true;
        isCrouching = false;
        stamina = maxStamina;
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.instance.bGameOver) return;

        HandleCrouching();
        HandlePlayerMovement();

        HandleGravity();

        UIManager.instance.SetStaminaLength(stamina / maxStamina);
    }
    private void HandleGravity()
    {
        charController.Move(fallVelocity * Time.deltaTime);
        if(charController.isGrounded)
        {
            isFalling = false;
        }
        else
        {
            isFalling = true;
        }

        if(isFalling)
        {
            fallVelocity += Physics.gravity * Time.deltaTime * gravMultiplier;
        }
        else
        {
            fallVelocity = new Vector3(0f,-1f,0f);
        }
    }

    [SerializeField]
    float staminaAlpha = 0f;

    float fTime_elapsedBob;
    float fTime_barDuration = 0f;
    private void HandlePlayerMovement()
    {
        if(GameManager.instance.bGameOver) return;

        moveX = Input.GetAxis("Horizontal");
        moveY = Input.GetAxis("Vertical");

        if(GameManager.instance.isInUI)
        {
            moveX = 0f;
            moveY = 0f;
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 60, Time.deltaTime * 20f);
        }

        Vector3 moveDir = Camera.main.transform.forward * moveY + Camera.main.transform.right * moveX;

        moveDir.y = 0f;
        moveDir = moveDir.normalized;

        if(Input.GetKey(KeyCode.LeftShift) && canRun && !isCrouching)
        {
            if(stamina > 0f)
            {
                currMoveSpeed = runSpeed;
                if(moveDir != Vector3.zero)
                {
                    stamina-=Time.deltaTime*20f;
                    staminaAlpha += Time.deltaTime * 5f;
                    if(staminaAlpha > 1f) {
                        staminaAlpha = 1f;
                        fTime_barDuration = 0f;
                    }
                    Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 70, Time.deltaTime * 20f);

                    //UI updates
                    UIManager.instance.SetStaminaAlpha(staminaAlpha);
                }
                else
                {
                    stamina += Time.deltaTime * 5f;
                }
            }
            else
            {
                canRun = false;
                PlayerAudioController.instance.PlayAudio(AUDIOSOUND.TIRED);
            }
        }
        else
        {
            stamina += Time.deltaTime * 15f;
            if(stamina > maxStamina)
            {
                stamina = maxStamina;
                canRun = true;
            }

            currMoveSpeed = isCrouching?crouchSpeed:walkSpeed;

            //ui handling
            if(canRun)
            {
                if(fTime_barDuration > 1f)
                {
                    staminaAlpha -= Time.deltaTime * 5f;
                    if(staminaAlpha < 0f) staminaAlpha = 0f;
                }
                else
                {
                    fTime_barDuration+=Time.deltaTime;
                }
                UIManager.instance.SetStaminaAlpha(staminaAlpha);
            }
            else
            {
                //run out of stamina, blinking stamina bar
                staminaAlpha += Time.deltaTime;
                if(staminaAlpha > 0.3f)
                {
                    UIManager.instance.SetStaminaAlpha(1f);
                    if(staminaAlpha > 0.6f)
                    {
                        staminaAlpha = 0f;
                    }
                }
                else
                {
                    UIManager.instance.SetStaminaAlpha(0f);
                }
            }
            //
            
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 60, Time.deltaTime * 20f);
        }
        moveDir *= currMoveSpeed;

        HandleBobbing(moveDir.magnitude);
        HandleFootsteps(moveDir.magnitude);

        if(GameManager.instance.isInUI) return;
        charController.Move(moveDir * Time.deltaTime);
    }

    private void HandleBobbing(float moveSpeed)
    {
        if (GameManager.instance.isInUI) return;
        if(!charController.isGrounded) return;
        if(pc.adrenalineOn) return; 
        fTime_elapsedBob += Time.deltaTime;

        if(moveSpeed > 0)
            Camera.main.transform.localPosition = Vector3.Lerp(new Vector3(0f,0.5f - 0.05f,0f), new Vector3(0f,(0.5f) + 0.05f,0f), (Mathf.Sin(fTime_elapsedBob * moveSpeed * 4f) + 1) * 0.5f);
        else
            Camera.main.transform.localPosition = Vector3.Lerp(Camera.main.transform.localPosition, new Vector3(0f,0.5f,0f),Time.deltaTime*5f); 
    }

    private void HandleCrouching()
    {
        if(Input.GetKeyDown(KeyCode.LeftControl))
        {
            isCrouching = !isCrouching;
        }
        charController.height = isCrouching?0.9f:1.7f;
    }
    private Coroutine footstepCoroutine;
    private IEnumerator PlayFootstep()
    {
        if(isCrouching || !charController.isGrounded || GameManager.instance.bGameOver)
        {
            footstepCoroutine = null;
            yield break;
        }

        PlayerAudioController.instance.PlayAudio(AUDIOSOUND.FOOTSTEP_LEFT);
        AlertSoundObservers();

        yield return new WaitForSeconds(1.5f/currMoveSpeed);

        if(isCrouching || !charController.isGrounded || GameManager.instance.bGameOver)
        {
            footstepCoroutine = null;
            yield break;
        }

        PlayerAudioController.instance.PlayAudio(AUDIOSOUND.FOOTSTEP_RIGHT);
        AlertSoundObservers();

        yield return new WaitForSeconds(1.5f/currMoveSpeed);

        footstepCoroutine = StartCoroutine(PlayFootstep());
    }
    private void HandleFootsteps(float moveSpeed)
    {
        if(moveSpeed > 0)
        {
            if(footstepCoroutine != null) return;
            footstepCoroutine = StartCoroutine(PlayFootstep());
        }
        else if(footstepCoroutine!=null)
        {
            StopCoroutine(footstepCoroutine);
            footstepCoroutine = null;
        }
    }

    private void AlertSoundObservers()
    {
        foreach(GameObject obj in sc.GetObjectsInRange())
        {
            obj.GetComponent<IAudioObserver>()?.Notify(transform.position,gameObject);
        }
    }
}
