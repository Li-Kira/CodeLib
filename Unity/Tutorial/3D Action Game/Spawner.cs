 using System;
 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 using UnityEngine.Events;

 public class Spawner : MonoBehaviour
{
    private List<SpawnPoint> _spawnPointsList;
    private List<Character> _charactersList;
    private bool isSpawned;

    public Collider _Collider;

    public UnityEvent OnAllSpawnCharacterDead;
    
    private void Awake()
    {
        var spawnArray = transform.parent.GetComponentsInChildren<SpawnPoint>();
        _spawnPointsList = new List<SpawnPoint>(spawnArray);
        _charactersList = new List<Character>();
    }

    private void Update()
    {
        if (_charactersList.Count == 0 || !isSpawned)
        {
            return;
        }

        bool ALL_Spawn_Enermy_Dead = true;
        
        foreach (Character _character in _charactersList)
        {
            if (_character.CurrentState != Character.CharacterState.Dead)
            {
                ALL_Spawn_Enermy_Dead = false;
                break;
            }
            
        }
        //添加监听事件以及函数
        if (ALL_Spawn_Enermy_Dead)
        {
            if (OnAllSpawnCharacterDead != null)
            {
                OnAllSpawnCharacterDead.Invoke();
            }
            _charactersList.Clear();
        }
    }


    public void SpawnEnemys()
    {
        if (isSpawned)
            return;

        isSpawned = true;

        foreach (var _spawnPoints in _spawnPointsList)
        {
            if (_spawnPoints.EnemyToSpawn != null)
            {
                GameObject spawnedGameObject = Instantiate(_spawnPoints.EnemyToSpawn, _spawnPoints.transform.position,
                    _spawnPoints.transform.rotation);
                spawnedGameObject.transform.GetComponent<Character>().SwitchStateTo(Character.CharacterState.Spawn);
                _charactersList.Add(spawnedGameObject.GetComponent<Character>());
            }
        }


    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            SpawnEnemys();
            
        }
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_Collider.transform.position,_Collider.bounds.size);
    }
}
