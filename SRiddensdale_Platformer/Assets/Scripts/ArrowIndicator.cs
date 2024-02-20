using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class handles the arrow indicator for when the bubble is not on screen
/// </summary>
public class ArrowIndicator : MonoBehaviour
{
    private enum Direction { Left = 0, Right = 1, Up = 2, Down = 3 }

    [SerializeField]
    private Vector2 _arrowBounds;
    [SerializeField]
    private GameObject[] _arrows;

    // private references
    private Hankling hankling;
    private Player player;
    private bool showArrows;

    private Direction dir = Direction.Left;

    void Start()
    {
        // get references
        player = GameManager.Instance.LocalPlayer;
        hankling = FindObjectOfType<Hankling>();

        // subscribe to events
        hankling.OnBubbleEnter += () => showArrows = true;
        hankling.OnBubblePopped += () => showArrows = false;
        GameManager.Instance.OnGameOver += () => showArrows = false;
    }

    void Update()
    {
        // ensure the center of all arrows is constantly at the camera position
        transform.position = new Vector2(Camera.main.transform.position.x, Camera.main.transform.position.y);

        // update arrows to be invis
        if (!showArrows) {
            UpdateArrows(-1);
            return;
        }

        // record the last direction for comparison
        Direction lastDir = dir;

        bool hit = false;
        // Check each possible position and display the corresponding arrow
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
        else if(hankling.transform.position.y < Camera.main.transform.position.y - _arrowBounds.y) {
            dir = Direction.Up;
            _arrows[(int)dir].transform.position = new Vector2(hankling.transform.position.x, _arrows[(int)dir].transform.position.y);
            hit = true;
        }
        else if(hankling.transform.position.y > Camera.main.transform.position.y + _arrowBounds.y) {
            dir = Direction.Down;
            _arrows[(int)dir].transform.position = new Vector2(hankling.transform.position.x, _arrows[(int)dir].transform.position.y);
            hit = true;
        }

        // clamp the position of the corresponding arrow
        _arrows[(int)dir].transform.position = new Vector2(
            Mathf.Clamp(_arrows[(int)dir].transform.position.x, Camera.main.transform.position.x - _arrowBounds.x + 2, Camera.main.transform.position.x + _arrowBounds.x - 2),
            Mathf.Clamp(_arrows[(int)dir].transform.position.y, Camera.main.transform.position.y - _arrowBounds.y + 2, Camera.main.transform.position.y + _arrowBounds.y - 2));

        // update arrows to display
        UpdateArrows((int)dir);
        if (!hit) UpdateArrows(-1);
    }

    /// <summary>
    /// Displays only the arrow index provided
    /// </summary>
    /// <param name="value"></param>
    private void UpdateArrows(int value)
    {
        for (int i = 0; i < _arrows.Length; i++)
        {
            if (i == value) _arrows[i].SetActive(true);
            else _arrows[i].SetActive(false);
        }
    }
}
