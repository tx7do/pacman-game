using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GhostMove : MonoBehaviour
{
    public GameObject[] wayPointsGos;
    public float speed = 0.2f;
    private readonly List<Vector3> _wayPoints = new List<Vector3>();
    private int _index;
    private Vector3 _startPos;
    private static readonly int DirX = Animator.StringToHash("DirX");
    private static readonly int DirY = Animator.StringToHash("DirY");

    private void Start()
    {
        _startPos = transform.position + new Vector3(0, 3, 0);
        LoadAPath(wayPointsGos[GameManager.Instance.usingIndex[GetComponent<SpriteRenderer>().sortingOrder - 2]]);
    }

    private void FixedUpdate()
    {
        if (transform.position != _wayPoints[_index])
        {
            var temp = Vector2.MoveTowards(transform.position, _wayPoints[_index], speed);
            GetComponent<Rigidbody2D>().MovePosition(temp);
        }
        else
        {
            _index++;
            if (_index >= _wayPoints.Count)
            {
                _index = 0;
                LoadAPath(wayPointsGos[Random.Range(0, wayPointsGos.Length)]);
            }
        }

        Vector2 dir = _wayPoints[_index] - transform.position;
        GetComponent<Animator>().SetFloat(DirX, dir.x);
        GetComponent<Animator>().SetFloat(DirY, dir.y);
    }

    private void LoadAPath(GameObject go)
    {
        _wayPoints.Clear();
        foreach (Transform t in go.transform)
        {
            _wayPoints.Add(t.position);
        }

        _wayPoints.Insert(0, _startPos);
        _wayPoints.Add(_startPos);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name != "Pacman") return;

        if (GameManager.Instance.isSuperPacman)
        {
            transform.position = _startPos - new Vector3(0, 3, 0);
            _index = 0;
            GameManager.Instance.score += 500;
        }
        else
        {
            collision.gameObject.SetActive(false);
            GameManager.Instance.gamePanel.SetActive(false);
            Instantiate(GameManager.Instance.gameoverPrefab);
            Invoke(nameof(ReStart), 3f);
        }
    }

    private void ReStart()
    {
        SceneManager.LoadScene(sceneBuildIndex: 0);
    }
}