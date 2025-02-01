using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public InputSystem input;
    public List<Unit> allies;
    public List<Unit> enemies;
    public bool canInterrupt;
    public string loadedStory;
    public AudioClip loadedMusic;
    public List<AudioClip> MusicFiles;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        input = new InputSystem();
        allies = new List<Unit>();
        enemies = new List<Unit>();
        canInterrupt = true;
    }

    private void Start()
    {
    }

    public void LoadScene(string SceneName)
    {
        SceneManager.LoadScene(SceneName);
    }
}
