using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DrawPhysicsLine : MonoBehaviour
{

    [Header("線を構成するブロック")]
    [SerializeField] GameObject linePrefab;
    [Header("線の単位長")]
    [SerializeField] float lineLength = 0.2f;
    [Header("線の幅")]
    [SerializeField] float lineWidth = 0.2f;
    [Header("線の奥行き")]
    [SerializeField] float lineZ = 1f;
    [Header("線の単位長の延長(線がギザギザするときはこの値を調整)")]
    [SerializeField] float lineLengthExtension = 0.1f;

    [SerializeField] private int instantiateAmount;

    // 一つ前のタッチされたpositionを保持
    private Vector3 touchPos;

    Camera mainCamera;

    public List<GameObject> lines1 { get; private set; } = new List<GameObject>();

    public List<GameObject> lines2 { get; private set; } = new List<GameObject>();

    private int index = 0;

    private bool isUsingList1 = true;

    private MeshRenderer mesh;

    private BoxCollider boxCollider;

    void Awake()
    {
        mesh = GetComponent<MeshRenderer>();

        boxCollider = GetComponent<BoxCollider>();

        mainCamera = Camera.main;

        for (int i = 0; i < instantiateAmount; i++)
        {
            GameObject obj = Instantiate(linePrefab) as GameObject;

            obj.SetActive(false);

            lines1.Add(obj);
        }

        for (int i = 0; i < instantiateAmount; i++)
        {
            GameObject obj = Instantiate(linePrefab) as GameObject;

            obj.SetActive(false);

            lines2.Add(obj);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            ChangeShape();
        }
    }

    public void ChangeShape()
    {

        if (boxCollider.enabled)
        {
            mesh.enabled = false;
            boxCollider.enabled = false;
        }

        for (int i = 0; i < instantiateAmount; i++)
        {
            if (!isUsingList1)
            {
                lines1[i].SetActive(false);

                lines1[i].transform.parent = null;

                if (i < index) lines2[i].SetActive(true);
            }
            else
            {
                lines2[i].SetActive(false);

                lines2[i].transform.parent = null;

                if (i < index) lines1[i].SetActive(true);
            }
        }

        //Debug.Log("shifted" + lines1[0].activeSelf + lines2[0].activeSelf);

        index = 0;
    }

    public void PenDown(Vector3 drawPos)
    {
        touchPos = drawPos;

        if (!lines1[0].activeSelf && !lines2[0].activeSelf)
        {
            isUsingList1 = true;
        }
        else
        {
            isUsingList1 = !lines1[0].activeSelf ? true : false;
        }
    }

    public void PenMove(Vector3 drawPos)
    {
        Vector3 startPos = touchPos;
        Vector3 endPos = drawPos;
        float distance = Vector3.Distance(startPos, endPos);

        if (index >= lines1.Count || index >= lines2.Count)
        {
            Debug.Log("no More Objects to draw");

            return;
        }

        if (distance > lineLength * Time.deltaTime / Time.fixedDeltaTime)
        {
            // GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);

            GameObject obj = isUsingList1 ? lines1[index] : lines2[index];

            obj.transform.parent = transform;

            obj.transform.localPosition = (startPos + endPos) / 2;
            obj.transform.right = (endPos - startPos).normalized;
            obj.transform.localScale = new Vector3(distance + lineLengthExtension, lineWidth, lineZ);

            touchPos = endPos;

            index++;
        }
    }
}
