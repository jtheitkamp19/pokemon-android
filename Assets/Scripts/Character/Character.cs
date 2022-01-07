using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public float moveSpeed;
    CharacterAnimator animator;

    public bool IsMoving {
        get; private set;
    }

    public CharacterAnimator Animator {
        get { return animator; }
    }

    private void Awake() {
        animator = GetComponent<CharacterAnimator>();
    }

    public IEnumerator Move(Vector2 moveVector, Action OnMoveOver=null) {
        animator.MoveX = Mathf.Clamp(moveVector.x, -1f, 1f);
        animator.MoveY = Mathf.Clamp(moveVector.y, -1f, 1f);

        var targetPos = transform.position;
        targetPos.x += moveVector.x;
        targetPos.y += moveVector.y;

        if (!IsPathClear(targetPos))
            yield break;

        IsMoving = true;

        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon) {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;
        IsMoving = false;

        OnMoveOver?.Invoke();
    }

    public void HandleUpdate() {
        animator.IsMoving = IsMoving;
    }

    private bool IsPathClear(Vector3 targetPos) {
        var diff = targetPos - transform.position;
        var dir = diff.normalized;

        // Start the collision box one tile ahead of the character in the dir direction to prevent collision with self
        if (Physics2D.BoxCast(transform.position + dir, new Vector2(0.2f, 0.2f), 0f, dir, diff.magnitude - 1, GameLayers.Instance.SettingLayer | GameLayers.Instance.InteractableLayer | GameLayers.Instance.PlayerLayer)) {
            return false;
        }

        return true;
    }

    public void LookTowards(Vector3 targetPos) {
        var xDiff = Mathf.Floor(targetPos.x) - Mathf.Floor(transform.position.x);
        var yDiff = Mathf.Floor(targetPos.y) - Mathf.Floor(transform.position.y);

        if (xDiff == 0 || yDiff == 0) {
            animator.MoveX = Mathf.Clamp(xDiff, -1f, 1f);
            animator.MoveY = Mathf.Clamp(yDiff, -1f, 1f);
        } else {
            Debug.LogError("Error in Look Towards: Character can not look diagonally.");
        }
    }

    private bool IsWalkable(Vector3 targetPos) {
        if (Physics2D.OverlapCircle(targetPos, .2f, GameLayers.Instance.SettingLayer | GameLayers.Instance.InteractableLayer) != null) {
            return false;
        }
        return true;
    }
}
