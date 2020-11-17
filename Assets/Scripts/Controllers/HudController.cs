using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HudController : MonoBehaviour
{
    public Canvas hudCanvas;
    public Image healthBar;
    public Canvas menuCanvas;
    public HudPassive hudAbilityPrefab;

    Text winLoseText;
    Text monolithsRemainingText;
    Text enemiesRemainingText;

    Dictionary<string, HudPassive> passives;

    bool hasWon;
    bool hasLost;

    private void Awake()
    {
        passives = new Dictionary<string, HudPassive>();
        monolithsRemainingText = GameObject.Find("MonolithsText").GetComponent<Text>();
        enemiesRemainingText = GameObject.Find("EnemiesText").GetComponent<Text>();
    }

    public void UpdatePlayerHud(EntityStats stats)
    {
        healthBar.fillAmount = stats.hp / stats.maxHp;

        monolithsRemainingText.text = "Monoliths Remaining: " + GameController.Instance.monoliths.Count;
        enemiesRemainingText.text = "Enemies Remaining: " + GameController.Instance.enemies.Count;

        hasWon = GameController.Instance.CheckWinConditions();
        hasLost = GameController.Instance.CheckLoseConditions();

        if (hasWon)
        {
            ShowWinMenu();
        }
        else if (hasLost)
        {
            ShowLoseMenu();
        }
    }

    public void UpdatePassiveHud(PassivePickup.PassiveStats passiveStats)
    {
        if (passives.ContainsKey(passiveStats.passiveName))
        {
            passives[passiveStats.passiveName].IncreaseCount();
        }
        else
        {
            HudPassive hudPassive = GameObject.Instantiate(hudAbilityPrefab);
            hudPassive.passiveName = passiveStats.passiveName;
            hudPassive.passiveImage = passiveStats.passiveImage;
            hudPassive.gameObject.GetComponent<Image>().sprite = passiveStats.passiveImage;
            hudPassive.gameObject.transform.SetParent(GameObject.FindGameObjectWithTag("PassivePanel").transform);
            passives.Add(passiveStats.passiveName, hudPassive);
        }
    }

    public void ShowLoseMenu()
    {
        menuCanvas.gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.Confined;
        winLoseText = GameObject.Find("MenuPanel").GetComponentInChildren<Text>(true);
        winLoseText.color = Color.red;
        winLoseText.text = "You have died.";
    }

    public void ShowWinMenu()
    {
        menuCanvas.gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.Confined;
        winLoseText = GameObject.Find("MenuPanel").GetComponentInChildren<Text>(true);
        winLoseText.color = Color.green;
        winLoseText.text = "You have won!";
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ShowMenu()
    {
        if (menuCanvas.gameObject.activeSelf == false)
        {
            Time.timeScale = 0;
            GameController.Instance.isPaused = true;
            menuCanvas.gameObject.SetActive(true);
            Cursor.lockState = CursorLockMode.Confined;
        }
        else
        {
            Time.timeScale = 1;
            GameController.Instance.isPaused = false;
            menuCanvas.gameObject.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
