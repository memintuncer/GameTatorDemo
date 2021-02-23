using UnityEngine;

/*
 Player yol çizmesini kolaylaştırmak için, yol çizme mekaniği bitene kadar sahnedeki kutuların transparan olmasını, colliderlarının disable olmasını sağlar.
Yol tamamlandığı zaman ilk haline döner.
 */

public class ObstacleController : MonoBehaviour
{
    public Material transparentMat;
    private Material oldMaterial;
    private GameObject player;
    private MeshRenderer meshRenderer;
    
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        oldMaterial = meshRenderer.material;
        player = GameObject.FindGameObjectWithTag("Player");
    }

   
    void Update()
    {
        if (player.GetComponent<PlayerController>().touchStartedOnPlayer)
        {
            meshRenderer.material = transparentMat;
            gameObject.GetComponent<Collider>().enabled = false;
        }
        else
        {
            gameObject.GetComponent<Collider>().enabled = true;
            meshRenderer.material = oldMaterial;
        }
       
    }
}
