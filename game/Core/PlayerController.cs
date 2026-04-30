using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fungus;

public class PlayerController : MonoBehaviour
{
    public float turnSmoothing = 15f; 
    public float speedDampTime = 0.1f;

    private Animator anim;              
    private GameObject table;              
    private GameObject popup;
    private Toggle expander;              
    private HashIDs hash;              

    private float h;
    private float v;

    void Awake()
    {
        anim = GetComponent<Animator>();
        table = GameObject.FindGameObjectWithTag("truthTable");
        popup = GameObject.FindGameObjectWithTag("popup");
        expander = GameObject.FindGameObjectWithTag("toggle").GetComponent<Toggle>();
        hash = GameObject.FindGameObjectWithTag("GameController").GetComponent<HashIDs>();
    }

    void FixedUpdate()
    {
        if (!SayDialog.GetSayDialog().isActiveAndEnabled && !MenuDialog.GetMenuDialog().isActiveAndEnabled && !table.activeSelf && !popup.activeSelf && !expander.isOn)
        {
            h = Input.GetAxis("Horizontal");
            v = Input.GetAxis("Vertical");
        }
        else
        {
            h = 0f;
            v = 0f;
        }

        MovementManagement(h, v);
    }

    void Update()
    {
        // AudioManagement();
    }

    void MovementManagement(float horizontal, float vertical)
    {
        if (horizontal != 0f || vertical != 0f)
        {
            Rotating(horizontal, vertical);
            anim.SetFloat(hash.speedFloat, 5.5f, speedDampTime, Time.deltaTime);
        }
        else
            anim.SetFloat(hash.speedFloat, 0);
    }

    void Rotating(float horizontal, float vertical)
    {
        Vector3 targetDirection = new Vector3(horizontal, 0f, vertical);

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);

        Quaternion newRotation = Quaternion.Lerp(GetComponent<Rigidbody>().rotation, targetRotation, turnSmoothing * Time.deltaTime);

        GetComponent<Rigidbody>().MoveRotation(newRotation);
    }

    // void AudioManagement()
    // {
    //     if (anim.GetCurrentAnimatorStateInfo(0).fullPathHash == hash.locomotionState)
    //     {
    //         if (!GetComponent<AudioSource>().isPlaying)
    //             GetComponent<AudioSource>().Play();
    //     }
    //     else
    //         GetComponent<AudioSource>().Stop();
    //
    // }
}
