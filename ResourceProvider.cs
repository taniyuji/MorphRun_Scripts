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

    [SerializeField]
    private ShapeInputController _inputController;

    public ShapeInputController inputController
    {
        get { return _inputController; }
    }

    [SerializeField]
    private DrawPhysicsLine _drawer;

    public DrawPhysicsLine drawer
    {
        get { return _drawer; }
    }

    [SerializeField]
    private ShapeAnimation _shapeAnimation;

    public ShapeAnimation shapeAnimation
    {
        get { return _shapeAnimation; }
    }

    void Awake()
    {
        i = this;
    }
}
