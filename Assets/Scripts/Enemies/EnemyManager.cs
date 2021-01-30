using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemyManager : SystemSingleton<EnemyManager>
{
    public Transform[] SpawnLocations;
    private GameManager _gm;
    public GameObject GargoylePrefab;
    public Transform PlayerPosition;

    public Transform Target1;
    public Transform Target2;

    private bool _isPlayerSet = false;

    public int MaxGargoyles = 10;
    private List<GameObject> _gargoyles = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        GargoylePrefab = Resources.Load<GameObject>("Gargoyle");
        _gm = GameManager.Get();
        StartCoroutine(SpawnGargoyles());
    }

    IEnumerator SpawnGargoyles()
    {
        StartCoroutine(GargoyleAttack());
        for (int j = 0; j < 50; j++) {
            int spawnLoc = Random.Range(0, SpawnLocations.Length);
            int targetLoc = Random.Range(0, 2);
            for (int i = 0; i < MaxGargoyles; i++)
            {
                var gargoyle = Instantiate(GargoylePrefab, SpawnLocations[spawnLoc].position, Quaternion.identity);
                gargoyle.GetComponent<Bat>().SetTarget(targetLoc == 0 ? Target1 : Target2);
                gargoyle.GetComponent<Bat>().Direction = -1;
                _gargoyles.Add(gargoyle);
                yield return new WaitForSeconds(.2f);
            }
        }

    }

    IEnumerator GargoyleAttack()
    {
        while (true)
        {
            var eligibleGargoyles = _gargoyles.Where(a => 
                a.GetComponent<Bat>().State == Bat.EnemyState.InPosition &&
                a.activeSelf).ToList();
            if (eligibleGargoyles.Count == 0) yield return new WaitForSeconds(Random.Range(1, 4));
            else
            {
                var gargoyle = eligibleGargoyles[Random.Range(0, eligibleGargoyles.Count)];
                gargoyle.GetComponent<Bat>().Attack();
                yield return new WaitForSeconds(Random.Range(1, 4));
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!_isPlayerSet)
        {
            _gm.GetPlayer().transform.position = PlayerPosition.position;
            _isPlayerSet = true;
        }
    }
}
