using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class knife_scrpt : MonoBehaviourPunCallbacks
{
    

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
         
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerBody"))
        {
           
            if(photonView.IsMine)
            {
        
                if (other.gameObject.layer == 9)
                {
                   
                    other.transform.root.gameObject.GetPhotonView().RPC("TakeDamage", RpcTarget.All, 100, PhotonNetwork.LocalPlayer.ActorNumber);
                     
                }
                
            }
         

        }
    }

}
