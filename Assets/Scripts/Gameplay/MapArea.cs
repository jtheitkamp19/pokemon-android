using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapArea : MonoBehaviour
{
    [SerializeField] List<Pokemon> wildPokemon;

    public Pokemon GetRandomWildPokemon() {
        var wild = wildPokemon[Random.Range(0, wildPokemon.Count)];
        wild.Init();
        return wild;
    }
}
