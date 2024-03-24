using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.UI;
public class Scene : MonoBehaviour
{
    [SerializeField] private Text scoreText;
    private const string SaveFile = "Saves";
    private void Start()
    {
        scoreText.text = ReadFile(SaveFile);
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
            sw.WriteLine("00");
            sw.Close();
            var sr = File.OpenText(file);
            var text = sr.ReadLine();
            sr.Close();
            return text;
        }
    }

    public void PressButton()
    {
        SceneManager.LoadScene(1);
    }
}

