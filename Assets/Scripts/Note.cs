using UnityEngine;

public class Note : MonoBehaviour
{
    public MeshRenderer[] meshRenderer; //lista para tener una liste de los meshrenderers y poder cambiar el material de cada uno de ellos
    private Material[] originalMaterials; //variable para guardar el material original del meshrenderer y poder volver a el despues de cambiarlo
    public Material hightlightMaterial;
    private float lookRange = 3f;

    private PlayerLook player; //acceder al script de la camera 
    private Camera playerCamPosition; //variable para guardar la posicion de la camara del jugador y poder compararla con la posicion de la nota para saber si el jugador esta mirando la nota o no
    private bool isLookedAt = false; // si esta referenciado o siendo mirado, al principio sera falso porque obviamente aun no vio nada

    void Start()
    {
        meshRenderer = GetComponentsInChildren<MeshRenderer>(); //buscamos mesh renderers en los hijos, mesh renderer es para renderizar el 3D
        originalMaterials = new Material[meshRenderer.Length];
        for (int i = 0; i < meshRenderer.Length; i++)
        {
            originalMaterials[i] = meshRenderer[i].material; //guardamos el material original de cada meshrenderer en la lista
        }
        player = FindAnyObjectByType<PlayerLook>();
        playerCamPosition = player.GetComponentInChildren<Camera>();
    }

    void Update()
    {
        Ray ray = new Ray(playerCamPosition.transform.position, playerCamPosition.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, lookRange))
        {
            if (hit.collider.gameObject == this.gameObject)
            {
                isLookedAt = true;
                Debug.Log("Looking at note :D" + isLookedAt);
                IsLookedAt(isLookedAt);
            }
            return;
        }
        else
        {
            isLookedAt = false;
            //Debug.Log("Note looking at note " + isLookedAt);
            IsLookedAt(isLookedAt);
        }
    }

    void IsLookedAt(bool isLookAt)
    {
        isLookedAt = isLookAt;
        if (isLookedAt)
        {
            foreach (MeshRenderer mr in meshRenderer)
            {
                mr.material = hightlightMaterial; //cambiamos el material del meshrenderer al material de highlight
            }
        } else
        {
            foreach (MeshRenderer mr in meshRenderer)
            {
                
                   mr.material = originalMaterials[0]; //cambiamos el material del meshrenderer al material original guardado en la lista

            }
        }
    }
}
