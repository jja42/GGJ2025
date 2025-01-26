using UnityEngine;

public class TitleScreenMenu : MonoBehaviour
{
    public GameObject Instructions;
    public GameObject Instructions1;
    public GameObject Instructions2;
    public GameObject Attributions;
    public GameObject Attributions1;
    public GameObject Attributions2;
    public GameObject Attributions3;
    public GameObject Attributions4;

    public void Begin()
    {
        GameManager.instance.LoadScene("IntroScene");
    }

    public void OpenInstructions()
    {
        Instructions.SetActive(true);
        InstructionsPage1();
    }

    public void InstructionsPage1()
    {
        Instructions1.SetActive(true);
        Instructions2.SetActive(false);
    }

    public void InstructionsPage2()
    {
        Instructions1.SetActive(false);
        Instructions2.SetActive(true);
    }

    public void AttributionsPage1()
    {
        Attributions1.SetActive(true);
        Attributions2.SetActive(false);
        Attributions3.SetActive(false);
        Attributions4.SetActive(false);
    }

    public void AttributionsPage2()
    {
        Attributions1.SetActive(false);
        Attributions2.SetActive(true);
        Attributions3.SetActive(false);
    }

    public void AttributionsPage3()
    {
        Attributions2.SetActive(false);
        Attributions3.SetActive(true);
        Attributions4.SetActive(false);
    }
    public void AttributionsPage4()
    {
        Attributions3.SetActive(false);
        Attributions4.SetActive(true);
    }

    public void CloseInstructions()
    {
        Instructions.SetActive(false);
    }

    public void OpenAttributions()
    {
        Attributions.SetActive(true);
        AttributionsPage1();
    }

    public void CloseAttributions()
    {
        Attributions.SetActive(false);
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void Title()
    {
        GameManager.instance.LoadScene("TitleScene");
    }
}
