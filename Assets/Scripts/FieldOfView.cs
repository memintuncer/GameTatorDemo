using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

/*
 -Sahne içerindeki FieldofView'i oluşturan script. 
 -Enemy'nin child objesinde kullanılan script sayesinde Player'ın içine girmemesi gereken dinamik bir görüş mesafesi oluşturuluyor.
 -Objelerin arkasını görmeyecek şekilde güncellenen görüş mesafesi Player içine girdiği anda Game Over koşulunu gerçekleştiriyor.
   
 */

public class FieldOfView : MonoBehaviour
{

    //Görüş mesafesinin açısı, uzunluğu vs. gibi özellikleri
    [Header("Field of View Settings")]
    [SerializeField] private float viewRadius = 50f;
    [SerializeField, Range(0, 360)] private float viewAngle = 90f;


    [SerializeField] private int meshResolution = 10;
    [SerializeField] private MeshFilter viewMeshFilter;
    private Mesh viewMesh;
    private List<Vector3> viewPoints = new List<Vector3>(); //Mesh'in çizileceği sınırları belirlemede kullanılan noktaların tutulduğu liste

    public TextMeshProUGUI Text;
    private bool playerFound;

    void Start()
    {
        playerFound = false;
        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;

    }


    void Update()
    {
        if (playerFound)
        {
            
            StartCoroutine("playerSpotted");
        }
        viewMeshFilter.mesh = viewMesh;
        DrawFieldOfView();
    }




    void DrawFieldOfView()
    {

        //"FoVinfo()" fonksiyonu ile elde ettiğim değerleri kullanarak objesini Meshini güncelliyorum. 

        viewPoints.Clear();
        FoV oldFoV = new FoV();

        for (int i = 0; i <= Mathf.RoundToInt(viewAngle * meshResolution); i++)
        {
            FoV newFoV = FoVinfo(transform.eulerAngles.y - viewAngle / 2 + (viewAngle / Mathf.RoundToInt(viewAngle * meshResolution)) * i, viewRadius);

            if (i > 0)
            {
                if (oldFoV.hit != newFoV.hit)
                {
                    
                    if (oldFoV.point != Vector3.zero)
                    {
                        viewPoints.Add(oldFoV.point);
                    }
                    if (newFoV.point != Vector3.zero)
                    {
                        viewPoints.Add(newFoV.point);
                    }
                }
            }

            viewPoints.Add(newFoV.point);
            oldFoV = newFoV;
        }
        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero;
        for (int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        viewMesh.Clear();

        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
    }


    /*
    
    -Objeden  "DirFromAngle" fonksiyonu ile elde edilen yöne Rayler gönderiyorum
    -Gönderdiğim Rayler hit olursa hit olunan noktanın bilgisi ile FoV objesi oluşturuyorum,
    eğer hit olmazsa belirlenen sınırları kapsayan  FoV objesi oluşturuyorum.
    -Eğer hit Player'a geliyorsa, Enemy objesini ve playerFound değerini güncelliyorum.
     */

    FoV FoVinfo(float globalAngle, float viewRadius)
    {
        Vector3 direction = DirFromAngle(globalAngle, true);
        RaycastHit hit;

        Physics.autoSyncTransforms = false;
        Debug.DrawRay(transform.position, direction * viewRadius, Color.red);

        if (Physics.Raycast(transform.position, direction, out hit, viewRadius))
        {
            if (hit.collider.transform.tag == "Player")
            {
                transform.parent.GetComponent<EnemyController>().playerSpotted = true;
                transform.parent.LookAt(hit.point);
                playerFound = true;
            }
            Physics.autoSyncTransforms = true;
            return new FoV(true, hit.point, hit.distance, globalAngle);

        }

        
        
        else
        {
            Physics.autoSyncTransforms = true;
            return new FoV(false, transform.position + direction * viewRadius, viewRadius, globalAngle);
        }

    }


    /*
     Açı değerini yön vektörü dönüştürdüm.
     */
    public Vector3 DirFromAngle(float angleInDegrees, bool IsAngleGlobal)
    {
        if (!IsAngleGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }


    IEnumerator playerSpotted()
    {
        
        Text.text = "Player Spotted!";
        
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene("Game");
        Time.timeScale = 1;
    }
}


/*
 -Görüş alanı çizerken kullandığım struct yapısı(Field of View)
 -Bu structan gelen değerler yardımı ile obje için kullandığım ViewMesh componentini güncelliyorum.
 */

public struct FoV
{
    public bool hit; 
    public Vector3 point; 
    public float distance; 
    public float angle;

    public FoV(bool hit, Vector3 point, float distance, float angle)
    {
        this.hit = hit;
        this.point = point;
        this.distance = distance;
        this.angle = angle;
    }
}




