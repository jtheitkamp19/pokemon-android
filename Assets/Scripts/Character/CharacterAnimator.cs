using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    [SerializeField] List<Sprite> walkDownSprites;
    [SerializeField] List<Sprite> walkUpSprites;
    [SerializeField] List<Sprite> walkRightSprites;
    [SerializeField] List<Sprite> walkLeftSprites;
    [SerializeField] FacingDirection defaultDirection = FacingDirection.Down;

    public FacingDirection DefaultDirection {
        get { return defaultDirection; }
    }

    public float MoveX {
        get; set;
    }

    public float MoveY {
        get; set;
    }

    public bool IsMoving {
        get; set;
    }

    SpriteAnimator walkDownAnim;
    SpriteAnimator walkUpAnim;
    SpriteAnimator walkRightAnim;
    SpriteAnimator walkLeftAnim;

    SpriteAnimator currentAnim;
    bool wasPreviouslyMoving;

    SpriteRenderer spriteRenderer;

    private void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        walkDownAnim = new SpriteAnimator(walkDownSprites, spriteRenderer);
        walkUpAnim = new SpriteAnimator(walkUpSprites, spriteRenderer);
        walkRightAnim = new SpriteAnimator(walkRightSprites, spriteRenderer);
        walkLeftAnim = new SpriteAnimator(walkLeftSprites, spriteRenderer);

        SetFacingDirection(defaultDirection);

        currentAnim = walkDownAnim;
    }

    private void Update() {
        var prevAnim = currentAnim;

        if (MoveX == 1) {
            currentAnim = walkRightAnim;
        } else if (MoveX == -1) {
            currentAnim = walkLeftAnim;
        } else if (MoveY == 1) {
            currentAnim = walkUpAnim;
        } else if (MoveY == -1) {
            currentAnim = walkDownAnim;
        }

        if (currentAnim != prevAnim || IsMoving != wasPreviouslyMoving) {
            currentAnim.Start();
        }

        if (IsMoving) {
            currentAnim.HandleUpdate();
        } else {
            spriteRenderer.sprite = currentAnim.Frames[0];
        }

        wasPreviouslyMoving = IsMoving;
    }

    public void SetFacingDirection(FacingDirection direction) {
        if (direction == FacingDirection.Right)
            MoveX = 1;
        else if (direction == FacingDirection.Left)
            MoveX = -1;
        else if (direction == FacingDirection.Down)
            MoveY = -1;
        else
            MoveY = 1;
    }
}

public enum FacingDirection { Up, Down, Left, Right }
