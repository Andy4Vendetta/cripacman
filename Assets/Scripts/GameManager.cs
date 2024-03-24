using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
using System.IO;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    private const string SaveFile = "Saves";
    
    [SerializeField] private Ghost[] ghosts;
    [SerializeField] private Pacman pacman;
    [SerializeField] private Transform pellets;
    [SerializeField] private Text gameOverText;
    [SerializeField] private Text scoreText;
    [SerializeField] private Text livesText;
    [SerializeField] private Text highScoreText;
    [SerializeField] private Text levelText;
    
    [SerializeField] private AudioSource sound;
    [SerializeField] private AudioSource gameMusicCalm;
    [SerializeField] private AudioSource gameMusicCombat;

    [SerializeField] private AudioClip coinClip;
    [SerializeField] private AudioClip rageClip;
    [SerializeField] private AudioClip deathClip;
    [SerializeField] private AudioClip ghostDeathClip;
    [SerializeField] private AudioClip gameOverClip;
    [SerializeField] private AudioClip notOverClip;
    [SerializeField] private AudioClip calmClip;
    [SerializeField] private AudioClip combatClip;
    [SerializeField] private AudioClip enoughClip;
    [SerializeField] private AudioClip yesClip;
    [SerializeField] private AudioClip niceTryClip;
    [SerializeField] private AudioClip swordsClip;
    [SerializeField] private AudioClip thatsItClip;

    private int ghostMultiplier = 1;
    private int level = 1;
    private int bloomInt;
    [SerializeField] private PostProcessVolume mVolume;
    private Bloom mBloom;
    private Vignette mVignette;
    private ChromaticAberration mChroma;
    [SerializeField] private PostProcessProfile postProcProf;
    private int HighScore { get; set; }
    private int Lives { get; set; } = 3;
    private int Score { get; set; }
    private void Awake()
    {
        if (Instance != null) {
            DestroyImmediate(gameObject);
        } else {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    private static string ReadFile(string file)
    {
        if (File.Exists(file))
        {
            var sr = File.OpenText(file);
            var text = sr.ReadLine();
            sr.Close();
            return text;
        }
        else
        {
            var sw = File.CreateText(file);
            sw.WriteLine("0");
            sw.Close();
            var sr = File.OpenText(file);
            var text = sr.ReadLine();
            sr.Close();
            return text;
        }
    }
    private static void WriteFile(string file, int hs)
    {
        var sw = File.CreateText(file);
        sw.Flush();
        sw.WriteLine(hs);
        sw.Close();
    }
    private void Start()
    {
        mBloom = postProcProf.GetSetting<Bloom>();
        mChroma = postProcProf.GetSetting<ChromaticAberration>();
        NewGame();
        HighScore = int.Parse(ReadFile(SaveFile));
        highScoreText.text = ReadFile(SaveFile);
        gameMusicCalm.clip = calmClip;
        gameMusicCombat.clip = combatClip;
        gameMusicCalm.loop = true;
        gameMusicCombat.loop = true;
        gameMusicCalm.volume = 0.6f;
        gameMusicCombat.volume = 0;
        gameMusicCalm.Play();
        gameMusicCombat.Play();
    }
    private void Update()
    {
        if (Lives <= 0 && Input.anyKeyDown) {
            // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
            NewGame();
        }
    }
    // ReSharper disable Unity.PerformanceAnalysis
    private void NewGame()
    {
        SetScore(0);
        SetLives(3);
        NewRound();
        SetLevel(1);
        mBloom.intensity.value = 0;
        bloomInt = 0;
        foreach (var ghost in ghosts)
        {
            ghost.Movement.speed = 7f;
        }
        gameMusicCalm.volume = 0.6f;
        gameMusicCombat.volume = 0;
    }
    private void NewRound()
    {
        NormalEffects();
        gameOverText.enabled = false;
        foreach (Transform pellet in pellets) {
            pellet.gameObject.SetActive(true);
        }
        foreach (var ghost in ghosts)
        {
            ghost.Movement.speed += 0.5f;
        }
        SetLevel(level + 1);
        ResetState();
    }
    private void ResetState()
    {
        sound.Stop();
        foreach (var t in ghosts)
        {
            t.ResetState();
        }
        pacman.ResetState();
    }
    private void GameOver()
    {
        sound.PlayOneShot(gameOverClip, 1);
        gameOverText.enabled = true;
        NormalEffects();
        if (HighScore < Score)
        {
            SetHighScore(Score);
        }
        foreach (var t in ghosts)
        {
            t.gameObject.SetActive(false);
        }
        pacman.gameObject.SetActive(false);
    }
    private void SetLives(int lives)
    {
        Lives = lives;
        livesText.text = "x" + lives;
    }
    private void SetLevel(int level1)
    {
        level = level1;
        levelText.text = level1.ToString().PadLeft(2, '0');
    }
    private void SetHighScore(int highscore)
    {
        HighScore = highscore;
        highScoreText.text = highscore.ToString();
        WriteFile(SaveFile, highscore);
    }
    private void SetScore(int score)
    {
        Score = score;
        scoreText.text = score.ToString().PadLeft(2, '0');
    }
    public void PacmanEaten()
    {
        sound.Stop();
        switch (Lives)
        {
            case 2:
                sound.PlayOneShot(ghostDeathClip, 1);
                sound.PlayOneShot(notOverClip, 1);
                mBloom.intensity.value = 20;
                bloomInt = 20;
                gameMusicCombat.volume = 0.4f;
                gameMusicCalm.volume = 0;
                break;
            case > 2:
                sound.PlayOneShot(deathClip, 1);
                break;
        }
        pacman.DeathSequence();
        SetLives(Lives - 1);
        NormalEffects();
        if (Lives > 0) {
            Invoke(nameof(ResetState), 3f);
        } else {
            GameOver();
        }
    }
    public void GhostEaten(Ghost ghost)
    {
        sound.PlayOneShot(ghostDeathClip, 1);
        var points = ghost.points * ghostMultiplier;
        SetScore(Score + points);
        ghostMultiplier++;
    }
    public void HpBonusEaten(HpBonus pellet)
    {
        if (Lives < 3)
        {
            SetLives(Lives + 1);
            sound.PlayOneShot(thatsItClip, 1);
        }
        if (Lives > 1)
        {
            mBloom.intensity.value = 0;
            bloomInt = 0;
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
        pellet.gameObject.SetActive(false);
        SetScore(Score + pellet.points);
        if (HasRemainingPellets()) return;
        sound.Stop();
        NormalEffects();
        if (Lives == 1)
        {
            SetScore(Score + 5000);
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
    public void PowerPelletEaten(PowerPellet pellet)
    {
        sound.Stop();
        if (Lives < 2)
        {
            pacman.HealthRage(1.5f);
            CancelInvoke(nameof(RageEffect));
            Invoke(nameof(RageEffect), 0.8f);
            sound.PlayOneShot(enoughClip, 1);
            sound.PlayOneShot(swordsClip, 1);
        }
        else
        {
            sound.PlayOneShot(rageClip, 0.5f);
        }
        foreach (var t in ghosts)
        {
            t.Frightened.Enable(pellet.duration);
        }
        PelletEaten(pellet);
        CancelInvoke(nameof(ResetGhostMultiplier));
        Invoke(nameof(ResetGhostMultiplier), pellet.duration);
    }
    private void RageEffect()
    {
        if (pacman)
        {
            mBloom.intensity.value = 30;
            mChroma.intensity.value = 1;
        }
        CancelInvoke(nameof(NormalEffects));
        Invoke(nameof(NormalEffects), 6.2f);
    }
    private void NormalEffects()
    {
        mBloom.intensity.value = bloomInt;
        mChroma.intensity.value = 0;
    }
    private bool HasRemainingPellets()
    {
        return pellets.Cast<Transform>().Any(pellet => pellet.gameObject.activeSelf);
    }
    private void ResetGhostMultiplier()
    {
        ghostMultiplier = 1;
    }
}