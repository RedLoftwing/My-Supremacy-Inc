using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private GameObject[] buttons;
    private bool buttonAdvance;
    private int buttonNum;

    private void Start()
    {
        //Set default.
        buttonAdvance = false;
        buttonNum = 0;
    }

    public void AdvanceButton()
    {
        //Increase and set true to change which button shows.
        buttonAdvance = true;
        buttonNum++;
    }

    private void Update()
    {
        if (buttonAdvance)
        {
            //IF at end of tutorial, load MainMenu scene.
            if (buttonNum == buttons.Length)
            {
                SceneManager.LoadScene("MainMenu");
            }
            else
            {
                buttons[buttonNum].SetActive(true);
                if (buttonNum > 0)
                {
                    buttons[buttonNum - 1].SetActive(false);
                }

                buttonAdvance = false;
            }
        }
    }
}
