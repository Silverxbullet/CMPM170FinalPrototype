using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using PrototypeFinalProject;

public class EnemySpawner : MonoBehaviour
{
    public List<GameObject> enemy_prefabs = new List<GameObject>();
    public List<BurgerLayerSO> burger_layers = new List<BurgerLayerSO>();
    public HorizontalWrapArena arena;
    public List<Transform> enemy_spawns = new List<Transform>();
    public float spawn_interval = 3f;
    public int min_layers = 2;
    public int max_layers = 4;

    public void Start() {
        StartCoroutine(TimedEnemySpawn());
    }

    IEnumerator TimedEnemySpawn() {
        yield return new WaitForSeconds(spawn_interval);
        SpawnEnemy();
        
        StartCoroutine(TimedEnemySpawn());
    }

    private void SpawnEnemy() {
        GameObject spawned_enemy = Instantiate(enemy_prefabs[Random.Range(0, enemy_prefabs.Count)]);
        List<BurgerLayerSO> spawned_enemy_layers = new List<BurgerLayerSO>();
        for (int i = 0; i < Random.Range(min_layers, max_layers + 1); i++) {
            spawned_enemy_layers.Add(burger_layers[Random.Range(0, burger_layers.Count)]);
        }
        spawned_enemy.GetComponent<CharacterBase>().Init(spawned_enemy_layers);
        spawned_enemy.GetComponent<ScreenWrapTarget2D>().Arena = this.arena;
        spawned_enemy.transform.position = enemy_spawns[Random.Range(0, enemy_spawns.Count)].position;
    }

}

