using System.IO.IsolatedStorage;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class SlenderMan : MonoBehaviour
{
    private NavMeshAgent navMeshAgentSlender;
    private PlayerMovement player;
    private PlayerLook playerLook;
    private SkinnedMeshRenderer slenderMeshRenderer;
    private Animator slenderAnimator;

    private float baseSpeed = 0.5f;
    private float catchDistance = 2f;
    private bool isGameOver = false;

    public float gameOverDelay = 3f;

    //para controlar el audio de slendy
    public AudioSource staticAudio;
    public float maxStaticDistance = 15f;
    public float minStaticDistance = 2f;

    void Start()
    {
        navMeshAgentSlender = GetComponent<NavMeshAgent>();
        player = FindAnyObjectByType<PlayerMovement>();
        playerLook = FindAnyObjectByType<PlayerLook>();
        slenderMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        slenderAnimator = GetComponent<Animator>();

        navMeshAgentSlender.speed = baseSpeed; 
        if (staticAudio != null)
        {
            staticAudio.loop = true;
            staticAudio.spatialBlend = 1f;
        }

    }

    void Update()
    {
        if (isGameOver) return;
        if (navMeshAgentSlender.enabled)
        {
            navMeshAgentSlender.destination = player.transform.position;

            float currentVelocity = navMeshAgentSlender.velocity.magnitude;
            slenderAnimator.SetFloat("speed", currentVelocity);
            VerificarDistanciaConJugador();
        }
        cambiarDificultad();
        UpdateStaticAudio();

    }

    public void cambiarDificultad()
    {
        int notas = GameManager.Instance.GetNotesCount();
        if (notas < 1)
        {
            navMeshAgentSlender.enabled = false;
            slenderMeshRenderer.enabled = false;
        } else
        {
            navMeshAgentSlender.enabled = true;
            slenderMeshRenderer.enabled = true;
        }
        navMeshAgentSlender.speed = baseSpeed + (notas * 0.5f);
        if (notas >= 2)
        {
            slenderAnimator.SetBool("isSprint", true);
            navMeshAgentSlender.acceleration = 12f;
        }
    }

    public void VerificarDistanciaConJugador()
    {
        //verificar la distancia entre el jugador y slennder
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position); //cuanto de distancia entre estos dos valores osea entre la pos de player y de slender
        if (distanceToPlayer <= catchDistance)
        {
            AtraparJugador();
        }

    }

    public void AtraparJugador()
    {
        isGameOver = true;
        isGameOver = true;

        if (GameManager.Instance != null)
            GameManager.Instance.IsGameOver = true;
        if (staticAudio != null)
            staticAudio.Stop();

        // la ides es: detener a slender, mira al jugador en eje Y, desactivar los controles del jugador y forzar a la camra de platyer a quevea a slender
        navMeshAgentSlender.isStopped = true;
        navMeshAgentSlender.velocity = Vector3.zero;

        Vector3 slenderLookAt = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z); //mantenga el valor en Y para uqe no cambie rotacion de forma extrana
        transform.LookAt(slenderLookAt);
        slenderAnimator.SetTrigger("jumpscare");

        player.enabled = false;
        playerLook.enabled = false;

        Vector3 playerLookAtSlender = transform.position + (Vector3.up * 2f);
        playerLook.playerCamera.LookAt(playerLookAtSlender);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        StartCoroutine(LoadNextSceneAfterDelay(gameOverDelay));
    }

    private System.Collections.IEnumerator LoadNextSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    void UpdateStaticAudio()
    {
        if (staticAudio == null || player == null) return;

        float distance = Vector3.Distance(transform.position, player.transform.position);

        if (distance <= maxStaticDistance)
        {
            if (!staticAudio.isPlaying)
                staticAudio.Play();

            float t = Mathf.InverseLerp(maxStaticDistance, minStaticDistance, distance);
            staticAudio.volume = Mathf.Clamp01(t);
        }
        else
        {
            if (staticAudio.isPlaying)
                staticAudio.Stop();
        }
    }
}