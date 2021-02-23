using UnityEngine;

/*
 
�nternetten haz�r olarak kulland���m bir Script, i�inde herhangi bir de�i�iklik yapmad�m.
�al��ma mekani�ini anlamak ad�na scriptin anlat��� videoyu izleyerek,
sahnedeki Assetlerin oranlar�n�n korunmas�n� sa�layacak �ekilde  ZoomFactor vekt�r�n�n de�erleriyle oynad�m.
 
 */
public class CameraAspectRatioScaler : MonoBehaviour {

  
    public Vector2 ReferenceResolution;

    
    public Vector3 ZoomFactor = Vector3.one;

   
    [HideInInspector]
    public Vector3 OriginPosition;

    
    void Start () {
        OriginPosition = transform.position;
    }
	
	
	void Update () {

        if (ReferenceResolution.y == 0 || ReferenceResolution.x == 0)
            return;

        var refRatio = ReferenceResolution.x / ReferenceResolution.y;
        var ratio = (float)Screen.width / (float)Screen.height;

        transform.position = OriginPosition + transform.forward * (1f - refRatio / ratio) * ZoomFactor.z
                                            + transform.right   * (1f - refRatio / ratio) * ZoomFactor.x
                                            + transform.up      * (1f - refRatio / ratio) * ZoomFactor.y;


    }
}
