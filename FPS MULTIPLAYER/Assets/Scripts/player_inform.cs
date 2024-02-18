using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_inform : MonoBehaviourPunCallbacks
{
    public Manager m;
    public PhotonView pv;
    private int[] Id;
    public GameObject[] players_go;

    private void Update()
    {
        //for (int i = 0; i < playerInfo.Count; i++)
        //{

        //}

        //Debug.Log("Qtd de players: " + m.playerInfo.Count);

        //Id[0] = GameObject.Find("Player(Clone)").gameObject.GetComponent<PhotonView>().ViewID;
        //Debug.Log("Meu id é : " + Id);
        //foreach (PlayerInfo a in m.playerInfo)
        //{
        //    Debug.Log(m.playerInfo[0].kills);
        //}

        //foreach (PlayerInfo a in m.playerInfo)
        //{
        //    Debug.Log("meu ID é : " + a.actor);
        //}

        //StartCoroutine(delay());






    }
    private void Start()
    {
        m = m.GetComponent<Manager>();
        
       


    }

    public IEnumerator delay()
    {
        yield return new WaitForSeconds(3);
        // inicializa array
        GameObject[] players = new GameObject[m.playerInfo.Count];

        for (int i = 0; i < m.playerInfo.Count;)
        {
            if (players[i] == null)
            {
               
                players[i] = GameObject.Find("Player(Clone)");
                Debug.Log("Player: " + players[i] + "Na posição : " + i + "Id = " + players[i].GetComponent<Motion>().playerProfile.username);

            }
            i++;
        }

    }


}
