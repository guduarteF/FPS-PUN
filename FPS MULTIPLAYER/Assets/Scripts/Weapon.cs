using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Photon.Pun;



public class Weapon : MonoBehaviourPunCallbacks
{
    #region Variables

  

    public Gun[] loadout;
    //public GameObject[] weapon_model;
    [HideInInspector] public Gun currentGunData;


    
    public Transform weaponParent;
    public GameObject bulletHolePrefab;
    public LayerMask canBeShot;
    public AudioSource sfx;
    public AudioClip hitmarkerSound;
    public bool isAiming = false;

    private float currentCooldown;
    public int currentIndex;

    [HideInInspector]
    public GameObject currentWeapon;

    private Image hitmarkerImage;
    private float hitmarkerWait;

    public bool isReloading = false;

    private Color CLEARWHITE = new Color(1, 1, 1, 0);

    private static int somatorio = 0;
    public Material aug_mat , scar_mat , sv_mat , lapua_mat , svd_mat;

    private bool usingPistol;

    private bool ready_to_attack1, ready_to_attack2;

    public GameObject muzzleflash_prefab;
    public GameObject arma_atual_tp;

    //granade launcher
    public GameObject p_granade;
    public GameObject explosion;


    

    #endregion

#region Monobehaviour Callbacks



    // Start is called before the first frame update
    void Start()
    {
      
        foreach (Gun a in loadout) a.Initialize();
        hitmarkerImage = GameObject.Find("HUD/Hitmarker/Image").GetComponent<Image>();
        hitmarkerImage.color = CLEARWHITE;
        Debug.Log("Player spawnou");
        //somatorio = 0;
        Equip(somatorio);


    }

    // Update is called once per frame
    void Update()
    {
        if(!Motion.mot.isdead)
        { 

        if (Pause.paused && photonView.IsMine) return;

            // WEAPON TEST

            //if (photonView.IsMine && Input.GetKeyDown(KeyCode.E))
            //{
            //    somatorio++;
            //    photonView.RPC("Equip", RpcTarget.All, somatorio);

            //}

            //if (photonView.IsMine && Input.GetKeyDown(KeyCode.Q))
            //{
            //    somatorio--;
            //    photonView.RPC("Equip", RpcTarget.All, somatorio);

            //}

            #region Shoot/Attack/Reload

            if (currentIndex == 29)
        {

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                
                StartCoroutine(Atack(0));
            }

            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                StartCoroutine(Atack(1));
            }
        }



            if (currentWeapon != null)
            {
                if (photonView.IsMine)
                {

                    if (currentIndex != 29 && currentIndex != 28)
                    {

                        // 0 = semi // 1 = auto // 2+ = burst
                        if (loadout[currentIndex].burst != 1)
                        {
                            if (Input.GetMouseButtonDown(0) && currentCooldown <= 0 && !isReloading)
                            {
                                if (loadout[currentIndex].FireBullet())
                                {
                                    photonView.RPC("Shoot", RpcTarget.All);

                                    //FirstPerson
                                    ParticleSystem muzzleflash_particles = currentWeapon.transform.Find("Anchor/Design/Gun/Arms/arms_gun/Armature/weapon/Components/Muzzleflash Particles").GetComponent<ParticleSystem>();
                                    ParticleSystem spark_particles = currentWeapon.transform.Find("Anchor/Design/Gun/Arms/arms_gun/Armature/weapon/Components/SparkParticles").GetComponent<ParticleSystem>();

                                    muzzleflash_particles.Play();
                                    spark_particles.Play();

                                    photonView.RPC("ThirdPersonParticle", RpcTarget.All);

                                    //shotguns
                                    if (currentIndex == 8 || currentIndex == 9 || currentIndex == 17)
                                    {
                                        Animator weapon_anim_pump = currentWeapon.transform.Find("Anchor/Design/Gun/Arms/arms_gun").GetComponent<Animator>();
                                        weapon_anim_pump.Play("pump_shoot");
                                    }
                                }
                                else
                                {
                                    StartCoroutine(Reload(loadout[currentIndex].reload));
                                }
                            }
                        }
                        else
                        {
                            if (Input.GetMouseButton(0) && currentCooldown <= 0 && !isReloading)
                            {
                                if (loadout[currentIndex].FireBullet())
                                {

                                    photonView.RPC("Shoot", RpcTarget.All);

                                    //First Person
                                    ParticleSystem muzzleflash_particles = currentWeapon.transform.Find("Anchor/Design/Gun/Arms/arms_gun/Armature/weapon/Components/Muzzleflash Particles").GetComponent<ParticleSystem>();
                                    ParticleSystem spark_particles = currentWeapon.transform.Find("Anchor/Design/Gun/Arms/arms_gun/Armature/weapon/Components/SparkParticles").GetComponent<ParticleSystem>();


                                    muzzleflash_particles.Play();
                                    spark_particles.Play();


                                    photonView.RPC("ThirdPersonParticle", RpcTarget.All);

                                    if (currentIndex == 17)
                                    {
                                        Animator weapon_anim_pump = currentWeapon.transform.Find("Anchor/Design/Gun/Arms/arms_gun").GetComponent<Animator>();
                                        weapon_anim_pump.Play("pump_shoot");


                                    }
                                }
                                else
                                {

                                    StartCoroutine(Reload(loadout[currentIndex].reload));
                                }

                            }
                        }


                        if (Input.GetKeyDown(KeyCode.R) && !gameObject.GetComponent<Motion>().isRunning)
                        {
                            StartCoroutine(Reload(loadout[currentIndex].reload));
                        }



                        //cooldown
                        if (currentCooldown > 0)
                        {
                            currentCooldown -= Time.deltaTime;
                        }

                    }
                    else if (currentIndex == 28)
                    {
                        // lança granada

                        if (loadout[currentIndex].burst != 1)
                        {
                            if (Input.GetMouseButtonDown(0) && currentCooldown <= 0 && !isReloading)
                            {
                                if (loadout[currentIndex].FireBullet())
                                {
                                    //FirstPerson Particle Effect
                                    ParticleSystem muzzleflash_particles = currentWeapon.transform.Find("Anchor/Design/Gun/Arms/arms_gun/Armature/weapon/Components/Muzzleflash Particles").GetComponent<ParticleSystem>();
                                    ParticleSystem spark_particles = currentWeapon.transform.Find("Anchor/Design/Gun/Arms/arms_gun/Armature/weapon/Components/SparkParticles").GetComponent<ParticleSystem>();

                                    SomDeTiro();

                                    muzzleflash_particles.Play();
                                    spark_particles.Play();

                                    //Third person effect 
                                    photonView.RPC("ThirdPersonParticle", RpcTarget.All);

                                    // Granade Launcher Shoot
                                    photonView.RPC("ShootGranadeLauncher", RpcTarget.All);
                                }
                                else
                                {
                                    StartCoroutine(Reload(loadout[currentIndex].reload));
                                }
                            }
                        }

                        //cooldown
                        if (currentCooldown > 0)
                        {
                            currentCooldown -= Time.deltaTime;
                        }

                        if (Input.GetKeyDown(KeyCode.R) && !gameObject.GetComponent<Motion>().isRunning)
                        {
                            StartCoroutine(Reload(loadout[currentIndex].reload));
                        }

                    }


                }
            }
            //Weapon position elasticity
            currentWeapon.transform.localPosition = Vector3.Lerp(currentWeapon.transform.localPosition, Vector3.zero, Time.deltaTime * 4f);
        }

        #endregion

        if (photonView.IsMine)
        {
            if(hitmarkerWait > 0)
            {
                hitmarkerWait -= Time.deltaTime;
            }
            else if(hitmarkerImage.color.a > 0 )
            {
                hitmarkerImage.color = Color.Lerp(hitmarkerImage.color, new Color(1, 1, 1, 0), Time.deltaTime * 0.5f);
            }
        }
       
    }
    #endregion

    #region Private Methods

    #region Reload
    IEnumerator Reload(float p_wait)
    {
        ReloadSound();

        isReloading = true;

        Animator weapon_anim_reload = currentWeapon.transform.Find("Anchor/Design/Gun/Arms/arms_gun").GetComponent<Animator>();

        weapon_anim_reload.SetBool("reload", true);
        weapon_anim_reload.Play("pistol_reload");
  
        if (GetComponent<Motion>().isIdling)
        {
            // GetComponent<Animator>().Play("rifle_reload_idle");
            GetComponent<Animator>().SetBool("Reload_Idle",true);
            GetComponent<Animator>().SetBool("Reload_Run", false);
        }
        else
        {
            //GetComponent<Animator>().Play("rifle_reload_run");
            GetComponent<Animator>().SetBool("Reload_Idle", false);
            GetComponent<Animator>().SetBool("Reload_Run", true);
        }    

        yield return new WaitForSeconds(p_wait);

        if (GetComponent<Motion>().isIdling)
        {
            
            GetComponent<Animator>().SetBool("Reload_Idle", false);
            
        }
        else
        {
           
            GetComponent<Animator>().SetBool("Reload_Run", false);
        }

        weapon_anim_reload.SetBool("reload", false);

        loadout[currentIndex].Reload();
       
        isReloading = false;
        
    }

    #endregion

    #region Equip
    [PunRPC]   
    void Equip(int p_ind)
    {
       
        if (currentWeapon != null)
        {
            if(isReloading) StopCoroutine("Reload");
            Destroy(currentWeapon);
        }

        currentIndex = p_ind;
        GameObject t_newWeapon = Instantiate(loadout[p_ind].prefab, weaponParent.position, weaponParent.rotation, weaponParent);
        t_newWeapon.transform.localPosition = Vector3.zero;
        t_newWeapon.transform.localEulerAngles = Vector3.zero;
        t_newWeapon.GetComponent<Sway>().isMine = photonView.IsMine;
        Model_Weapon_Outside(p_ind);

       

        if (photonView.IsMine)
        {
            ChangeLayerRecursively(t_newWeapon, 8);
        }
        else
        {
            ChangeLayerRecursively(t_newWeapon, 0);
        }

        currentWeapon = t_newWeapon;
        currentGunData = loadout[p_ind];

        Debug.Log(currentIndex);

        if(photonView.IsMine)
        Next_Weapon_UI(currentIndex);

    }

    #endregion

        private void Next_Weapon_UI(int index_atual)
        {
        NextWeaponUi.nw.NextWeapon(index_atual);



        }

    #region Model_Weapon_Outside [ACTIVEorNOT]
    private void Model_Weapon_Outside(int p_ind)
    {

        Transform Right_Hand = transform.Find("mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:RightShoulder/mixamorig:RightArm/mixamorig:RightForeArm/mixamorig:RightHand/Weapon");
        Transform Left_Hand = transform.Find("mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:LeftShoulder/mixamorig:LeftArm/mixamorig:LeftForeArm/mixamorig:LeftHand/Weapon");
        
        if (p_ind == 0)
        {
             GameObject arma_atual = Right_Hand.GetChild(0).gameObject;
            arma_atual_tp = arma_atual;
            arma_atual.SetActive(true);
        }
        else if(p_ind > 0 && p_ind < 29)
        {
            int arma_num = p_ind - 1;          
            GameObject arma_anterior = Right_Hand.GetChild(arma_num).gameObject;          
            arma_anterior.SetActive(false); 
            GameObject arma_atual = Right_Hand.GetChild(p_ind).gameObject;
            arma_atual_tp = arma_atual;
            arma_atual.SetActive(true);
        }
        else
        {
            int arma_num = p_ind - 1;
            GameObject arma_anterior = Right_Hand.GetChild(arma_num).gameObject;
            arma_anterior.SetActive(false);
            GameObject arma_atual = Left_Hand.GetChild(0).gameObject;
            arma_atual.SetActive(true);
        }


    }
    #endregion

    private void ChangeLayerRecursively (GameObject p_target , int p_layer)
    {
        p_target.layer = p_layer;
        foreach (Transform a in p_target.transform)
        {
            ChangeLayerRecursively(a.gameObject, p_layer);

        }
            
    }

    #region AIM
    public bool Aim(bool p_isAiming)
    {     
        if (!currentWeapon) return false;
        if (isReloading) p_isAiming =  false;

        isAiming = p_isAiming;
        Transform t_anchor = currentWeapon.transform.Find("Anchor");
        Transform t_state_ads = currentWeapon.transform.Find("States/ADS");
        Transform t_state_hip = currentWeapon.transform.Find("States/Hip");

        if (p_isAiming)
        {
            //aim

            t_anchor.position = Vector3.Lerp(t_anchor.position, t_state_ads.position, Time.deltaTime * loadout[currentIndex].aimSpeed);
            //t_anchor.eulerAngles = Vector3.Lerp(t_anchor.rotation.eulerAngles, t_state_ads.rotation.eulerAngles, Time.deltaTime * loadout[currentIndex].aimSpeed);

            if(currentIndex == 16 || currentIndex == 15 || currentIndex == 20 || currentIndex == 18 || currentIndex == 19)
            {
                ChangeScopeMatColor(currentIndex, isAiming);

                
                
            }
           

            
        }
        else
        {
            //hip
            t_anchor.position = Vector3.Lerp(t_anchor.position, t_state_hip.position, Time.deltaTime * loadout[currentIndex].aimSpeed);
            //t_anchor.eulerAngles = Vector3.Lerp(t_anchor.rotation.eulerAngles, t_state_hip.rotation.eulerAngles, Time.deltaTime * loadout[currentIndex].aimSpeed);


            if (currentIndex == 16 || currentIndex == 15 || currentIndex == 20 || currentIndex == 18 || currentIndex == 19)
            {
                ChangeScopeMatColor(currentIndex, isAiming);

               

            }
        }

        return p_isAiming;
    }

    #endregion

    #region Shoot
    [PunRPC]
    void Shoot()
    {
        SomDeTiro();

        Transform t_spawn = transform.Find("Cameras/NormalCamera");
        Transform t_spawn_zoom = currentWeapon.transform.Find("Anchor/Design/Gun/Arms/arms_gun/Armature/arm_R/lower_arm_R/hand_R/WEAPON/prefab/scope/Camera");

        //cooldown
        currentCooldown = loadout[currentIndex].firerate;

        for (int i = 0; i < Mathf.Max(1, currentGunData.pellets); i++)
        {          
            // index referente as snipers 

            if (isAiming && currentIndex == 15 || currentIndex == 16 && isAiming || currentIndex == 18 && isAiming  || currentIndex == 20 && isAiming || currentIndex == 19 && isAiming)
            {
                #region Sniper Shoot

                

                //setup bloom
                Vector3 t_bloom = t_spawn_zoom.position + t_spawn_zoom.forward * 1000f;

                //bloom
                t_bloom = t_spawn_zoom.position + t_spawn_zoom.forward * 1000f;
                t_bloom += Random.Range(-loadout[currentIndex].bloom, loadout[currentIndex].bloom) * t_spawn_zoom.up;
                t_bloom += Random.Range(-loadout[currentIndex].bloom, loadout[currentIndex].bloom) * t_spawn_zoom.right;
                t_bloom -= t_spawn_zoom.position;
                t_bloom.Normalize();

               
                //raycast aiming zoom camera on scoped weapons
                RaycastHit t_hit = new RaycastHit();
                if (Physics.Raycast(t_spawn_zoom.position, t_bloom, out t_hit, 1000f, canBeShot))
                {
                    GameObject t_newHole = Instantiate(bulletHolePrefab, t_hit.point + t_hit.normal * 0.001f, Quaternion.identity) as GameObject;
                    t_newHole.transform.LookAt(t_hit.point + t_hit.normal);
                    Destroy(t_newHole, 5f);
                                        

                    if (photonView.IsMine)
                    {
                        //shoting other player on newtwork
                        if (t_hit.collider.gameObject.layer == 9)
                        {
                            

                            //get damage
                            if (t_hit.collider.transform.root.gameObject.GetComponent<Motion>().current_health <= loadout[currentIndex].damage)
                            {
                                somatorio++;
                                photonView.RPC("Equip", RpcTarget.All, somatorio);
                                
                            }

                            //get damage
                            t_hit.collider.transform.root.gameObject.GetPhotonView().RPC("TakeDamage", RpcTarget.All, loadout[currentIndex].damage, PhotonNetwork.LocalPlayer.ActorNumber);

                            //show hitmarker
                            hitmarkerImage.color = Color.white;
                            sfx.PlayOneShot(hitmarkerSound);
                            hitmarkerWait = 1f;
                        }

                        //shooting target
                        if (t_hit.collider.gameObject.layer == 10 || t_hit.collider.gameObject.layer == 17)
                        {
                            //show hitmarker
                            hitmarkerImage.color = Color.white;
                            sfx.PlayOneShot(hitmarkerSound);
                            hitmarkerWait = 1f;
                        }

                    }
                }
                #endregion
            }
            else
            {
                

                //setup bloom
                Vector3 t_bloom = t_spawn.position + t_spawn.forward * 1000f;

                //bloom
                t_bloom = t_spawn.position + t_spawn.forward * 1000f;
                t_bloom += Random.Range(-loadout[currentIndex].bloom, loadout[currentIndex].bloom) * t_spawn.up;
                t_bloom += Random.Range(-loadout[currentIndex].bloom, loadout[currentIndex].bloom) * t_spawn.right;
                t_bloom -= t_spawn.position;
                t_bloom.Normalize();

                //raycast hipfire normal camera
                RaycastHit t_hit = new RaycastHit();
                if (Physics.Raycast(t_spawn.position, t_bloom, out t_hit, 1000f, canBeShot))
                {
                    GameObject t_newHole = Instantiate(bulletHolePrefab, t_hit.point + t_hit.normal * 0.001f, Quaternion.identity) as GameObject;
                    t_newHole.transform.LookAt(t_hit.point + t_hit.normal);
                    Destroy(t_newHole, 5f);
                    

                    if (photonView.IsMine)
                    {
                        //shoting other player on newtwork
                        if (t_hit.collider.gameObject.layer == 9)
                        {
                            if (t_hit.collider.transform.root.gameObject.GetComponent<Motion>().current_health <= loadout[currentIndex].damage)
                            {
                                somatorio++;
                                photonView.RPC("Equip", RpcTarget.All, somatorio);

                            }

                            t_hit.collider.transform.root.gameObject.GetPhotonView().RPC("TakeDamage", RpcTarget.All, loadout[currentIndex].damage, PhotonNetwork.LocalPlayer.ActorNumber);
                            
                            

                            //show hitmarker
                            hitmarkerImage.color = Color.white;
                            sfx.PlayOneShot(hitmarkerSound);
                            hitmarkerWait = 1f;
                        }

                        //shooting target
                        if (t_hit.collider.gameObject.layer == 10 || t_hit.collider.gameObject.layer == 17)
                        {

                            //show hitmarker
                            hitmarkerImage.color = Color.white;
                            sfx.PlayOneShot(hitmarkerSound);
                            hitmarkerWait = 1f;
                        }

                    }
                }
            }
           

            
        }

        //gun fx
        currentWeapon.transform.Rotate(-loadout[currentIndex].recoil, 0, 0);
        currentWeapon.transform.position -= currentWeapon.transform.forward * loadout[currentIndex].kickback;
     

    }

    #endregion

    

    #region TakeDamage
    [PunRPC]
    private void TakeDamage(int p_damage, int p_actor )
    {
            GetComponent<Motion>().TakeDamage(p_damage, p_actor);
        
    }

    #endregion

    #endregion

    #region Public Methods
    public void RefreshAmmo(Text p_text)
    {
        int t_clip = loadout[currentIndex].GetClip();
        int t_stache = loadout[currentIndex].GetStash();

        p_text.text = t_clip.ToString("D2") + " / " + t_stache.ToString("D2");
    }

    #region Change_Scope_Mat_Color
    public void ChangeScopeMatColor(int index , bool aiming)
    {
        if (aiming)
        {
            if (index == 16)
            {
                aug_mat.color = Color.white;
            }
            else if (index == 15)
            {
                scar_mat.color = Color.white;
            }
            else if (index == 20)
            {
                lapua_mat.color = Color.white;
            }
            else if (index == 18)
            {
                sv_mat.color = Color.white;
            }
            else if(index == 19)
            {
                svd_mat.color = Color.white;
            }
        }
        else
        {
            if (index == 16)
            {
                aug_mat.color = Color.black;
            }
            else if (index == 15)
            {
                scar_mat.color = Color.black;
            }
            else if (index == 20)
            {
                lapua_mat.color = Color.black;
            }
            else if (index == 18)
            {
                sv_mat.color = Color.black;
            }
            else if (index == 19)
            {
                svd_mat.color = Color.black;
            }

        }
                 
        
    }
    #endregion

    #region Attack

    [PunRPC]
    public IEnumerator Atack(int mousebutton)
    {
        Animator knife_anim_attack = currentWeapon.transform.Find("Anchor/Design/Gun/Arms/arms_gun").GetComponent<Animator>();
        
        bool atacking1 = false;
        bool atacking2 = false;
       

        if (mousebutton == 0 && !atacking1 && !atacking2)
        {
            if(ready_to_attack1 == false)
            {
                yield return new WaitForSeconds(0.5f);
                ready_to_attack1 = true;
                GetComponent<Motion>().isAttacking_1 = true;
                knife_anim_attack.Play("knife_attack_2");
                photonView.RPC("KnifeCol_Enable", RpcTarget.All);
                atacking1 = true;

                yield return new WaitForSeconds(0.5f);
                photonView.RPC("KnifeCol_Disable", RpcTarget.All);
                atacking1 = false;
                GetComponent<Motion>().isAttacking_1 = false;
                yield return new WaitForSeconds(0.5f);
                ready_to_attack1 = false;

            }     
           
        }
        else if(mousebutton == 1 && !atacking2 && !atacking1)
        {
            if (ready_to_attack2 == false)
            {
                yield return new WaitForSeconds(0.5f);
                ready_to_attack2 = true;
                knife_anim_attack.Play("knife_attack_1");
                photonView.RPC("KnifeCol_Enable", RpcTarget.All);
                atacking2 = true;
                GetComponent<Motion>().isAttacking_2 = true;
                yield return new WaitForSeconds(0.5f);
                photonView.RPC("KnifeCol_Disable", RpcTarget.All);
                atacking2 = false;
                GetComponent<Motion>().isAttacking_2 = false;
                yield return new WaitForSeconds(0.5f);
                ready_to_attack2 = false;
            }
        }
               
    }
    #endregion

    [PunRPC]
    void KnifeCol_Enable()
    {
        GameObject knife = currentWeapon.transform.Find("Anchor/Design/Gun/Arms/arms_gun/Armature/knife").gameObject;
        SphereCollider knife_col = knife.GetComponent<SphereCollider>();
        knife_col.enabled = true;
    }

    [PunRPC]
    void KnifeCol_Disable()
    {
        GameObject knife = currentWeapon.transform.Find("Anchor/Design/Gun/Arms/arms_gun/Armature/knife").gameObject;
        SphereCollider knife_col = knife.GetComponent<SphereCollider>();

        knife_col.enabled = false;
    }

   [PunRPC]

   void ThirdPersonParticle()
    {
        //Third Person
        ParticleSystem muzzleflash_particles_tp = arma_atual_tp.transform.Find("Components/Muzzleflash Particles").GetComponent<ParticleSystem>();
        ParticleSystem spark_particles_tp = arma_atual_tp.transform.Find("Components/SparkParticles").GetComponent<ParticleSystem>();

        muzzleflash_particles_tp.Play();
        spark_particles_tp.Play();
    }

    [PunRPC]
    void ShootGranadeLauncher()
    {
        Transform spawn_granade = gameObject.transform.Find("Weapon").GetChild(0).Find("spawn_granade");

        //cooldown
        currentCooldown = loadout[currentIndex].firerate;

        for (int i = 0; i < Mathf.Max(1, currentGunData.pellets); i++)
        {
           
            // Instanciar  projétil granada
            GameObject granade = Instantiate(p_granade, spawn_granade.position, spawn_granade.rotation);
            if(photonView.IsMine)
            {
                granade.layer = 19;
            }
            else
            {
                granade.layer = 20;
            }

            // Lançar Projetil
            granade.GetComponent<Rigidbody>().AddForce(spawn_granade.forward * 15 ,ForceMode.Impulse);

            //Efeito da Explosão 
            StartCoroutine(ExplosionEffect(granade));

            //Destruir projétil
            Destroy(granade, 3f);

           
        }

        //gun fx
        currentWeapon.transform.Rotate(-loadout[currentIndex].recoil, 0, 0);
        currentWeapon.transform.position -= currentWeapon.transform.forward * loadout[currentIndex].kickback;
        
      
    }

    private IEnumerator ExplosionEffect(GameObject ref_granade)
    {
        yield return new WaitForSeconds(2.6f);
        FindObjectOfType<AudioManager>().PlayOneShot("Projectile Explosion"); 
        Instantiate(explosion, ref_granade.transform.position, Quaternion.identity);      

    }

    #region Som
    private void ReloadSound()
    {
        switch (currentIndex)
        {
            case 0:
                // Mp7
                FindObjectOfType<AudioManager>().PlayOneShot("Reload Pistol");
                break;
            case 1:
                // Mac10
                FindObjectOfType<AudioManager>().PlayOneShot("Reload Pistol");
                break;
            case 2:
                // Thompson
                FindObjectOfType<AudioManager>().PlayOneShot("Reload Sub");
                break;
            case 3:
                // P90
                FindObjectOfType<AudioManager>().PlayOneShot("Reload Sub");
                break;
            case 4:
                // mp5

                FindObjectOfType<AudioManager>().PlayOneShot("Reload Sub");
                break;
            case 5:
                // Vector
                FindObjectOfType<AudioManager>().PlayOneShot("Reload Sub");
                break;
            case 6:
                // Mpx
                FindObjectOfType<AudioManager>().PlayOneShot("Reload Sub");
                break;
            case 7:
                // DoubleBarrel

                FindObjectOfType<AudioManager>().PlayOneShot("Shoot Double Barrel");
                break;
            case 8:
                // Pump

                FindObjectOfType<AudioManager>().PlayOneShot("Reload Shotgun");
                break;
            case 9:
                // Spas 12

                FindObjectOfType<AudioManager>().PlayOneShot("Reload Shotgun");
                break;
            case 10:
                // Sks
                FindObjectOfType<AudioManager>().PlayOneShot("Reload Rifle");
                break;
            case 11:
                // Famas

                FindObjectOfType<AudioManager>().PlayOneShot("Reload Rifle");
                break;
            case 12:
                // Colt

                FindObjectOfType<AudioManager>().PlayOneShot("Reload Rifle");
                break;
            case 13:
                // Ak 47
                FindObjectOfType<AudioManager>().PlayOneShot("Reload Rifle");
                break;
            case 14:
                // Sa-58
                FindObjectOfType<AudioManager>().PlayOneShot("Reload Rifle");
                break;
            case 15:
                // Scar
                FindObjectOfType<AudioManager>().PlayOneShot("Reload Rifle");
                break;
            case 16:
                //  AUG
                FindObjectOfType<AudioManager>().PlayOneShot("Reload Rifle");
                break;
            case 17:
                // Mosin
                FindObjectOfType<AudioManager>().PlayOneShot("Reload Mosin");
                break;
            case 18:
                // Sv 98
                FindObjectOfType<AudioManager>().PlayOneShot("Reload Rifle");
                break;
            case 19:
                // SVD
                FindObjectOfType<AudioManager>().PlayOneShot("Reload Rifle");
                break;
            case 20:
                // Lapua Magnum
                FindObjectOfType<AudioManager>().PlayOneShot("Reload Rifle");
                break;
            case 21:
                // RPK
                FindObjectOfType<AudioManager>().PlayOneShot("Reload Rifle");
                break;
            case 22:
                // M2
                FindObjectOfType<AudioManager>().PlayOneShot("Reload Rifle");
                break;
            case 23:
                // Glockk
                FindObjectOfType<AudioManager>().PlayOneShot("Reload Pistol");
                break;
            case 24:
                // Tec9
                FindObjectOfType<AudioManager>().PlayOneShot("Reload Pistol");
                break;
            case 25:
                // Usp
                FindObjectOfType<AudioManager>().PlayOneShot("Reload Pistol");
                break;
            case 26:
                // Revolver
                FindObjectOfType<AudioManager>().PlayOneShot("Reload Pistol");
                break;
            case 27:
                // Deagle
                FindObjectOfType<AudioManager>().PlayOneShot("Reload Pistol");
                break;
            case 28:
                // Granade Launcher
                FindObjectOfType<AudioManager>().PlayOneShot("Reload Rifle");
                break;




        }



    }


    private void SomDeTiro()
    {
        switch (currentIndex)
        {
            case 0:
                // Mp7
               
                FindObjectOfType<AudioManager>().PlayOneShot("Shoot Mp7");
                break;
            case 1:
                // Mac10
               
                FindObjectOfType<AudioManager>().PlayOneShot("Shoot Mac10");
                break;
            case 2:
                // Thompson
                FindObjectOfType<AudioManager>().PlayOneShot("Shoot MP5");
                break;
            case 3:
                // P90
                FindObjectOfType<AudioManager>().PlayOneShot("Shoot P90");
                break;
            case 4:
                // mp5
               
                FindObjectOfType<AudioManager>().PlayOneShot("Shoot MP5");
                break;
            case 5:
                // Vector
                FindObjectOfType<AudioManager>().PlayOneShot("Shoot Scar");
                break;
            case 6:
                // Mpx
                FindObjectOfType<AudioManager>().PlayOneShot("Shoot M16");
                break;
            case 7:
                // DoubleBarrel
               
                FindObjectOfType<AudioManager>().PlayOneShot("Shoot Double Barrel");
                break;
            case 8:
                // Pump
             
                FindObjectOfType<AudioManager>().PlayOneShot("Shoot Shotgun");
                break;
            case 9:
                // Spas 12
               
                FindObjectOfType<AudioManager>().PlayOneShot("Shoot Spas");
                break;
            case 10:
                // Sks
                FindObjectOfType<AudioManager>().PlayOneShot("Shoot Sks");
                break;
            case 11:
                // Famas
             
                FindObjectOfType<AudioManager>().PlayOneShot("Famas Shoot");
                break;
            case 12:
                // Colt
          
                FindObjectOfType<AudioManager>().PlayOneShot("Shoot M16");
                break;
            case 13:
                // Ak 47
                FindObjectOfType<AudioManager>().PlayOneShot("Shoot Ak-47");
                break;
            case 14:
                // Sa-58
                FindObjectOfType<AudioManager>().PlayOneShot("Shoot Sa-58");
                break;
            case 15:
                // Scar
                FindObjectOfType<AudioManager>().PlayOneShot("Shoot Scar");
                break;
            case 16:
                //  AUG
                FindObjectOfType<AudioManager>().PlayOneShot("Shoot M16");
                break;
            case 17:
                // Mosin
                FindObjectOfType<AudioManager>().PlayOneShot("Shoot Mosin");
                break;
            case 18:
                // Sv 98
                FindObjectOfType<AudioManager>().PlayOneShot("Shoot Awp");
                break;
            case 19:
                // SVD
                FindObjectOfType<AudioManager>().PlayOneShot("Shoot Svd");
                break;
            case 20:
                // Lapua Magnum
                FindObjectOfType<AudioManager>().PlayOneShot("Shoot Lapua");
                break;
            case 21:
                // RPK
                FindObjectOfType<AudioManager>().PlayOneShot("Shoot Rpk");
                break;
            case 22:
                // M2
                FindObjectOfType<AudioManager>().PlayOneShot("Shoot M2");
                break;
            case 23:
                // Glockk
                FindObjectOfType<AudioManager>().PlayOneShot("Shoot Glock");
                break;
            case 24:
                // Tec9
                FindObjectOfType<AudioManager>().PlayOneShot("Shoot Tec9");
                break;
            case 25:
                // Usp
                FindObjectOfType<AudioManager>().PlayOneShot("Shoot Scar");
                break;
            case 26:
                // Revolver
                FindObjectOfType<AudioManager>().PlayOneShot("Shoot Revolver");
                break;
            case 27:
                // Deagle
                FindObjectOfType<AudioManager>().PlayOneShot("Shoot Deagle");
                break;
            case 28:
                // Granade Launcher
                FindObjectOfType<AudioManager>().PlayOneShot("Granade Launcher");
                break;




        }
    }
    #endregion

    #endregion
}
