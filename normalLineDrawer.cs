using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//LineRendererを制御するスクリプト
public class normalLineDrawer : MonoBehaviour
{
   
    [SerializeField]
    private Transform pointerTransform;

    [SerializeField]
    private Canvas drawRangeCanvas;

    private LineRenderer lineRenderer;

    private int positionCount;



    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            pointerTransform.gameObject.SetActive(true);

            drawRangeCanvas.gameObject.SetActive(true);

            return;
        }

        if(Input.GetMouseButton(0))
        {
            positionCount++;
            lineRenderer.positionCount = positionCount;
            lineRenderer.SetPosition(positionCount - 1, pointerTransform.localPosition);
        }

        if (Input.GetMouseButtonUp(0))
        {
            positionCount = 0;

            lineRenderer.positionCount = 0;

            pointerTransform.gameObject.SetActive(false);

            drawRangeCanvas.gameObject.SetActive(false);
        }
    }
}
