using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceProvider : MonoBehaviour
{
    public static ResourceProvider i;

    [SerializeField]
    private LineDrawer _lineDrawer;

    public LineDrawer lineDrawer
    {
        get { return _lineDrawer; }
    }

    void Awake()
    {
        i = this;
    }
}
