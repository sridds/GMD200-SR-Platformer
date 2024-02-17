using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowIndicator : MonoBehaviour
{
    private enum Direction { Left = 0, Right = 1, Up = 2, Down = 3 }

    [SerializeField]
    private Vector2 _arrowBounds;
    [SerializeField]
    private GameObject[] _arrows;

    private Hankling hankling;
    private Player player;
    private bool showArrows;

    private Direction dir = Direction.Left;

    void Start()
    {
        player = GameManager.Instance.LocalPlayer;
        hankling = FindObjectOfType<Hankling>();

        hankling.OnBubbleEnter += () => showArrows = true;
        hankling.OnBubblePopped += () => showArrows = false;
        GameManager.Instance.OnGameOver += () => showArrows = false;
    }

    void Update()
    {
        transform.position = new Vector2(Camera.main.transform.position.x, Camera.main.transform.position.y);

        if (!showArrows)
        {
            UpdateArrows(-1);
            return;
        }
        Direction lastDir = dir;

        bool hit = false;

        if(hankling.transform.position.x > Camera.main.transform.position.x + _arrowBounds.x) {
            dir = Direction.Left;
            _arrows[(int)dir].transform.position = new Vector2(_arrows[(int)dir].transform.position.x, hankling.transform.position.y);
            hit = true;
        }
        else if(hankling.transform.position.x < Camera.main.transform.position.x - _arrowBounds.x) {
            dir = Direction.Right;
            _arrows[(int)dir].transform.position = new Vector2(_arrows[(int)dir].transform.position.x, hankling.transform.position.y);
            hit = true;
        }
        if(hankling.transform.position.y < Camera.main.transform.position.y - _arrowBounds.y) {
            dir = Direction.Up;
            _arrows[(int)dir].transform.position = new Vector2(hankling.transform.position.x, _arrows[(int)dir].transform.position.y);
            hit = true;
        }
        else if(hankling.transform.position.y > Camera.main.transform.position.y + _arrowBounds.y) {
            dir = Direction.Down;
            _arrows[(int)dir].transform.position = new Vector2(hankling.transform.position.x, _arrows[(int)dir].transform.position.y);
            hit = true;
        }

        UpdateArrows((int)dir);
        if (!hit) UpdateArrows(-1);
    }

    private void UpdateArrows(int value)
    {
        for (int i = 0; i < _arrows.Length; i++)
        {
            if (i == value) _arrows[i].SetActive(true);
            else _arrows[i].SetActive(false);
        }
    }
}
