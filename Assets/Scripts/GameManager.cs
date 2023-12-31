using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //Serialized variables
    [SerializeField] private float powerUpTime = 10;
    [SerializeField] private Text scoreText, pelletsLeftText;
    [SerializeField] private Transform bonusItemSpawn;
    [SerializeField] private Bounds ghostSpawnBounds;
    [SerializeField] private GameObject endPanel;
    [SerializeField] private AudioClip pelletClip;
    [SerializeField] private AudioClip powerPelletClip;
    [SerializeField] private AudioClip bonusItemClip;
    [SerializeField] private AudioClip eatGhostClip;

    //Private variables
    private GameObject bonusItem;
    private int totalPellets = 0;
    private int score = 0;
    private int collectedPellets = 0;
    private AudioSource aSrc;
    private int pelletsLeft;
    public GameObject pointer;
    public Image powerUpBar;
    public bool barBool, menuActive, modeToggle;

    //Auto-properties
    public float PowerUpTimer { get; private set; } = -1;
    public Bounds GhostSpawnBounds { get { return ghostSpawnBounds; } }

    //Singletons
    public static GameManager Instance { get; private set; }

    //Delegates
    public delegate void PowerUp();
    public delegate void GameEvent();
    public GameEvent Delegate_GameOver = delegate { };

    //Events
    public event PowerUp Event_PickUpPowerPellet = delegate { };
    public event PowerUp Event_EndPowerUp = delegate { };
    public event GameEvent Event_GameVictory = delegate { };

    /// <summary>
    /// Create necessary references.
    /// </summary>
    private void Awake()
    {
        //Set singleton
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("Game Manager: More than one Game Manager is present in the scene.");
            enabled = false;
        }
        //Set audio source reference
        TryGetComponent(out AudioSource audioSource);
        if(audioSource != null)
        {
            aSrc = audioSource;
        }
        else
        {
            Debug.LogError("Game Manager: No audio source attached to Game Manager!");
        }
        //Find bonus item
        bonusItem = GameObject.FindGameObjectWithTag("Bonus Item");
        //Count pellets
        totalPellets = GameObject.FindGameObjectsWithTag("Pellet").Length;
        totalPellets += GameObject.FindGameObjectsWithTag("Power Pellet").Length;
    }

    /// <summary>
    /// Set initial game state.
    /// </summary>
    private void Start()
    {
        barBool = false;
        powerUpBar.fillAmount = 0f;
        //Assign delegates/events
        Event_GameVictory += ToggleEndPanel;
        Delegate_GameOver += ToggleEndPanel;
        //Disable bonus item
        if (bonusItem != null)
        {
            bonusItem.SetActive(false);
        }
        else
        {
            Debug.LogError("Game Manager: Bonus item must be in the scene and tagged as 'Bonus Item'!");
        }
        //Disable end game panel
        if (endPanel != null)
        {
            if (endPanel.activeSelf == true)
            {
                ToggleEndPanel();
            }
        }
        else
        {
            Debug.LogError("Game Manager: End Panel has not been assigned!");
        }
        //Set score text value
        if (scoreText != null)
        {
            scoreText.text = $"{score}";
            pelletsLeft = totalPellets - collectedPellets -1;
            pelletsLeftText.text = $"there are {pelletsLeft} pellets left!";
        }
        else
        {
            Debug.LogError("Game Manager: Score Text has not been assigned!");
        }
    }

    /// <summary>
    /// Frame by frame functionality.
    /// </summary>
    private void Update()
    {
        //Active power up timer
        if (PowerUpTimer > -1)
        {
            PowerUpTimer += Time.deltaTime;
            if (!barBool)
            {
                pointer.SetActive(true);
                StartCoroutine(BarDuration());
            }
            if(PowerUpTimer > powerUpTime)  //Power up timer finished
            {
                Event_EndPowerUp.Invoke();
                PowerUpTimer = -1;
            }
        }        
    }

    IEnumerator BarDuration() //Keeps GUI Powerup bar from exceeding total time limit of 10 seconds
    {
        barBool = true;
        yield return new WaitForSeconds(1);   
        powerUpBar.fillAmount -= 0.1f;
        barBool = false;
    }

    /// <summary>
    /// Called when a pellet is picked up.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="powerUp"></param>
    /// <param name="bonus"></param>
    public void PickUpPellet(int value, int type = 0)
    {
        //Add score
        score += value;
        //Set score text value
        if (scoreText != null)
        {
            scoreText.text = $"{score}";
            pelletsLeft = totalPellets - collectedPellets;
            pelletsLeftText.text = $"there are {pelletsLeft} pellets left!";
        }
        else
        {
            Debug.LogError("Game Manager: Score Text has not been assigned!");
        }

        if(type == 0) //Play Pellet sound
        {
            aSrc.PlayOneShot(pelletClip);
        }
        else if (type == 1) //Activate power up
        {
            Event_PickUpPowerPellet.Invoke();
            PowerUpTimer = 0;
            powerUpBar.fillAmount = 1f;
            aSrc.Stop();
            aSrc.PlayOneShot(powerPelletClip);
        }

        if (type != 2)
        {
            collectedPellets++;
            //Check ratio of collected pellets
            float ratio = (float)collectedPellets / totalPellets;
            if (ratio != 1)
            {
                if (ratio % 0.25f == 0)
                {
                    //Spawn bonus item
                    if (bonusItem != null)
                    {
                        if (bonusItem.activeSelf == false)
                        {
                            if (bonusItemSpawn != null)
                            {
                                bonusItem.transform.position = bonusItemSpawn.position;
                                bonusItem.SetActive(true);
                            }
                            else
                            {
                                Debug.LogError("Game Manager: Bonus Item Spawn has not been assigned!");
                            }
                        }
                    }
                    else
                    {
                        Debug.LogError("Game Manager: Bonus item must be in the scene and tagged as 'Bonus Item'!");
                    }
                }
            }
            else
            {
                Event_GameVictory.Invoke();
            }
        }
        else
        {
            aSrc.PlayOneShot(bonusItemClip);
        }
    }

    /// <summary>
    /// Called when a ghost is eaten.
    /// </summary>
    /// <param name="ghost"></param>
    public void EatGhost(Ghost ghost)
    {
        //Add score
        score += 5;
        //Set score text value
        if (scoreText != null)
        {
            scoreText.text = $"{score}";
        }
        aSrc.PlayOneShot(eatGhostClip);
        //Respawn
        ghost.SetState(ghost.RespawnState);
    }

    /// <summary>
    /// Resets the scene.
    /// </summary>
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Quits the game.
    /// </summary>
    public void QuitToDesktop()
    {
        Application.Quit();
    }

    /// <summary>
    /// Toggles the end game panel on and off.
    /// </summary>
    private void ToggleEndPanel()
    {
        if(endPanel.activeSelf == false)
        {
            endPanel.SetActive(true);
        }
        else
        {
            endPanel.SetActive(false);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(ghostSpawnBounds.center, ghostSpawnBounds.size);
    }
}
