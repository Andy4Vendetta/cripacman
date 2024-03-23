using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private Ghost[] ghosts;
    [SerializeField] private Pacman pacman;
    [SerializeField] private Transform pellets;
    [SerializeField] private Text gameOverText;
    [SerializeField] private Text scoreText;
    [SerializeField] private Text livesText;
    [SerializeField] private Text highScoreText;
    [SerializeField] private Text levelText;
    public AudioSource sound;
    public AudioSource gameMusicCalm;
    public AudioSource gameMusicCombat;
    public AudioClip coinClip;
    public AudioClip rageClip;
    public AudioClip deathClip;
    public AudioClip ghostDeathClip;
    public AudioClip gameOverClip;
    public AudioClip notOverClip;
    public AudioClip calmClip;
    public AudioClip combatClip;
    public AudioClip enoughClip;
    public AudioClip yesClip;
    public AudioClip niceTryClip;
    public AudioClip swordsClip;
    public AudioClip thatsitClip;
    private int ghostMultiplier = 1;
    private int lives = 3;
    private int score = 0;
    private int level = 1;
    private int highscore = 0;
// записывается ток после проверки на момент проигрыша и записывается лучший результат, засунуть в метод проигрыша    private int Hiscore = score;

    public int HighScore => highscore;
    public int Lives => lives;
    public int Score => score;

    private void Awake()
    {
        if (Instance != null) {
            DestroyImmediate(gameObject);
        } else {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        gameMusicCalm.clip = calmClip;
        gameMusicCombat.clip = combatClip;
        gameMusicCalm.loop = true;
        gameMusicCombat.loop = true;
        gameMusicCalm.volume = 0.6f;
        gameMusicCombat.volume = 0;
        gameMusicCalm.Play();
        gameMusicCombat.Play();
        NewGame();
    }
    private void Update()
    {
        if (lives <= 0 && Input.anyKeyDown) {
            NewGame();
        }
    }

    private void NewGame()
    {
        SetScore(0);
        SetLives(3);
        NewRound();
        SetLevel(1);
        foreach (var ghost in ghosts)
        {
            ghost.movement.speed = 7f;
        }
        gameMusicCalm.volume = 0.6f;
        gameMusicCombat.volume = 0;
    }

    private void NewRound()
    {
        gameOverText.enabled = false;
        foreach (Transform pellet in pellets) {
            pellet.gameObject.SetActive(true);
        }
        foreach (var ghost in ghosts)
        {
            ghost.movement.speed += 0.5f;
        }
        SetLevel(level + 1);
        ResetState();
    }

    private void ResetState()
    {
        sound.Stop();
        for (int i = 0; i < ghosts.Length; i++) {
            ghosts[i].ResetState();
        }
        if (lives > 1)
        {
            gameMusicCalm.volume = 0.6f;
            gameMusicCombat.volume = 0;
        }
        else
        {
            gameMusicCombat.volume = 0.4f;
            gameMusicCalm.volume = 0;
        }
        pacman.ResetState();
    }

    private void GameOver()
    {
        sound.PlayOneShot(gameOverClip, 1);
        gameOverText.enabled = true;
        if (HighScore < Score)
        {
            SetHighScore(Score);
        }

        for (int i = 0; i < ghosts.Length; i++) {
            ghosts[i].gameObject.SetActive(false);
        }

        pacman.gameObject.SetActive(false);
    }

    private void SetLives(int lives)
    {
        this.lives = lives;
        livesText.text = "x" + lives.ToString();
    }
    private void SetLevel(int level)
    {
        this.level = level;
        levelText.text = level.ToString().PadLeft(2, '0');
    }

    private void SetHighScore(int highscore)
    {
        this.highscore = highscore;
        highScoreText.text = highscore.ToString();
    }

// --отображает на интерфесе кол-во очков--
    private void SetScore(int score)
    {
        this.score = score;
        scoreText.text = score.ToString().PadLeft(2, '0');
    }

    public void PacmanEaten()
    {
        sound.Stop();
        if (lives == 2)
        {
            sound.PlayOneShot(ghostDeathClip, 1);
            sound.PlayOneShot(notOverClip, 1);
        }
        else if (lives > 2)
        {
            sound.PlayOneShot(deathClip, 1);
        }
        pacman.DeathSequence();
// тыбзим для + 1 жизки в бонусе
        SetLives(lives - 1);

        if (lives > 0) {
            Invoke(nameof(ResetState), 3f);
        } else {
            GameOver();
        }
    }

    public void GhostEaten(Ghost ghost)
    {
        sound.PlayOneShot(ghostDeathClip, 1);
        int points = ghost.points * ghostMultiplier;
        SetScore(score + points);
// условие появления бонуса на карте (5 призраков для 1 бонуса)
        ghostMultiplier++;
        //if (ghostEaten == 5)
        //{
        //    cherry.Spawn();
        //}
        //if (ghostMultipier = 5){ неправильный код, нужно вызывать метод вишенки, в котором уже будет все это делаться
        //SetLives(lives + 1);
        //pellet.gameObject.SetActive(true);
        //}
        // if (ghostMultipier = 5)
        // NAAME.SetActive(True);
    }
// юзять завтра для бонуса , действие, дает + 1 жизнь SetLives(lives + 1);
    public void HPBonusEaten(HPBonus pellet) //САМЫЙ ПРАВДОПОДОБНЫЙ ВАРИК, НО ОН ТОК БЛОЧИТ ПРОХОД ДЛА ПАКМАНА
    {
        if (lives < 3)
        {
            SetLives(lives + 1);
            sound.PlayOneShot(thatsitClip, 1);
        }
        if (lives > 1)
        {
            gameMusicCalm.volume = 0.6f;
            gameMusicCombat.volume = 0;
        }
        else
        {
            gameMusicCombat.volume = 0.4f;
            gameMusicCalm.volume = 0;
        }
        PelletEaten(pellet);
    }
    public void PelletEaten(Pellet pellet)
    {
        sound.PlayOneShot(coinClip, 0.3f);
        pellet.gameObject.SetActive(false); // будет также 
        SetScore(score + pellet.points);
        if (!HasRemainingPellets())
        {
            sound.Stop();
            if (lives == 1)
            {
                SetScore(score + 1000);
                sound.PlayOneShot(niceTryClip, 1);
            }
            else
            {
                sound.Stop();
                sound.PlayOneShot(yesClip, 1);
            }
            pacman.gameObject.SetActive(false);
            Invoke(nameof(NewRound), 3f);
        }
    }

    public void PowerPelletEaten(PowerPellet pellet)
    {
        sound.Stop();
        if (lives < 2)
        {
            pacman.HealthRage(1.5f);
            sound.PlayOneShot(enoughClip, 1);
            sound.PlayOneShot(swordsClip, 1);
        }
        else
        {
            sound.PlayOneShot(rageClip, 0.5f);
        }
        for (int i = 0; i < ghosts.Length; i++) {
            ghosts[i].frightened.Enable(pellet.duration);
        }

        PelletEaten(pellet);
        CancelInvoke(nameof(ResetGhostMultiplier));
        Invoke(nameof(ResetGhostMultiplier), pellet.duration);
        //pacman.movement.speedMultiplier = 1f;
        //pacman.HealthRage(1f);
    }

    private bool HasRemainingPellets()
    {
        foreach (Transform pellet in pellets)
        {
            if (pellet.gameObject.activeSelf) {
                return true;
            }
            //if (pellet.gameObject.activeSelf) {
            //    return true;
            //}
        }

        return false;
    }

    private void ResetGhostMultiplier()
    {
        ghostMultiplier = 1;
    }
}
