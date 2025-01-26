using UnityEngine;
using UnityEngine.InputSystem;

public class IntroManager : MonoBehaviour, InputSystem.IIntroActions
{
    public static IntroManager instance;
    int intro_index;

    private void Start()
    {
        instance = this;
        intro_index = 0;
        GameManager.instance.input.Intro.SetCallbacks(this);
        GameManager.instance.input.Intro.Enable();
    }
    public void ContinueIntro()
    {
        transform.GetChild(intro_index).gameObject.SetActive(false);
        intro_index++;
        if (intro_index < transform.childCount - 1)
        {
            transform.GetChild(intro_index).gameObject.SetActive(true);
        }
        else
        {
            GameManager.instance.input.Intro.Disable();
            GameManager.instance.loadedStory = "Intro";
            GameManager.instance.loadedMusic = GameManager.instance.MusicFiles[1];
            GameManager.instance.LoadScene("StoryScene");
        }
    }

    public void OnContinueIntro(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            ContinueIntro();
        }
    }
}
