using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public TMPro.TMP_Text livesText;
    public TMPro.TMP_Text coinsText;
    public TMPro.TMP_Text timerText;
    public Image[] arrowImages;
    public Image gameOverImage;

    private float flashTimer = 0.1f;
    private bool flashOn = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameOverImage.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // Set HUD text 
        livesText.text = GetHudText(Game.Instance.GetMarioState.Lives);
        coinsText.text = GetHudText(Game.Instance.GetMarioState.Coins);
        timerText.text = GetHudText(Mathf.CeilToInt(Game.Instance.TimeRemaining));

        // Handle the running meter arrows and (P)
        if (Game.Instance.GetMarioState.RunningMeter == 7)
        {
            flashTimer -= Time.deltaTime;

            if (flashTimer < 0.0f)
            {
                flashTimer = 0.1f;
                flashOn = !flashOn;
            }

            if (flashOn)
            {
                EnableAllArrows();
            }
            else
            {
                DisableAllArrows();
            }
        }
        else if (Game.Instance.GetMarioState.RunningMeter == 0)
        {
            DisableAllArrows();
            flashOn = true;
        }
        else
        {
            EnableArrows(Game.Instance.GetMarioState.RunningMeter - 1);
        }

        // If the game is over, show the game over image
        if (Game.Instance.IsGameOver)
        {
            if (gameOverImage.gameObject.activeSelf == false)
            {
                gameOverImage.gameObject.SetActive(true);
            }
        }
    }

    private string GetHudText(int value)
    {
        string returnString = "";
        string valueString = value.ToString();

        for (int i = 0; i < valueString.Length; i++)
        {
            returnString += "<sprite name=\"HUD-Numbers_";
            returnString += valueString[i];
            returnString += "\">";
        }

        return returnString;
    }

    private void EnableArrows(int maxEnabledIndex)
    {
        for (int i = 0; i < arrowImages.Length; i++)
        {
            Color color = new Color(1.0f, 1.0f, 1.0f, (i <= maxEnabledIndex) ? 1.0f : 0.0f);
            arrowImages[i].color = color;
        }
    }

    private void EnableAllArrows()
    {
        EnableArrows(arrowImages.Length - 1);
    }

    private void DisableAllArrows()
    {
        EnableArrows(-1);
    }
}
