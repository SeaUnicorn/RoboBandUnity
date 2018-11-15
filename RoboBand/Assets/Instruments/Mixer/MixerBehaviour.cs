﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MixerBehaviour : MonoBehaviour {

    /**
        * <value> PanelMixer - a editor menu </value>
        */
    public GameObject PanelMixer;

    /**
     * <value> a method called once per frame // checks if user press the guitar (fender)</value>
     */
    public void OnMouseDown()
    {
        //if you press on mixer...
        if (!GameObject.Find("Canvas").GetComponent<HUD>().edit && Input.GetMouseButtonDown(0))
        {
            Debug.Log("Mixer was pushed");
            //it becomes bigger
            gameObject.transform.localScale *= 1.5f;
            GameObject.Find("Canvas").GetComponent<HUD>().edit = true;
            //editor panel becomes active
            PanelMixer.SetActive(true);
        }

    }
}
