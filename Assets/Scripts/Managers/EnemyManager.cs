using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public List<GameObject> enemies;
    public GameObject currentEnemy;
    public Transform cam;
    public LineRenderer lr;
    public GameObject[] warnings;

    private float nextTick;
    private float tickRate = 0.05f;

    Dictionary<GameObject, LineRenderer> lines = new Dictionary<GameObject, LineRenderer>();
    List<float> expandTime = new List<float>();
    private int warningIndex;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!currentEnemy)
        {
            //endCombat?
        }
        else if(Time.time > nextTick)
        {
            ShowWarnings();
            nextTick = Time.time + tickRate;
        }
    }

    public void NewEnemy()
    {
        currentEnemy = Instantiate(enemies[Random.Range(0, enemies.Count)], cam);
    }

    public void ShowWarnings()
    {
        GameObject[] warnings = GameObject.FindGameObjectsWithTag("Indicator");
        if (warnings.Length <= 0)
            return;

        warningIndex = 0;

        for(int x = 0; x < warnings.Length; x++)
        {
            if(warnings[x].activeSelf)
            {
                if(!lines.ContainsKey(warnings[x]))
                {
                    lines.Add(warnings[x], Instantiate(lr, warnings[x].transform));
                    if(warningIndex >= expandTime.Count)
                        expandTime.Add(0);
                    else
                        expandTime[warningIndex] = 0;
                }

                Collider2D col = warnings[x].GetComponent<Collider2D>();
                if (col.GetType() == typeof(CircleCollider2D))
                {
                    DrawCircle(col as CircleCollider2D);
                }
                else if (col.GetType() == typeof(BoxCollider2D))
                {
                    DrawBox(col as BoxCollider2D);
                }

                warningIndex++;
            }
        }
    }

    public void DrawCircle(CircleCollider2D c)
    {
        LineRenderer line = lines.GetValueOrDefault(c.gameObject);

        if(expandTime[warningIndex] < 1.0f)
        {
            expandTime[warningIndex] += Time.deltaTime * (1 / tickRate) * 2;
        }
        float expand = expandTime[warningIndex];

        int numPoints = 61;
        float r = c.radius;
        float rotAmount = 400 / numPoints;
        GameObject temp = new GameObject();
        temp.transform.position = new Vector3(r * expand, 0, 0);
        Vector3[] positions = new Vector3[numPoints];

        for(int x = 0; x < numPoints; x++)
        {
            positions[x] = c.gameObject.transform.position + temp.transform.position + new Vector3(c.offset.x, c.offset.y, 0);

            temp.transform.position = Vector3.zero;
            temp.transform.Rotate(new Vector3(0,0,1), rotAmount);
            temp.transform.Translate(Vector3.right * (r * Random.Range(0.9f, 1.1f) * expand));
        }

        positions[numPoints - 1] = positions[0];

        line.positionCount = numPoints;
        line.SetPositions(positions);
        Destroy(temp);
    }

    public void DrawBox(BoxCollider2D b)
    {

    }

    public void RemoveWarning(GameObject c)
    {
        LineRenderer temp = lines.GetValueOrDefault(c);
        lines.Remove(c);
        Destroy(temp.gameObject);
    }
}
