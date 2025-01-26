using Ink.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class StoryManager : MonoBehaviour, InputSystem.IStoryActions
{
    public static StoryManager instance;
    private Story story;
    public TextMeshProUGUI textbox;
    public GameObject nameTagL;
    public GameObject nameTagR;
    public TextMeshProUGUI nameTagLText;
    public TextMeshProUGUI nameTagRText;
    public Image characterLeft;
    public Image characterRight;
    public bool more_dialogue = false;
    public List<Sprite> characterSprites;

    private void Start()
    {
        instance = this;
        GameManager.instance.input.Story.SetCallbacks(this);
        GameManager.instance.input.Story.Enable();
        PlayDialogue("Intro");
    }

    public void PlayDialogue(string name)
    {
        string title = name;
        TextAsset inkJSON = Resources.Load<TextAsset>("Story/" + title);
        story = new Story(inkJSON.text);
        story.BindExternalFunction("set_speaker_l", (string speaker) =>
        {
            SetSpeaker(speaker, false);
            return 0;
        });
        story.BindExternalFunction("set_speaker_r", (string speaker) =>
        {
            SetSpeaker(speaker, true);
            return 0;
        });
        refreshUI();
    }


    public void ContinueStory()
    {
        if (story.canContinue)
        {
            refreshUI();
        }
        else
        {
            ExitStory();
        }
    }


    void refreshUI()
    {
        textbox.text = loadStoryChunk();
    }

    public void ExitStory()
    {
        if ((string)story.variablesState["nextScene"] != null)
        {
            string scene = (string)story.variablesState["nextScene"];
            GameManager.instance.input.Story.Disable();
            GameManager.instance.LoadScene(scene);
        }
    }

    string loadStoryChunk()
    {
        string text = "";
        text = story.Continue();
        return text;
    }

    void SetSpeaker(string speaker, bool right)
    {
        if (right)
        {
            nameTagL.SetActive(false);
            nameTagR.SetActive(true);

            nameTagRText.text = speaker;
            if (speaker != "All")
            {
                characterRight.sprite = LoadSpeakerSprite(speaker);
            }
        }
        else
        {
            nameTagL.SetActive(true);
            nameTagR.SetActive(false);

            nameTagLText.text = speaker;
            if (speaker != "All")
            {
                characterLeft.sprite = LoadSpeakerSprite(speaker);
            }
        }
    }

    Sprite LoadSpeakerSprite(string speaker)
    {
        Sprite sprite = Resources.Load<Sprite>("Sprites/" + speaker);
        return sprite;
    }

    public void OnContinueStory(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        ContinueStory();
    }
}
