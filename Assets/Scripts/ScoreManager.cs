using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private Text _scoreText;

    private int _scoreValue;
    private string _playerPrefsKey = "HighScore";

    private void Start()
    {
        _scoreValue = 0;
        UpdateScoreText();
        PlayerPrefs.DeleteAll();

        if(!PlayerPrefs.HasKey(_playerPrefsKey))
        {
            PlayerPrefs.SetInt(_playerPrefsKey, 0);
        }
    }

    public void AddScore(int linesCleared)
    {
        if(linesCleared == 0)
            return;
        if (linesCleared == 1)
        {
            _scoreValue += 100;
            UpdateScoreText();
            return;
        }

        _scoreValue += ((linesCleared - 1) * 50) + linesCleared * 100;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        _scoreText.text = _scoreValue.ToString();
    }

    public bool GameOver()
    {
        if (_scoreValue <= PlayerPrefs.GetInt(_playerPrefsKey))
        {
            _scoreValue = 0;
            UpdateScoreText();
            return false;
        }

        PlayerPrefs.SetInt(_playerPrefsKey, _scoreValue);
        PlayerPrefs.Save();

        SceneManager.LoadScene("Highscore");
        return true;
    }
}
