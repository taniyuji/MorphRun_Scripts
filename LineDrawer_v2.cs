using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

//メッシュを生成させる InputManagerで使用
public class LineDrawer_v2 : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer meshObject;

    [SerializeField]
    private Material material;

    [SerializeField]
    private float length = 0.1f;

    [SerializeField]
    private float width = 0.1f;

    [SerializeField]
    private float zPos;

    //タッチ移動を格納
    List<Vector3> points = new List<Vector3>();

    //ポリゴンの頂点座標を格納
    List<Vector3> vertices = new List<Vector3>();
    List<Vector2> uvs = new List<Vector2>();

    //ポリゴンを描写するためのインデックスを格納
    List<int> triangles = new List<int>();

    int offset = 0;

    private Mesh mesh;

    private MeshFilter meshFilter;

    private MeshRenderer meshRenderer;

    private List<MeshCollider> meshColliders = new List<MeshCollider>();

    void Awake()
    {
        meshFilter = meshObject.gameObject.GetComponent<MeshFilter>();

        meshRenderer = meshObject.gameObject.GetComponent<MeshRenderer>();

        meshColliders.Add(meshObject.gameObject.GetComponent<MeshCollider>());
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var mousePosition = Input.mousePosition;

            mousePosition = new Vector3(mousePosition.x, mousePosition.y, zPos);

            PenDown(Camera.main.ScreenToWorldPoint(mousePosition));
        }
        else if (Input.GetMouseButton(0))
        {
            var mousePosition = Input.mousePosition;

            mousePosition = new Vector3(mousePosition.x, mousePosition.y, zPos);

            PenMove(Camera.main.ScreenToWorldPoint(mousePosition));
        }
    }

    Mesh CreateWholeMesh()
    {
        Mesh _mesh = new Mesh();
        _mesh.vertices = this.vertices.ToArray();

        _mesh.triangles = this.triangles.ToArray();
        _mesh.uv = GetUV();
        _mesh.Optimize();

        _mesh.RecalculateNormals();
        _mesh.RecalculateBounds();
        _mesh.RecalculateTangents();

        return _mesh;
    }

    Vector2[] GetUV()
    {
        for (int i = 0; i < Mathf.CeilToInt((float)vertices.Count / (float)8); i++)
        {
            int k = i * 8;
            uvs.Add(new Vector2(0, (float)k / (float)vertices.Count));
            k++;
            uvs.Add(new Vector2(1, (float)k / (float)vertices.Count));
            k++;
            uvs.Add(new Vector2(1, (float)k / (float)vertices.Count));
            k++;
            uvs.Add(new Vector2(0, (float)k / (float)vertices.Count));

            k++;
            uvs.Add(new Vector2(0, (float)k / (float)vertices.Count));
            k++;
            uvs.Add(new Vector2(1, (float)k / (float)vertices.Count));
            k++;
            uvs.Add(new Vector2(1, (float)k / (float)vertices.Count));
            k++;
            uvs.Add(new Vector2(0, (float)k / (float)vertices.Count));
        }
        return uvs.ToArray();
    }

    //メッシュを作成するメソッド
    void CreateMesh()
    {
        if (this.points.Count < 2)
        {
            Debug.Log("this.points < 2");
            return;
        }

        //1フレーム前のタッチ座標
        Vector3 prev = this.points[this.points.Count - 2];

        //最新のタッチ座標
        Vector3 top = this.points[this.points.Count - 1];

        Vector3 tmp = top - prev;
        //ドローが曲線を描く場合、prevとtopは斜めになる場合がある。
        //そういった際、足すxやyの値はその傾き具合によって変える必要があるため、ここで計算している
        Vector2 normal = new Vector2(tmp.y, -tmp.x).normalized * length;
        if (normal.x == 0 && normal.y == 0) return;//稀に値が0になる場合(prevとtopが近すぎる)があるためここで弾く
        //正面の面の座標を計算して使いまわす
        Vector2 fv1 = (Vector2)prev + normal;//左下
        Vector2 fv2 = (Vector2)top + normal;//右下
        Vector2 fv3 = (Vector2)top - normal;//右上
        Vector2 fv4 = (Vector2)prev - normal;//左上

        //曲線をより滑らかに見せる方法
        //1.メッシュ作成頻度を増やす
        //2.ドローされた座標よりも若干ずらして前のメッシュと重なるようにする
        //3.それぞれのメッシュの頂点が繋がるように頂点を計算する(難)
        //ポリゴンの頂点座標を取得
        Vector3[] _vertices =
        {
            new Vector3(fv1.x , fv1.y, prev.z ),
            new Vector3(fv2.x , fv2.y , top.z ),
            new Vector3(fv3.x , fv3.y , top.z ),
            new Vector3(fv4.x , fv4.y ,prev.z ),
            new Vector3(fv4.x , fv4.y , prev.z + width),
            new Vector3(fv3.x , fv3.y , top.z + width),
            new Vector3(fv2.x , fv2.y , top.z + width),
            new Vector3(fv1.x , fv1.y , prev.z +  width),
        };


        // ポリゴンの頂点座標をリストに追加
        for (int i = 0; i < _vertices.Length; i++)
        {
            this.vertices.Add(_vertices[i]);
        }

        //各ポリゴンの頂点座標にインデックス番号を割り当て
        int[] _triangles =
        {
            offset, offset + 2, offset + 1,
            offset, offset + 3, offset + 2,
            offset + 2, offset + 3, offset + 4,
            offset + 2, offset + 4, offset + 5,
            offset + 1, offset + 2, offset + 5,
            offset + 1, offset + 5, offset + 6,
            offset, offset + 7, offset + 4,
            offset, offset + 4, offset + 3,
            offset + 5, offset + 4, offset + 7,
            offset + 5, offset + 7, offset + 6,
            offset, offset + 6, offset + 7,
            offset, offset + 1, offset + 6,
        };

        //割り当てたインデックス番号をリストに追加
        for (int i = 0; i < _triangles.Length; i++)
        {
            this.triangles.Add(_triangles[i]);
        }

        offset += 8;//インデックス番号の総計を更新

        //メッシュに割り当て
        mesh.vertices = this.vertices.ToArray();

        mesh.triangles = this.triangles.ToArray();

        mesh.Optimize();

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();

        meshFilter.sharedMesh = mesh;

        meshRenderer.material = material;

        meshRenderer.enabled = true;
    }

    //メッシュの情報をリセットさせるメソッド
    public void PenDown(Vector3 tp)
    {
        meshRenderer.enabled = false;

        //メッシュの情報をリセット
        this.points.Clear();
        this.vertices.Clear();
        this.triangles.Clear();

        // 開始点を保存
        this.points.Add(tp);

        this.offset = 0;

        // メッシュ生成
        mesh = new Mesh();

        meshObject.transform.position = Vector3.zero;

        meshObject.transform.eulerAngles = Vector3.zero;

        for (int i = 0; i < meshColliders.Count; i++)
        {
            meshColliders[i].enabled = false;
        }
    }
    //マウスやタッチ入力がある間メッシュの更新をするメソッド
    public void PenMove(Vector3 tp)
    {
        //タッチしたポジションを追加
        this.points.Add(tp);
        //メッシュを生成
        CreateMesh();
    }
}
