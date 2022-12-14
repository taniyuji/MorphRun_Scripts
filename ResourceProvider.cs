using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ResourceProvider : MonoBehaviour
{
    public static ResourceProvider i;

    [SerializeField]
    private CinemachineVirtualCamera _cineCam;

    public CinemachineVirtualCamera cineCam
    {
        get { return _cineCam; }
    }

    void Awake()
    {
        i = this;
    }
}
