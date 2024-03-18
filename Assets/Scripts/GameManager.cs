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
    }

    private void NewRound()
    {
        gameOverText.enabled = false;

        foreach (Transform pellet in pellets) {
            pellet.gameObject.SetActive(true);
        }

        SetLevel(level + 1);
        ResetState();
    }

    private void ResetState()
    {
        for (int i = 0; i < ghosts.Length; i++) {
            ghosts[i].ResetState();
        }

        pacman.ResetState();
    }

    private void GameOver()
    {
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
        PelletEaten(pellet);
        if (lives < 3)
        {
            SetLives(lives + 1);
        }
    }
    public void PelletEaten(Pellet pellet)
    {
        pellet.gameObject.SetActive(false); // будет также 
        //SetLives(lives + 1);

        SetScore(score + pellet.points);
// уже не надо, до свидания :)
        if (!HasRemainingPellets())
        {
            pacman.gameObject.SetActive(false);
            Invoke(nameof(NewRound), 3f);
        }
    }

    public void PowerPelletEaten(PowerPellet pellet)
    {
        for (int i = 0; i < ghosts.Length; i++) {
            ghosts[i].frightened.Enable(pellet.duration);
        }

        PelletEaten(pellet);
        CancelInvoke(nameof(ResetGhostMultiplier));
        Invoke(nameof(ResetGhostMultiplier), pellet.duration);
    }

    private bool HasRemainingPellets()
    {
        foreach (Transform pellet in pellets)
        {
            if (pellet.gameObject.activeSelf) {
                return true;
            }
        }

        return false;
    }

    private void ResetGhostMultiplier()
    {
        ghostMultiplier = 1;
    }
}
