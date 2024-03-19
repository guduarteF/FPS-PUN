using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class NextWeaponUi : MonoBehaviour
{
    public static NextWeaponUi nw;
    public GameObject[] weapon_ui;
    private int next_index , previous_index;

    void Start()
    {
        nw = this;
    }

    void Update()
    {

    }

    public void NextWeapon(int index)
    {
        
        if(index < 28)
        {
             next_index = index + 1;
        }
        
        if(index != 0)
        {
             previous_index = index - 1;
            
        }

        DisableWeaponUI(index);
        DisableWeaponText(index);
        EnableCurrentWeaponUI(index);
        EnableNeighborWeaponUI(next_index, previous_index , index);


        transform.position = new Vector2(transform.position.x, transform.position.y - 145f);

    }

    public void DisableWeaponUI(int current)
    { 
        if(current > 0 && current < 28)
        {
            for (int i = 0; i < 29; i++)
            {
                if (i != current && i != next_index && i != previous_index)
                {
                    weapon_ui[i].transform.GetChild(0).gameObject.SetActive(false);
                }
            }
        }
        else if(current == 0)
        {
            for (int i = 0; i < 29; i++)
            {
                if (i != current && i != next_index )
                {
                    weapon_ui[i].transform.GetChild(0).gameObject.SetActive(false);
                }
            }
        }
        else if(current == 28)
        {
            for (int i = 0; i < 29; i++)
            {
                if (i != current && i != previous_index)
                {
                    weapon_ui[i].transform.GetChild(0).gameObject.SetActive(false);
                }
            }
        }

       
    }
    public void EnableCurrentWeaponUI(int current)
    {
        weapon_ui[current].transform.GetChild(0).gameObject.SetActive(true);
        EnableWeaponText(current);

    

    }

    public void EnableNeighborWeaponUI(int w_next , int w_previous , int current)
    {      

        if (current > 0 && current < 28)
        {
            GameObject child_zero_next = weapon_ui[w_next].transform.GetChild(0).gameObject;
            GameObject child_zero_previous = weapon_ui[w_previous].transform.GetChild(0).gameObject;

            child_zero_next.SetActive(true);
            child_zero_previous.SetActive(true);

            Color newColor_next = child_zero_next.transform.GetChild(0).GetComponent<Image>().color;
            newColor_next.a = 0.5f;

            Color newColor_previous = child_zero_previous.transform.GetChild(0).GetComponent<Image>().color;
            newColor_previous.a = 0.5f;

        }
        else if (current == 0)
        {
            GameObject child_zero_next = weapon_ui[w_next].transform.GetChild(0).gameObject;

            child_zero_next.SetActive(true);
            Color newColor_next = child_zero_next.transform.GetChild(0).GetComponent<Image>().color;
            newColor_next.a = 0.5f;
        }
        else if(current == 28)
        {
            GameObject child_zero_previous = weapon_ui[w_previous].transform.GetChild(0).gameObject;

            child_zero_previous.SetActive(true);
            Color newColor_previous = child_zero_previous.transform.GetChild(0).GetComponent<Image>().color;
            newColor_previous.a = 0.5f;
        }
       
    }

    public void DisableWeaponText(int current)
    {

       

        if (current > 0 && current < 28)
        {
            GameObject w_next = weapon_ui[current + 1].transform.GetChild(0).gameObject;
            GameObject w_previous = weapon_ui[current - 1].transform.GetChild(0).gameObject;

            GameObject w_next_txt1 = w_next.transform.GetChild(1).gameObject;
            GameObject w_next_backg1 = w_next.transform.GetChild(2).gameObject;
            GameObject w_next_txt2 = w_next.transform.GetChild(3).gameObject;
            GameObject w_next_backg2 = w_next.transform.GetChild(4).gameObject;

            GameObject w_previous_txt1 = w_previous.transform.GetChild(1).gameObject;
            GameObject w_previous_backg1 = w_previous.transform.GetChild(2).gameObject;
            GameObject w_previous_txt2 = w_previous.transform.GetChild(3).gameObject;
            GameObject w_previous_backg2 = w_previous.transform.GetChild(4).gameObject;

            // disable next
            w_next_txt1.SetActive(false);
            w_next_backg1.SetActive(false);
            w_next_txt2.SetActive(false);
            w_next_backg2.SetActive(false);

            //disable previous
            w_previous_txt1.SetActive(false);
            w_previous_backg1.SetActive(false);
            w_previous_txt2.SetActive(false);
            w_previous_backg2.SetActive(false);
        }
        else if(current == 0)
        {
            GameObject w_next = weapon_ui[current + 1].transform.GetChild(0).gameObject;

            GameObject w_next_txt1 = w_next.transform.GetChild(1).gameObject;
            GameObject w_next_backg1 = w_next.transform.GetChild(2).gameObject;
            GameObject w_next_txt2 = w_next.transform.GetChild(3).gameObject;
            GameObject w_next_backg2 = w_next.transform.GetChild(4).gameObject;

            // disable next
            w_next_txt1.SetActive(false);
            w_next_backg1.SetActive(false);
            w_next_txt2.SetActive(false);
            w_next_backg2.SetActive(false);
        }
        else if(current == 28)
        {
            GameObject w_previous = weapon_ui[current - 1].transform.GetChild(0).gameObject;

            GameObject w_previous_txt1 = w_previous.transform.GetChild(1).gameObject;
            GameObject w_previous_backg1 = w_previous.transform.GetChild(2).gameObject;
            GameObject w_previous_txt2 = w_previous.transform.GetChild(3).gameObject;
            GameObject w_previous_backg2 = w_previous.transform.GetChild(4).gameObject;

            //disable previous
            w_previous_txt1.SetActive(false);
            w_previous_backg1.SetActive(false);
            w_previous_txt2.SetActive(false);
            w_previous_backg2.SetActive(false);
        }
        
       
       
    }

    public void EnableWeaponText(int current)
    {
        GameObject w_current = weapon_ui[current].transform.GetChild(0).gameObject;
        
        GameObject w_current_txt1 = w_current.transform.GetChild(1).gameObject;
        GameObject w_current_backg1 = w_current.transform.GetChild(2).gameObject;
        GameObject w_current_txt2 = w_current.transform.GetChild(3).gameObject;
        GameObject w_current_backg2 = w_current.transform.GetChild(4).gameObject;

        w_current_txt1.SetActive(true);
        w_current_backg1.SetActive(true);
        w_current_txt2.SetActive(true);
        w_current_backg2.SetActive(true);
    }


}
