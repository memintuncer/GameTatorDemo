using System.Collections;
using UnityEngine;

/*
  Player FieldofWiew içine girmediği müddetçe, Enemy'nin belirli süre zarfında yumurtaları kontrol etmesini sağlayan script. 
     */

public class EnemyController : MonoBehaviour
{
    public Transform[] waypoints; // Pozisyonları yumurtaların pozisyonları ile aynı olan boş objeleri tuttum. 
    public int nextWP=0,prevWP=0; //Bakılacak rota için kullandığım waypoint indexleri
    public float rotationSpeed;
    public bool playerSpotted;
    
    void Start()
    {
        StartCoroutine("nextWayPoint");
        
    }

    
    void Update()
    {


        if(!playerSpotted) 
            patrol();

    }


    /*
     Belirli aralıklarla "waypoints" içindeki hedef noktalar arasında Enemy'nin bakış noktalarını belirliyorum.
     */
    void patrol()
    {
        Vector3 targetDirection = waypoints[nextWP % 3].position - transform.position;
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, rotationSpeed * Time.deltaTime, 0.0f);
        transform.rotation = Quaternion.LookRotation(newDirection);
        
        if (prevWP == nextWP - 1)
        {
            
            StartCoroutine("nextWayPoint");
            prevWP++;
            
        }
        
    }

  

    IEnumerator nextWayPoint()
    {
       
        yield return new WaitForSeconds(5f);
        nextWP++;
        

    }
}
