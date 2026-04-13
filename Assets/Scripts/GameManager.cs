using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public bool IsGameOver { get; set; } = false;
    private int notesCount = 0;
    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
        IsGameOver = false;
    }
    void Start()
    {

    }
    public int GetNotesCount()
    {
        return notesCount;
    }
    public void AddNote()
    {
        notesCount++;
    }

    public void RestartGame()
    {
        notesCount = 0;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}