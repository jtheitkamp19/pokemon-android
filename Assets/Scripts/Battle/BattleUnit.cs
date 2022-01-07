using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleUnit : MonoBehaviour
{
    const float PRE_ENTER_PLAYER_X = -488f;
    const float PRE_ENTER_ENEMY_X = 500f;

    [SerializeField] bool isPlayerUnit;
    [SerializeField] BattleHud hud;

    public BattleHud Hud {
        get { return hud; }
    }

    public Pokemon Pokemon {
        get; set;
    }

    public bool IsPlayerUnit {
        get { return isPlayerUnit; }
    }

    Image image;
    //-188, -100
    Vector3 originalPos;
    Color originalColor;

    private void Awake() {
        image = GetComponent<Image>();
        originalPos = image.transform.localPosition;
        originalColor = image.color;
    }

    public void Clear() {
        hud.gameObject.SetActive(false);
    }

    public void Setup(Pokemon pokemon) {
        Pokemon = pokemon;
        if (isPlayerUnit) {
            image.sprite = Pokemon.Base.BackSprite;
        } else {
            image.sprite = Pokemon.Base.FrontSprite;
        }

        hud.SetData(pokemon);
        hud.gameObject.SetActive(true);

        image.color = originalColor;
        PlayEnterAnimation();
    }

    public void PlayEnterAnimation() {
        if (isPlayerUnit) {
            image.transform.localPosition = new Vector3(PRE_ENTER_PLAYER_X, originalPos.y);
        } else {
            image.transform.localPosition = new Vector3(PRE_ENTER_ENEMY_X, originalPos.y);
        }

        image.transform.DOLocalMoveX(originalPos.x, 1f);
    }

    public void PlayAttackAnimation() {
        var sequence = DOTween.Sequence();
        
        if (isPlayerUnit) {
            sequence.Append(image.transform.DOLocalMoveX(originalPos.x + 50f, 0.25f));
        } else {
            sequence.Append(image.transform.DOLocalMoveX(originalPos.x - 50f, 0.25f));
        }

        sequence.Append(image.transform.DOLocalMoveX(originalPos.x, 0.25f));
    }

    public void PlayHitAnimation() {
        var sequence = DOTween.Sequence();

        sequence.Append(image.DOColor(Color.gray, 0.1f));
        sequence.Append(image.DOColor(originalColor, 0.1f));
    }

    public void PlayFaintAnimation() {
        var sequence = DOTween.Sequence();

        sequence.Append(image.transform.DOLocalMoveY(originalPos.y - 150f, 0.5f));
        sequence.Join(image.DOFade(0f, 0.5f));
    }
}
