using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class HighscoreScreen : MonoBehaviour
{
    [SerializeField] private Text _textPart1;
    [SerializeField] private Text _textPart2;
    [SerializeField] private Text _textPart3;

    private bool _animationOver = false;

    private void Start()
    {
        _textPart3.text = PlayerPrefs.GetInt("HighScore").ToString();

        Sequence sequence = DOTween.Sequence(); 

        sequence.Join(_textPart1.DOFade(0f, 0f));
        sequence.Join(_textPart2.DOFade(0f, 0f));
        sequence.Join(_textPart3.DOFade(0f, 0f));

        sequence.Join(_textPart1.DOFade(1f, 4f));
        sequence.Join(_textPart2.DOFade(1f, 4f));
        sequence.Join(_textPart3.DOFade(1f, 4f));

        sequence.OnComplete(() => _animationOver = true);
    }

    private void Update()
    {
        if(Input.anyKey && _animationOver)
        {
            SceneManager.LoadScene("Tetris");
        }
    }
}
