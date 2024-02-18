using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;

public class Motion : MonoBehaviourPunCallbacks, IPunObservable
{
    #region Variables
    public static Motion mot;

    public float speed;
    public float sprintModifier;
    public float crouchModifier;
    public float jumpForce;
    public int max_health;
    public Camera normalCam;
    public Camera weaponCam;
    public GameObject cameraParent;
    public Transform weaponParent;
    public Transform groundDetector;
    public LayerMask ground;

    [HideInInspector] public ProfileData playerProfile;
    public TextMeshPro playerUsername;

    //public float slideAmount;
    public float crouchAmount;

    //public GameObject headsCollider;
    //public GameObject chestCollider;
    //public GameObject legsCollider;
    public GameObject[] body;


    private GameObject arma;


    private Transform ui_healthbar;
    private Text ui_ammo;
    private Text ui_username;

    private Rigidbody rig;

    private Vector3 targetWeaponBobPosition;
    private Vector3 weaponParentOrigin;
    private Vector3 weaponParentCurrentPos;

    private float movementCounter;
    private float idleCounter;

    private float baseFOV;
    
    
    private float sprintFOVModifier = 1.5f;

    private Vector3 origin;

    public int current_health;

    private Manager manager;
    private Weapon weapon;

    private bool crouched;
    //private Vector3 slide_dir;

    private float aimAngle;

    private Vector3 normalCamTarget;
    private Vector3 weaponCamTarget;

    private bool usingPistol;
    private bool usingKnife;

    public bool isIdling;
    private bool isAiming;
    public bool isRunning;
    public bool isAttacking_1 , isAttacking_2;

    private bool running_rifle_forward = false;

    public List<Collider> RagdollParts = new List<Collider>();
    public List<Collider> BoxColParts = new List<Collider>();

    public bool isdead;

    public int currentWeaponIndex;
    #endregion

    #region Photon Callbacks


    public void OnPhotonSerializeView(PhotonStream p_stream, PhotonMessageInfo p_message)
    {
        if (p_stream.IsWriting)
        {
            p_stream.SendNext((int)(weaponParent.transform.localEulerAngles.x * 100f));
        }
        else
        {
            aimAngle = (int)p_stream.ReceiveNext() / 100f;
        }
    }
    #endregion

    #region Monobehaviour Callbacks

    private void Awake()
    {
        mot = this;
        photonView.RPC("SetRagdollParts", RpcTarget.All);
        //SetRagdollParts();

    }

    
    void Start()
    {

        // Reset blood 
        GameObject blood_image = GameObject.Find("Blood");
        Color current_blood_color = blood_image.GetComponent<Image>().color;
        current_blood_color.a = 0;
        blood_image.GetComponent<Image>().color = current_blood_color;

        //references
        manager = GameObject.Find("Manager").GetComponent<Manager>();
        weapon = GetComponent<Weapon>();
        rig = GetComponent<Rigidbody>();

        //set health
        current_health = max_health;

        //camera parent
        cameraParent.SetActive(photonView.IsMine);

        if (!photonView.IsMine)
        {
            gameObject.layer = 9;

            for (int i = 0; i < body.Length; i++)
            {
                body[i].layer = 9;

            }

            
            GameObject Hips = gameObject.transform.Find("mixamorig:Hips").gameObject;
            ChangeLayerRecursively(Hips, 16);
        }
       

        //fov
        baseFOV = normalCam.fieldOfView;
        origin = normalCam.transform.localPosition;
       
        //weapon position
        weaponParentOrigin = weaponParent.localPosition;
        weaponParentCurrentPos = weaponParentOrigin;

        //UI
        #region UI
        if (photonView.IsMine)
        {
            ui_healthbar = GameObject.Find("HUD/Health/Bar").transform;
            ui_ammo = GameObject.Find("HUD/Ammo/Text").GetComponent<Text>();
            ui_username = GameObject.Find("HUD/Username/Text").GetComponent<Text>();
            RefreshHealthBar();
            ui_username.text = Launcher.myProfile.username;

            photonView.RPC("SyncProfile", RpcTarget.All, Launcher.myProfile.username);
        }
        #endregion
    }


    void Update()
    {
       

            if (!photonView.IsMine)
            {
                RefreshMultiplayerState();
                return;
            }

            //Axis
            float t_hmove = Input.GetAxisRaw("Horizontal");
            float t_vmove = Input.GetAxisRaw("Vertical");

            //Controls
            bool sprint = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            bool jump = Input.GetKeyDown(KeyCode.Space);
            bool pause = Input.GetKeyDown(KeyCode.Escape);


            #region States
            //States
            bool isGrounded = Physics.Raycast(groundDetector.position, Vector3.down, 0.5f, ground);
            bool isFlying = !isGrounded;
            bool isJumping = jump && isGrounded;
            bool isSprinting = sprint && t_vmove > 0 && !isJumping && isGrounded && !gameObject.GetComponent<Weapon>().isReloading;
            isRunning = isSprinting;
            // bool isCrouching = crouch && !isSprinting && !isJumping && isGrounded;
            bool isIdle = isGrounded && !isJumping && !isSprinting && t_vmove == 0 && t_hmove == 0;
            bool isWalking = isGrounded && !isJumping && !isSprinting;
            isIdling = isIdle;

            #endregion

            #region UsingPistol / Using Knife
            // Using Pistol
            if (weapon.currentIndex == 0 || weapon.currentIndex == 1 || (weapon.currentIndex > 22 && weapon.currentIndex < 28))
            {
                usingPistol = true;
                GetComponent<Animator>().SetBool("Pistol", true);
            }
            else
            {
                usingPistol = false;
                GetComponent<Animator>().SetBool("Pistol", false);
            }

            //Using Knife
            if (weapon.currentIndex == 29)
            {
                usingKnife = true;
                GetComponent<Animator>().SetBool("Knife", true);
            }
            else
            {
                usingKnife = false;
                GetComponent<Animator>().SetBool("Knife", false);
            }

            #endregion

            //isFlying
            if (isFlying)
            {
                GetComponent<Animator>().SetBool("Flying", true);
            }
            else
            {
                GetComponent<Animator>().SetBool("Flying", false);
            }

            //IsAtacking

            if (isAttacking_1)
            {

                GetComponent<Animator>().SetBool("Attack1", true);

            }
            else
            {
                GetComponent<Animator>().SetBool("Attack1", false);
            }

            if (isAttacking_2)
            {
                GetComponent<Animator>().SetBool("Attack2", true);
            }
            else
            {
                GetComponent<Animator>().SetBool("Attack2", false);
            }

            // anim state      
            #region Animação 

            if (usingPistol)
            {
                if (isSprinting)
                {
                    //   GetComponent<Animator>().Play("pistol_run_forward");
                    GetComponent<Animator>().SetFloat("Speed", 1f);

                }
                else if (t_vmove < 0 && isWalking)
                {
                    // GetComponent<Animator>().Play("pistol_run_forward");
                    GetComponent<Animator>().SetFloat("Speed", -0.5f);


                }
                else if ((t_vmove > 0 || t_hmove > 0 || t_hmove < 0) && isWalking)
                {
                    // GetComponent<Animator>().Play("pistol_run_forward");
                    GetComponent<Animator>().SetFloat("Speed", 0.5f);

                }
                else if (isIdle)
                {
                    //GetComponent<Animator>().Play("pistol_idle");
                    GetComponent<Animator>().SetFloat("Speed", 0f);

                }

            }
            else if (!usingPistol && !usingKnife)
            {

                if (isSprinting)
                {
                    //GetComponent<Animator>().Play("rifle_run_forward_aiming");
                    GetComponent<Animator>().SetFloat("Speed", 1f);

                }
                else if (t_vmove < 0 && isWalking)
                {
                    //GetComponent<Animator>().Play("rifle_run_forward_aiming");
                    GetComponent<Animator>().SetFloat("Speed", -0.5f);


                }
                else if ((t_vmove > 0 || t_hmove > 0 || t_hmove < 0) && isWalking)
                {
                    //GetComponent<Animator>().Play("rifle_run_forward_aiming");
                    GetComponent<Animator>().SetFloat("Speed", 0.5f);

                }
                else if (isIdle)
                {
                    // GetComponent<Animator>().Play("rifle_aiming_idle");
                    GetComponent<Animator>().SetFloat("Speed", 0f);

                }

            }
            else if (usingKnife)
            {

                if (isSprinting)
                {
                    // GetComponent<Animator>().Play("knife_run");
                    GetComponent<Animator>().SetFloat("Speed", 1f);

                }
                else if (t_vmove < 0 && isWalking)
                {
                    // GetComponent<Animator>().Play("knife_run");
                    GetComponent<Animator>().SetFloat("Speed", -0.5f);


                }
                else if ((t_vmove > 0 || t_hmove > 0 || t_hmove < 0) && isWalking)
                {
                    //GetComponent<Animator>().Play("knife_run");
                    GetComponent<Animator>().SetFloat("Speed", 0.5f);

                }
                else if (isIdle)
                {
                    if (!isAttacking_1 && !isAttacking_2)
                    {
                        GetComponent<Animator>().SetFloat("Speed", 0f);
                    }
                    // GetComponent<Animator>().Play("rifle_aiming_idle");


                }


            }



            #endregion


            //Pause
            #region Pause
            if (pause)
            {
                GameObject.Find("Pause").GetComponent<Pause>().TogglePause();
            }

            if (Pause.paused)
            {
                t_hmove = 0f;
                t_vmove = 0f;
                sprint = false;
                jump = false;
                pause = false;
                isGrounded = false;
                isJumping = false;
                isSprinting = false;

            }
            #endregion

            //Jumping
            if (isJumping)
            {
                rig.AddForce(Vector3.up * jumpForce);
            }

            //if (Input.GetKeyDown(KeyCode.U))
            //{
            //    TakeDamage(100, -1);
            //}

            //Head Bob
            #region HeadBob
            if (!isGrounded)
            {
                HeadBob(idleCounter, 0.01f, 0.01f);
                idleCounter += 0;
                weaponParent.localPosition = Vector3.MoveTowards(weaponParent.localPosition, targetWeaponBobPosition, Time.deltaTime * 2f * 0.2f);
            }
            else if (t_hmove == 0 && t_vmove == 0)
            {
                // mirando 
                if (gameObject.GetComponent<Weapon>().isAiming)
                {
                    HeadBob(idleCounter, 0.0f, 0.0f);
                    idleCounter += Time.deltaTime;
                    weaponParent.localPosition = Vector3.MoveTowards(weaponParent.localPosition, targetWeaponBobPosition, Time.deltaTime * 2f * 0.2f);
                }
                else
                {
                    // idling 
                    HeadBob(idleCounter, 0.01f, 0.01f);
                    idleCounter += Time.deltaTime;
                    weaponParent.localPosition = Vector3.MoveTowards(weaponParent.localPosition, targetWeaponBobPosition, Time.deltaTime * 2f * 0.2f);
                }

            }
            else if (!isSprinting && !crouched)
            {
                if (gameObject.GetComponent<Weapon>().isAiming)
                {
                    // walking & aiming
                    HeadBob(idleCounter, 0.002f, 0.002f);
                    idleCounter += Time.deltaTime;
                    weaponParent.localPosition = Vector3.MoveTowards(weaponParent.localPosition, targetWeaponBobPosition, Time.deltaTime * 2f * 0.2f);
                }
                else
                {
                    // walking
                    HeadBob(movementCounter, 0.035f, 0.035f);
                    movementCounter += Time.deltaTime * 3f;
                    weaponParent.localPosition = Vector3.MoveTowards(weaponParent.localPosition, targetWeaponBobPosition, Time.deltaTime * 6f * 0.2f);
                }

            }
            //else if (crouched)
            //{
            //    //crouching
            //    HeadBob(movementCounter, 0.02f, 0.02f);
            //    movementCounter += Time.deltaTime * 1.75f;
            //    weaponParent.localPosition = Vector3.MoveTowards(weaponParent.localPosition, targetWeaponBobPosition, Time.deltaTime * 6f * 0.2f);

            //}
            else
            {
                //sprint           
                HeadBob(movementCounter, 0.075f, 0.075f);
                movementCounter += Time.deltaTime * 5f;
                weaponParent.localPosition = Vector3.MoveTowards(weaponParent.localPosition, targetWeaponBobPosition, Time.deltaTime * 8f * 0.2f);
            }

            //ui Refresh
            RefreshHealthBar();
            weapon.RefreshAmmo(ui_ammo);
        
    }
    #endregion

    private void FixedUpdate()
    {
       
        
            if (!photonView.IsMine) return;
            //Axis
            float t_hmove = Input.GetAxisRaw("Horizontal");
            float t_vmove = Input.GetAxisRaw("Vertical");

            //Controls
            bool sprint = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            bool jump = Input.GetKeyDown(KeyCode.Space);
            bool aim = Input.GetMouseButton(1);

            //States
            bool isGrounded = Physics.Raycast(groundDetector.position, Vector3.down, 0.1f, ground);
            bool isJumping = jump && isGrounded;
            bool isSprinting = sprint && t_vmove > 0 && !isJumping && isGrounded && !gameObject.GetComponent<Weapon>().isReloading;
            bool isAiming = aim && !isSprinting;

            #region isPaused
            if (Pause.paused)
            {
                t_hmove = 0f;
                t_vmove = 0f;
                sprint = false;
                jump = false;
                isGrounded = false;
                isJumping = false;
                isSprinting = false;
                isAiming = false;
            }
        #endregion

        #region Movement speed modifier

        //Movement
        if (!isdead)
        {





            Vector3 t_direction = Vector3.zero;
            float t_adjustedSpeed = speed;

            t_direction = new Vector3(t_hmove, 0, t_vmove);
            t_direction.Normalize();
            t_direction = transform.TransformDirection(t_direction);

            if (isSprinting)
            {
                //if (crouched) photonView.RPC("SetCrouch", RpcTarget.All, false);
                t_adjustedSpeed *= sprintModifier;
            }
            else if (isAiming)
            {
                //crouched or aiming
                t_adjustedSpeed *= crouchModifier;
            }

            Vector3 t_targetVelocity = t_direction * t_adjustedSpeed * Time.deltaTime;
            t_targetVelocity.y = rig.velocity.y;
            rig.velocity = t_targetVelocity;

            #endregion

            //Aiming
            if (weapon.currentIndex != 29)
            {
                isAiming = weapon.Aim(isAiming);
            }

        }
            //Camera Stuff
            //FOV

            #region FOV

            if (isSprinting)
            {
                normalCam.fieldOfView = Mathf.Lerp(normalCam.fieldOfView, baseFOV * sprintFOVModifier, Time.deltaTime * 2f);
                weaponCam.fieldOfView = Mathf.Lerp(normalCam.fieldOfView, baseFOV * sprintFOVModifier, Time.deltaTime * 2f);
            }
            else if (isAiming)
            {
                normalCam.fieldOfView = Mathf.Lerp(normalCam.fieldOfView, baseFOV * weapon.currentGunData.mainFOV, Time.deltaTime * 8f);
                weaponCam.fieldOfView = Mathf.Lerp(normalCam.fieldOfView, baseFOV * weapon.currentGunData.weaponFOV, Time.deltaTime * 8f);
            }
            else
            {
                normalCam.fieldOfView = Mathf.Lerp(normalCam.fieldOfView, baseFOV, Time.deltaTime * 8f);
                weaponCam.fieldOfView = Mathf.Lerp(normalCam.fieldOfView, baseFOV, Time.deltaTime * 8f);
            }

            if (crouched)
            {
                normalCamTarget = Vector3.MoveTowards(normalCam.transform.localPosition, origin + Vector3.down * crouchAmount, Time.deltaTime * 8);
                weaponCamTarget = Vector3.MoveTowards(normalCam.transform.localPosition, origin + Vector3.down * crouchAmount, Time.deltaTime * 16);
            }
            else
            {
                normalCamTarget = Vector3.MoveTowards(normalCam.transform.localPosition, origin, Time.deltaTime * 8);
                weaponCamTarget = Vector3.MoveTowards(normalCam.transform.localPosition, origin, Time.deltaTime * 16);
            }


        
    }

    #endregion

        #region LATEUPDATE
    private void LateUpdate()
    {
      
        normalCam.transform.localPosition = normalCamTarget;
        weaponCam.transform.localPosition = weaponCamTarget;

        //if (!isdead)
        //{
        //    // pegar o script de animação da arma atual equipada
            
        //    if (gameObject.transform.Find("Weapon").GetChild(0).gameObject != null)
        //    {
                
        //        arma = gameObject.transform.Find("Weapon").GetChild(0).gameObject;
        //        arma.GetComponent<weaponAnim>().SprintAnim(isRunning);

        //    }
        //}
    }

  
    #endregion
#endregion

    #region Private Methods

    void RefreshHealthBar()
    {
        float t_health_ratio = (float)current_health / (float)max_health;
        ui_healthbar.localScale = Vector3.Lerp(ui_healthbar.localScale, new Vector3(t_health_ratio, 1, 1), Time.deltaTime * 8f);

    }

    [PunRPC]
    private void SyncProfile(string p_username)
    {
        playerProfile = new ProfileData(p_username);
        playerUsername.text = playerProfile.username;

    }

    #region CROUCH [REMOVED]
    [PunRPC]
    void SetCrouch(bool p_state)
    {
        // FUNCAO REMOVIDA 

        if (crouched == p_state) return;

        crouched = p_state;

        if (crouched)
        {
            //standingCollider.SetActive(false);
            //crouchingCollider.SetActive(true);
            //weaponParentCurrentPos += Vector3.down * crouchAmount;
        }
        else
        {
           // standingCollider.SetActive(true);
            //crouchingCollider.SetActive(false);
           // weaponParentCurrentPos -= Vector3.down * crouchAmount;
        }
    }
    #endregion

    void HeadBob(float p_z, float p_x_intensity, float p_y_intensity)
    {
        float t_aim_adjust = 1f;
        if (isAiming) t_aim_adjust = 0.1f;
        targetWeaponBobPosition = weaponParentCurrentPos + new Vector3(Mathf.Cos(p_z) * p_x_intensity, Mathf.Sin(p_z * 2) * p_y_intensity, 0);
    }


    #endregion




    #region public methods

    #region takedamage
    public void TakeDamage(int p_damage, int p_actor)
    {
        if (photonView.IsMine)
        {
            //current_health -= p_damage;
            photonView.RPC("update_current_health", RpcTarget.All, p_damage);
            RefreshHealthBar();
            Debug.Log(current_health);

            // Blood image 

            GameObject blood_image = GameObject.Find("Blood");
            Color current_blood_color = blood_image.GetComponent<Image>().color;
            current_blood_color.a += .1f;
            blood_image.GetComponent<Image>().color = current_blood_color;

            if (current_health <= 0)
            {
                             
                manager.ChangeStat_S(PhotonNetwork.LocalPlayer.ActorNumber, 1, 1);
               
                if (p_actor >= 0)
                {
                    manager.ChangeStat_S(p_actor, 0, 1);
                    
                }

              
                StartCoroutine(IsDead(p_actor));
                photonView.RPC("TurnOnRagdoll", RpcTarget.All);
                photonView.RPC("TurnOffBoxCol", RpcTarget.All);
                //TurnOnRagdoll();
                //TurnOffBoxCol();

            }
        }

    }
    #endregion
    void RefreshMultiplayerState()
    {
        float cacheEulY = weaponParent.localEulerAngles.y;

        Quaternion targetRotation = Quaternion.identity * Quaternion.AngleAxis(aimAngle, Vector3.right);
        weaponParent.rotation = Quaternion.Slerp(weaponParent.rotation, targetRotation, Time.deltaTime * 8f);

        Vector3 finalRotation = weaponParent.localEulerAngles;
        finalRotation.y = cacheEulY;

        weaponParent.localEulerAngles = finalRotation;
    }
    #endregion

    private void ChangeLayerRecursively(GameObject p_target, int p_layer)
    {
        p_target.layer = p_layer;
        foreach (Transform a in p_target.transform)
        {
            ChangeLayerRecursively(a.gameObject, p_layer);
        }

    }

    [PunRPC]
    public void update_current_health(int damage)
    {
        current_health -= damage;
    }

    [PunRPC]
    public void TurnOnRagdoll()
    {       
        GetComponent<Rigidbody>().useGravity = false;      
        GetComponent<Animator>().enabled = false;

        foreach (Collider c in RagdollParts)
        {
            c.GetComponent<Rigidbody>().isKinematic = false;
            c.GetComponent<Rigidbody>().useGravity = true;
            c.isTrigger = false;
            c.attachedRigidbody.velocity = Vector3.zero;
        }
    }

    [PunRPC]
    private void SetRagdollParts()
    {
        Collider[] colliders = this.gameObject.GetComponentsInChildren<Collider>();

        foreach (Collider c in colliders)
        {
            if (c.gameObject.tag != "BodyParts") // Se o gameobject do collider for diferente do gameobject do player
            {
                c.isTrigger = true;
                c.GetComponent<Rigidbody>().isKinematic = true;
                c.GetComponent<Rigidbody>().useGravity = false;
                RagdollParts.Add(c);
            }
        }
    }

    [PunRPC]
    private void TurnOffBoxCol()
    {
        Collider[] box_colliders = this.gameObject.GetComponentsInChildren<Collider>();

        foreach (Collider c in box_colliders)
        {
            if (c.gameObject.tag == "BodyParts")
            {
                c.isTrigger = true;
                BoxColParts.Add(c);
                if (!photonView.IsMine)
                {
                    c.gameObject.layer = 16;
                }

            }
        }
    }

    IEnumerator IsDead(int actor)
    {       
        playerUsername.gameObject.SetActive(false);
        isdead = true;
        manager.mapcam.SetActive(true);
        normalCam.gameObject.SetActive(false);
        weaponCam.gameObject.SetActive(false);
        yield return new WaitForSeconds(3f);
        manager.mapcam.SetActive(false);
        normalCam.gameObject.SetActive(true);
        weaponCam.gameObject.SetActive(true);
        isdead = false;
        manager.Spawn();
        if (actor >= 0)
        {
            PhotonNetwork.Destroy(gameObject);
        }              

    }
    

    

    


}
