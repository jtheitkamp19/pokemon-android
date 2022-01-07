using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyMemberUI : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] HPBar hpBar;

    [SerializeField] Color highlightColor;

    Pokemon _pokemon;

    public void SetData(Pokemon pokemon) {
        nameText.text = pokemon.Base.Name;
        levelText.text = "Lvl " + pokemon.Level;
        hpBar.SetHP((float)pokemon.HP / pokemon.MaxHp);

        _pokemon = pokemon;
    }

    public void SetSelected(bool selected) {
        if (selected) {
            nameText.color = highlightColor;
        } else {
            nameText.color = Color.black;
        }
    }
}
