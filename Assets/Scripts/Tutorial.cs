using UnityEngine;
using UnityEngine.SceneManagement;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private GameObject[] buttons;
    private bool _buttonAdvance;
    private int _buttonNum;

    private void Start()
    {
        //Set default.
        _buttonAdvance = false;
        _buttonNum = 0;
    }

    public void AdvanceButton()
    {
        //Increase and set true to change which button shows.
        _buttonAdvance = true;
        _buttonNum++;
    }

    private void Update()
    {
        if (_buttonAdvance)
        {
            //IF at end of tutorial, load MainMenu scene.
            if (_buttonNum == buttons.Length)
            {
                SceneManager.LoadScene("MainMenu");
            }
            else
            {
                buttons[_buttonNum].SetActive(true);
                if (_buttonNum > 0)
                {
                    buttons[_buttonNum - 1].SetActive(false);
                }

                _buttonAdvance = false;
            }
        }
    }
}
