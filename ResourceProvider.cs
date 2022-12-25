using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

//シーン上にひとつしかないものの参照を提供するスクリプト
public class ResourceProvider : MonoBehaviour
{
    public static ResourceProvider i;

    [SerializeField]
    private CinemachineVirtualCamera _cineCam;

    public CinemachineVirtualCamera cineCam
    {
        get { return _cineCam; }
    }

    [SerializeField]
    private DrawPhysicsLine _drawer;

    public DrawPhysicsLine drawer
    {
        get { return _drawer; }
    }

    [SerializeField]
    private SpriteRenderer _ok;

    public SpriteRenderer ok
    {
        get { return _ok; }
    }

    void Awake()
    {
        i = this;
    }
}
