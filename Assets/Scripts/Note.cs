using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Note : MonoBehaviour
{
    public Material highlightMaterial;
    public TMP_Text areYouSureText;
    public InputActionReference collectActionReference;

    private MeshRenderer[] meshRenderers;
    private Material[] originalMaterials;
    private float lookRange = 3f;

    private PlayerLook player;
    private Camera playerCamPosition;
    private bool isLookedAt = false;

    private int noteLayerMask;


    void Start()
    {
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        originalMaterials = new Material[meshRenderers.Length];

        for (int i = 0; i < meshRenderers.Length; i++)
            originalMaterials[i] = meshRenderers[i].material;

        player = FindAnyObjectByType<PlayerLook>();
        playerCamPosition = player.GetComponentInChildren<Camera>();

        noteLayerMask = LayerMask.GetMask("Note");

        areYouSureText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
        {
            if (areYouSureText != null)
                areYouSureText.gameObject.SetActive(false);
            return;
        }

        if (playerCamPosition == null)
        {
            TryFindCamera();
            return;
        }

        CheckIfLookingAtNote();
        CollectNote();
    }

    void TryFindCamera()
    {
        player = FindAnyObjectByType<PlayerLook>();
        if (player != null)
            playerCamPosition = player.GetComponentInChildren<Camera>();
    }

    void CheckIfLookingAtNote()
    {
        if (playerCamPosition == null) return;
        Ray ray = new Ray(
            playerCamPosition.transform.position,
            playerCamPosition.transform.forward
        );

        if (Physics.Raycast(ray, out RaycastHit hit, lookRange, noteLayerMask) && hit.collider.gameObject == this.gameObject)
        {
            if (!isLookedAt)
            {
                isLookedAt = true;
                areYouSureText.gameObject.SetActive(true);
                IsLookedAt(true);
            }
        }
        else
        {
            if (isLookedAt)
            {
                isLookedAt = false;
                areYouSureText.gameObject.SetActive(false);
                IsLookedAt(false);
            }
        }
    }

    public void CollectNote()
    {
        if (isLookedAt && collectActionReference.action.WasPressedThisFrame())
        {
            areYouSureText.gameObject.SetActive(false);
            GameManager.Instance.AddNote();
            Destroy(gameObject);
        }
    }

    void IsLookedAt(bool state)
    {
        for (int i = 0; i < meshRenderers.Length; i++)
            meshRenderers[i].material = state ? highlightMaterial : originalMaterials[i];
    }
}