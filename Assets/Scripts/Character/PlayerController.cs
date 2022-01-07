using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Sprite sprite;
    [SerializeField] string name;

    public event Action OnEncountered;
    public event Action<Collider2D> OnEnterTrainersView;

    private Vector2 input;

    private Character character;

    public Sprite Sprite {
        get { return sprite; }
    }

    public string Name {
        get { return name; }
    }

    private void Awake() {
        character = GetComponent<Character>();
    }

    // Update is called once per frame
    public void HandleUpdate()
    {
        if (!character.IsMoving) {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            // Remove diagonals from movement
            if (input.x != 0) {
                input.y = 0; 
            }

            if (input != Vector2.zero) {
                StartCoroutine(character.Move(input, OnMoveOver));
            }
        }

        character.HandleUpdate();

        if (Input.GetKeyDown(KeyCode.Z)) {
            Interact();
        }
    }

    void Interact() {
        var facingDir = new Vector3(character.Animator.MoveX, character.Animator.MoveY);
        var interactPos = transform.position + facingDir;

        Debug.DrawLine(transform.position, interactPos, Color.green, 0.5f);

        var collider = Physics2D.OverlapCircle(interactPos, 0.3f, GameLayers.Instance.InteractableLayer);

        if (collider != null) {
            collider.GetComponent<Interactable>()?.Interact(transform);
        }
    }

    private void OnMoveOver() {
        CheckForEncounters();
        CheckIfInTrainerView();
    }

    private void CheckForEncounters() {
        if (Physics2D.OverlapCircle(transform.position, .2f, GameLayers.Instance.GrassLayer) != null) {
            if (UnityEngine.Random.Range(1, 101) <= 10) {
                Debug.Log("encountered a wild pokemon");
                character.Animator.IsMoving = false;
                OnEncountered();
            }
        }
    }

    private void CheckIfInTrainerView() {
        Collider2D collider = Physics2D.OverlapCircle(transform.position, .2f, GameLayers.Instance.FovLayer);

        if (collider != null) {
            character.Animator.IsMoving = false;
            OnEnterTrainersView(collider);
        }
    }
}
