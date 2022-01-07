using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PokemonParty : MonoBehaviour
{
    [SerializeField] List<Pokemon> party;

    public List<Pokemon> Pokemon {
        get { return party; }
    }

    public Pokemon GetHealthyPokemon() {
        return party.Where(x => x.HP > 0).FirstOrDefault();
    }

    private void Start() {
        foreach (var pokemon in party) {
            pokemon.Init();
        }
    }
}
