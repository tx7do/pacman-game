using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameObject pacman;
    public GameObject blinky;
    public GameObject clyde;
    public GameObject inky;
    public GameObject pinky;
    public GameObject startPanel;
    public GameObject gamePanel;
    public GameObject startCountDownPrefab;
    public GameObject gameoverPrefab;
    public GameObject winPrefab;
    public AudioClip startClip;
    public Text remainText;
    public Text nowText;
    public Text scoreText;

    public bool isSuperPacman;
    public List<int> usingIndex = new List<int>();
    public List<int> rawIndex = new List<int> {0, 1, 2, 3};
    private readonly List<GameObject> _pacdotGos = new List<GameObject>();
    private int _pacdotNum;
    private int _nowEat;
    public int score;

    private void Awake()
    {
        Instance = this;
        Screen.SetResolution(1024, 768, false);
        var tempCount = rawIndex.Count;
        for (var i = 0; i < tempCount; i++)
        {
            var tempIndex = Random.Range(0, rawIndex.Count);
            usingIndex.Add(rawIndex[tempIndex]);
            rawIndex.RemoveAt(tempIndex);
        }

        foreach (Transform t in GameObject.Find("Maze").transform)
        {
            _pacdotGos.Add(t.gameObject);
        }

        _pacdotNum = GameObject.Find("Maze").transform.childCount;
    }

    private void Start()
    {
        SetGameState(false);
    }

    private void Update()
    {
        if (_nowEat == _pacdotNum && pacman.GetComponent<PacmanMove>().enabled)
        {
            gamePanel.SetActive(false);
            Instantiate(winPrefab);
            StopAllCoroutines();
            SetGameState(false);
        }

        if (_nowEat == _pacdotNum)
        {
            const int sceneBuildIndex = 0;
            if (Input.anyKeyDown)
            {
                SceneManager.LoadScene(sceneBuildIndex);
            }
        }

        if (gamePanel.activeInHierarchy)
        {
            remainText.text = "Remain:\n\n" + (_pacdotNum - _nowEat);
            nowText.text = "Eaten:\n\n" + _nowEat;
            scoreText.text = "Score:\n\n" + score;
        }
    }

    public void OnStartButton()
    {
        StartCoroutine(PlayStartCountDown());
        AudioSource.PlayClipAtPoint(startClip, new Vector3(0, 0, -5));
        startPanel.SetActive(false);
    }

    public void OnExitButton()
    {
        Application.Quit();
    }

    private IEnumerator PlayStartCountDown()
    {
        var go = Instantiate(startCountDownPrefab);
        yield return new WaitForSeconds(4f);
        Destroy(go);
        SetGameState(true);
        Invoke("CreateSuperPacdot", 10f);
        gamePanel.SetActive(true);
        GetComponent<AudioSource>().Play();
    }

    public void OnEatPacdot(GameObject go)
    {
        _nowEat++;
        score += 100;
        _pacdotGos.Remove(go);
    }

    public void OnEatSuperPacdot()
    {
        score += 200;
        Invoke(nameof(CreateSuperPacdot), 10f);
        isSuperPacman = true;
        FreezeEnemy();
        StartCoroutine(RecoveryEnemy());
    }

    private IEnumerator RecoveryEnemy()
    {
        yield return new WaitForSeconds(3f);
        DisFreezeEnemy();
        isSuperPacman = false;
    }

    private void CreateSuperPacdot()
    {
        if (_pacdotGos.Count < 5)
        {
            return;
        }

        int tempIndex = Random.Range(0, _pacdotGos.Count);
        _pacdotGos[tempIndex].transform.localScale = new Vector3(3, 3, 3);
        _pacdotGos[tempIndex].GetComponent<Pacdot>().isSuperPacdot = true;
    }

    private void FreezeEnemy()
    {
        blinky.GetComponent<GhostMove>().enabled = false;
        clyde.GetComponent<GhostMove>().enabled = false;
        inky.GetComponent<GhostMove>().enabled = false;
        pinky.GetComponent<GhostMove>().enabled = false;
        blinky.GetComponent<SpriteRenderer>().color = new Color(0.7f, 0.7f, 0.7f, 0.7f);
        clyde.GetComponent<SpriteRenderer>().color = new Color(0.7f, 0.7f, 0.7f, 0.7f);
        inky.GetComponent<SpriteRenderer>().color = new Color(0.7f, 0.7f, 0.7f, 0.7f);
        pinky.GetComponent<SpriteRenderer>().color = new Color(0.7f, 0.7f, 0.7f, 0.7f);
    }

    private void DisFreezeEnemy()
    {
        blinky.GetComponent<GhostMove>().enabled = true;
        clyde.GetComponent<GhostMove>().enabled = true;
        inky.GetComponent<GhostMove>().enabled = true;
        pinky.GetComponent<GhostMove>().enabled = true;
        blinky.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
        clyde.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
        inky.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
        pinky.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
    }

    private void SetGameState(bool state)
    {
        pacman.GetComponent<PacmanMove>().enabled = state;
        blinky.GetComponent<GhostMove>().enabled = state;
        clyde.GetComponent<GhostMove>().enabled = state;
        inky.GetComponent<GhostMove>().enabled = state;
        pinky.GetComponent<GhostMove>().enabled = state;
    }
}