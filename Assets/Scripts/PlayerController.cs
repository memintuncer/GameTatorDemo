using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;



/*
 
Mouse Player'a dokunduğu andan itibaren Player'ın ilerliyeceği yolun çizilmesini ve Player hareketlerini kontrol eden script. 

 */

public class PlayerController : MonoBehaviour
{
    
    public GameObject wayPoint, child, eggs; 

    public LineRenderer lineRenderer; //Line çizilmesi için

    public List<GameObject> wayPoints; // Karakterin ilerleceği hedef noktası olarak kullandığım boş bir GameObject olan Waypoint objelerinin tutulduğu liste.

    public TextMeshProUGUI Text;

    private Rigidbody rigidbody;

    public bool touchStartedOnPlayer;
    public float speed, timeForNextRay = 0.05f, timer = 0, pushForce;

    private int chilCount, targetChildcount, currentWayPoint = 0, wayIndex;
    public bool levelClear, lineCompleted, move;



    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        lineRenderer.enabled = false;
        wayIndex = 1;
        move = false;
        touchStartedOnPlayer = false;
        lineCompleted = false;

        //Final noktasına ulaşıldığında tüm yumurtaların toplanıp toplanmadığını anlamak için sahnedeki yumurta sayılarını elde ediyorum.
        //Sabir bir sayı vermek yerine dinamik yumurta sayılarını için bu yöntemi seçtim.

        if (eggs == null)
        {
            eggs = GameObject.FindGameObjectWithTag("Child");
            targetChildcount = eggs.transform.childCount;
        }

        
    }
    public void OnMouseDown()
    {

        /*Mouse ile Player'a dokunulduğu anda eğer yol çizme işlemi tamamlanmamışsa aşağıdaki koşulları oluşturuyorum.
         
        - Yolun çizilebilmesi için LineRenderer objesini aktive ediyor ve başlangıç noktasını Player'ın pozisyonu olarak belirliyorum.
        - "touchStartedOnPlayer" değerini "true" yaparak Update içinde yolun çizilmesini sağlayacak olan koşulu aktive ediyorum.
          
        */
        if (!lineCompleted)
        {
            
            lineRenderer.enabled = true;
            lineRenderer.positionCount = 1;
            lineRenderer.SetPosition(0, transform.position);
            touchStartedOnPlayer = true;
            Text.text = "";
           
        }

       
        

    }
    
    void  Update()
    {
        // Mouse'un sol butonu ile Playerin ilk konumunu seçilip, basılı tutulduğu andan itibaren yolun çizilmesini sağlayan koşul.

        if (Input.GetMouseButton(0) && timer > timeForNextRay && touchStartedOnPlayer&& !lineCompleted) 
            drawLine();
       

        // Yol çizme işlemi tamamlandıktan sonra Player'ın hareketini kontol eden durum.

        if (Input.GetMouseButton(0) && lineCompleted && move && wayPoints.Count > 0) 
            playerMovement();
        
        timer += Time.deltaTime;

        // Yol tamamlandıktan sonra Player'ın hareket etmesine olanak sağlayacak olan koşul.

        if (Input.GetMouseButtonUp(0) && touchStartedOnPlayer)
        {
            if (!lineCompleted)
            {
                
                lineCompleted = true;
            }
            touchStartedOnPlayer = false;
            move = true;
            
        }
    }


    void playerMovement()
    {


        //"currentWayPoint" değeri ile Playerin anlık  bakacağı hedef noktayı seçiyor, hedefe ulaşana kadar da o noktaya doğru hareket etmesini sağlıyorum.
        // Hedefe ulaştığında, hedef noktasını Waypointlerin tutulduğu "wayPoints" listesinden indexi artırarak güncelliyorum.

        transform.LookAt(wayPoints[currentWayPoint].transform);
        transform.position = Vector3.MoveTowards(transform.position, wayPoints[currentWayPoint].transform.position, speed * Time.deltaTime);

        if (transform.position == wayPoints[currentWayPoint].transform.position) 
            currentWayPoint++;
       

        // Player çizilen yolu tamamladığında listeyi temizliyor, Leveli tamamlamak için gerekli koşulları sağlamış mı diye kontrol ediyorum.

        if (currentWayPoint == wayPoints.Count)
        {
            if (!levelClear)
            {
                StartCoroutine("TryAgain");
            }
            move = false;

            foreach (var item in wayPoints) Destroy(item);         
            
            wayPoints.Clear();
            wayIndex = 1;
            currentWayPoint = 0;
        }
        
    }


    private void drawLine()
    {


        // Farenin Vector3 konumunu Screen Space pozisyoundan World Space dönüştürdüm.
        //Kamera konumunu farenin pozisyon Vector3'ünden çıkarardım, bu sayede kameranın yönünün ve konumunun bir önemi kalmadı.

        Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 100));
        Vector3 direction = worldMousePos - Camera.main.transform.position;


        /*
        -Kamera pozisyonunda fare posizyonuna Ray gönderiyorum.
        -Hit noksatında boş Waypoint objeleri oluşturuyorum ve bu objlerin pozisyonu kullanarak LineRenderer ile yolu çiziyorum.
        -Waypoint objeleri "wayPoints" listesine ekleyerek "playerMovement()" fonksiyonunda Player karakterinin bakacağı yönü ve 
        ilerleyeceği pozisyonun belirlenmesini sağlıyorum.
         */

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, direction, out hit, 100f))
        {
            
            GameObject newWaypoint = Instantiate(wayPoint, new Vector3(hit.point.x, transform.position.y, hit.point.z), Quaternion.identity);
            wayPoints.Add(newWaypoint);
            lineRenderer.positionCount = wayIndex + 1;
            lineRenderer.SetPosition(wayIndex, newWaypoint.transform.position-new Vector3(0,.75f,0));
            timer = 0;
            wayIndex++;
        }
    }


    

    private void OnTriggerEnter(Collider other)
    {

        /*
         Player Yumurta objesinin collider'ine temas ettiğinde Egg objesini Destroy edip Player'ı takip edecek çocuk objeleri ürettim.
         */

        if (other.gameObject.tag == "Egg")
        {
            chilCount++;
            Destroy(other.gameObject);
            GameObject childObject = Instantiate(child, other.transform.position, Quaternion.identity);
            childObject.transform.parent = transform;
            childObject.transform.localPosition =  new Vector3(0, -0.5f,-chilCount);
  

        }


        /*
         Finish noktasındaki koşulları kontrol ettim.
         */

        if(other.gameObject.tag == "Finish")
        {
            if (chilCount == targetChildcount)
            {
                levelClear = true;
                StartCoroutine("LevelCleared");
            }
            else
            {
               
                StartCoroutine("TryAgain");
            }
        }
    }


    //Sahnedeki kutuları ilerletmek için Collide anında kutulara hız verip, temas bittiğinde kutuların hızlarını sıfırlıyorum. 

    private void OnCollisionEnter(Collision collision)
    {

        

        if (collision.gameObject.tag == "Obstacle") 
            collision.rigidbody.velocity = transform.forward * pushForce;
       
        if (collision.gameObject.tag == "Enemy") 
            StartCoroutine("TryAgain");
      
        


    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Obstacle")  
            collision.rigidbody.velocity=Vector3.zero;
    }


    IEnumerator LevelCleared()
    {
        
        Text.text = "Level Cleared!";
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("Game");
    }

    IEnumerator TryAgain()
    {
        move = false;
        Text.text = "Failed";
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("Game");
    }
}
