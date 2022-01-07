using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour, Interactable
{
    [SerializeField] Dialog dialog;
    [SerializeField] List<Vector2> movementPattern;
    [SerializeField] float timeBetweenPattern;

    NPCState state;
    float idleTimer = 0f;
    int currentMovementPattern = 0;

    Character character;

    public void Awake() {
        character = GetComponent<Character>();
    }

    public void Interact(Transform initiator) {
        if (state == NPCState.Idle) {
            state = NPCState.Dialog;
            Debug.Log("Interacting with npc");
            character.LookTowards(initiator.position);

            StartCoroutine(DialogManager.Instance.ShowDialog(dialog, () => {
                idleTimer = 0;
                state = NPCState.Idle;
            }));
        }
    }

    private void Update() {
        if (state == NPCState.Idle) {
            idleTimer += Time.deltaTime;
            if (idleTimer > timeBetweenPattern) {
                idleTimer = 0f;
                if (movementPattern.Count > 0) {
                    StartCoroutine(Walk());
                }
            }
        }

        character.HandleUpdate();
    }

    IEnumerator Walk() {
        state = NPCState.Walking;
        var oldPos = transform.position;

        yield return character.Move(movementPattern[currentMovementPattern]);

        if (transform.position != oldPos)
            currentMovementPattern = (currentMovementPattern + 1) % movementPattern.Count;

        state = NPCState.Idle;
    }
}

public enum NPCState { Idle, Walking, Dialog }
