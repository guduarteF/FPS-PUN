using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;


public class Look : MonoBehaviourPunCallbacks
{
    #region Variables
    public static bool cursorLocked = true;

    public Transform player;
    public Transform weaponCam;
    public Transform normalCam;
    public Transform weapon;

    public float xSensitivity;
    public float ySensitivity;
    public float maxAngle;

    private Quaternion camCenter;

    [SerializeField]
    private float maxSliderAmount = 100.0f;

    #endregion

    #region Monobehaviour Callbacks
    
    void Start()
    {
        camCenter = normalCam.localRotation; // Set rotation origin for cameras to camCenter
    }

    // Update is called once per frame
    void Update()
    {
        Alt_Sense();

        if (!photonView.IsMine)
        {
            return;
        }

        if (Pause.paused) return;

        SetY();
        SetX();
        UpdateCursorLock();

        weaponCam.rotation = normalCam.rotation;
    }

    #endregion

    #region Private Methods
   
    void SetY ()
    {
        float t_input = Input.GetAxis("Mouse Y") * ySensitivity * Time.deltaTime;
        Quaternion t_adj = Quaternion.AngleAxis(t_input , -Vector3.right); // t_adj =  temporary adjustment
        Quaternion t_delta = normalCam.localRotation * t_adj;

        if(Quaternion.Angle(camCenter , t_delta) < maxAngle)
        {
            normalCam.localRotation = t_delta;     
        }

        weapon.rotation = normalCam.rotation;
       
    }

    void SetX()
    {
        float t_input = Input.GetAxis("Mouse X") * xSensitivity * Time.deltaTime;
        Quaternion t_adj = Quaternion.AngleAxis(t_input, Vector3.up);
        Quaternion t_delta = player.localRotation * t_adj;
        player.localRotation = t_delta;
        

    }

    void UpdateCursorLock()
    {
        if(cursorLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            if(Input.GetKeyDown(KeyCode.Escape))
            {
                cursorLocked = false;
            }
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                cursorLocked = true;
            }
        }
    }
    #endregion

   public void Alt_Sense()
   {
        GameObject slider = GameObject.Find("UiSliderControler");
        float valor = slider.GetComponent<t_slider>().w_localvalue;
        xSensitivity = valor;
        ySensitivity = valor;
    }
       
    
}
