using UnityEngine;
using UnityEngine.UI;

public class PageCurlModifier : BaseMeshEffect
{
    [HideInInspector]
    public float A = -50f;
    [HideInInspector]
    public float Theta = 0.1f;
    [HideInInspector]
    public float Rho = 0.1f;

    public bool ForceNewMesh;
    public bool IsFront = true;

    private float _a, _theta, _rho;
    private Mesh _m;
    private Vector3[] _verts;

    void Update()
    {
        if (A != _a || Theta != _theta || Rho != _rho)
        {
            _a = A;
            _theta = Theta;
            _rho = Rho;
            Apply();
        }
    }

    public override void ModifyMesh(Mesh mesh)
    {
        if (!IsActive())
            return;
        if (ForceNewMesh || _m == null)
        {
            _m = CreateMesh(mesh, 30, 30, IsFront);
            _verts = _m.vertices;
        }
        var modifiedVerticies = new Vector3[_verts.Length];
        float R, r, beta;

        for (int i = 0; i < _verts.Length; i++)
        {
            var vi = _verts[i];
            R = Mathf.Sqrt(vi.x * vi.x + Mathf.Pow(vi.y - A, 2));
            r = R * Mathf.Sin(Theta);
            beta = Mathf.Asin(vi.x / R) / Mathf.Sin(Theta);

            var v1 = new Vector3(r * Mathf.Sin(beta),
                R + A - r * (1 - Mathf.Cos(beta)) * Mathf.Sin(Theta),
                r * (1 - Mathf.Cos(beta)) * Mathf.Cos(Theta));

            var vert = new Vector3(v1.x * Mathf.Cos(Rho) - v1.z * Mathf.Sin(Rho),
                v1.y,
                (v1.x * Mathf.Sin(Rho) + v1.z * Mathf.Cos(Rho))
                );
            modifiedVerticies[i] = vert;
        }
        mesh.vertices = modifiedVerticies;      
        mesh.uv = _m.uv;
        mesh.triangles = _m.triangles;
        mesh.tangents = _m.tangents;
        mesh.RecalculateNormals();
    }

    private void Apply()
    {
        enabled = false;
        enabled = true;
    }

    private Mesh CreateMesh(Mesh mesh, int widthSegments, int lengthSegments, bool front)
    {
        Debug.Log("Create Mesh");
        var m = new Mesh();
        var width = mesh.bounds.size.x;
        var length = mesh.bounds.size.y;
        var anchorOffset = new Vector2(-width / 2.0f, -length / 2.0f);

        int hCount2 = widthSegments + 1;
        int vCount2 = lengthSegments + 1;
        int numTriangles = widthSegments * lengthSegments * 6;
        int numVertices = hCount2 * vCount2;

        Vector3[] vertices = new Vector3[numVertices];
        Vector2[] uvs = new Vector2[numVertices];
        int[] triangles = new int[numTriangles];
        Vector4[] tangents = new Vector4[numVertices];
        Vector4 tangent = new Vector4(1f, 0f, 0f, -1f);

        int index = 0;
        float uvFactorX = 1.0f / widthSegments;
        float uvFactorY = 1.0f / lengthSegments;
        float scaleX = width / widthSegments;
        float scaleY = length / lengthSegments;
        for (float y = 0.0f; y < vCount2; y++)
        {
            for (float x = 0.0f; x < hCount2; x++)
            {
                vertices[index] = new Vector3(x * scaleX - width / 2f - anchorOffset.x, y * scaleY - length / 2f - anchorOffset.y, 0.0f);
                tangents[index] = tangent;
                if (front)
                {
                    uvs[index] = new Vector2(x * uvFactorX, y * uvFactorY);
                }
                else
                {
                    uvs[index] = new Vector2((1f-x * uvFactorX), y * uvFactorY);
                }
                index++;
            }
        }

        index = 0;
        for (int y = 0; y < lengthSegments; y++)
        {
            if (!front)
            {
                for (int x = 0; x < widthSegments; x++)
                {
                    triangles[index] = (y*hCount2) + x;
                    triangles[index + 1] = ((y + 1)*hCount2) + x;
                    triangles[index + 2] = (y*hCount2) + x + 1;

                    triangles[index + 3] = ((y + 1)*hCount2) + x;
                    triangles[index + 4] = ((y + 1)*hCount2) + x + 1;
                    triangles[index + 5] = (y*hCount2) + x + 1;
                    index += 6;
                }
            }
            else
            {
                // Same tri vertices with order reversed, so normals point in the opposite direction
                for (int x = 0; x < widthSegments; x++)
                {
                    triangles[index] = (y * hCount2) + x;
                    triangles[index + 1] = (y * hCount2) + x + 1;
                    triangles[index + 2] = ((y + 1) * hCount2) + x;
                    triangles[index + 3] = ((y + 1) * hCount2) + x;
                    triangles[index + 4] = (y * hCount2) + x + 1;
                    triangles[index + 5] = ((y + 1) * hCount2) + x + 1;
                    index += 6;
                }           
            }
        }

        m.vertices = vertices;
        m.uv = uvs;
        m.triangles = triangles;
        m.tangents = tangents;
        m.RecalculateNormals();
        return m;
    }
}