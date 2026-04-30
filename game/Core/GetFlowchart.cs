using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

public class GetFlowchart : MonoBehaviour
{
    public static Flowchart Instance;

    private void Awake()
    {
        Instance = GetComponent<Flowchart>();
    }
}
