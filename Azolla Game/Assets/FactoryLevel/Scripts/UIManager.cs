using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header ("Win/Lose panels")]
    [SerializeField] GameObject deathPanel;
    [SerializeField] GameObject winPanel;

    [Header("Pause game")]
    [SerializeField] private GameObject pauseScreen;


    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)) 
        {
            // If pause menu already active unpause
            if (pauseScreen.activeInHierarchy)
            {
                PauseMenu(false);
            }
            else
            {
                Cursor.visible= true;
                PauseMenu(true);
            }
        }
    }

    // Toggling win and death panels
    public void ToggleDeathPanel()
    {
        deathPanel.SetActive(!deathPanel.activeSelf);
    }

    public void ToggleWinPanel()
    {
        winPanel.SetActive(!winPanel.activeSelf);
    }

    private void PauseMenu(bool status)
    {
        // If true pause the game
        pauseScreen.SetActive(status);

        // 0 for pause the gameplay, 1 to back to normal speed
        if(status)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }
}
