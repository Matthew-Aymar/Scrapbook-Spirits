using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public List<GameObject> enemies;
    public GameObject currentEnemy;
    public DamageNumbers numbers;
    public Transform cam;
    public LineRenderer lr;
    public GameObject[] warnings;

    public GameObject poof;

    private float nextTick;
    private float tickRate = 0.05f;

    Dictionary<GameObject, LineRenderer> lines = new Dictionary<GameObject, LineRenderer>();
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
        GameObject p = Instantiate(poof, currentEnemy.transform);
        p.transform.Rotate(new Vector3(0, 0, 1), Random.Range(0, 360));
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

        int numPoints = 61;
        float r = c.radius;
        float rotAmount = 400 / numPoints;
        GameObject temp = new GameObject();
        temp.transform.position = new Vector3(r, 0, 0);
        Vector3[] positions = new Vector3[numPoints];

        for(int x = 0; x < numPoints; x++)
        {
            positions[x] = c.gameObject.transform.position + temp.transform.position + new Vector3(c.offset.x, c.offset.y, 0);

            temp.transform.position = Vector3.zero;
            temp.transform.Rotate(new Vector3(0,0,1), rotAmount);
            temp.transform.Translate(Vector3.right * (r * Random.Range(0.9f, 1.1f)));
        }

        positions[numPoints - 1] = positions[0];

        line.positionCount = numPoints;
        line.SetPositions(positions);
        Destroy(temp);
    }

    public void DrawBox(BoxCollider2D b)
    {
        LineRenderer line = lines.GetValueOrDefault(b.gameObject);

        int size = (int)(b.size.y * 8 + b.size.x * 8 + 1);
        Vector3[] linePositions = new Vector3[size];

        float segmentCount = 0;
        int lastSegment = 0;
        for(int x = 0; x < size - 1; x++)
        {
            float r = Random.Range(-0.1f, 0.1f);

            if(x < b.size.x * 4)
            {
                if (lastSegment != 0)
                    segmentCount = 0;
                linePositions[x] = new Vector3(b.bounds.min.x + segmentCount, b.bounds.min.y + r);
                lastSegment = 0;
            }
            else if(x < b.size.x * 4 + b.size.y * 4)
            {
                if (lastSegment != 1)
                    segmentCount = 0;
                linePositions[x] = new Vector3(b.bounds.max.x + r, b.bounds.min.y + segmentCount);
                lastSegment = 1;
            }
            else if(x < b.size.x * 8 + b.size.y * 4)
            {
                if (lastSegment != 2)
                    segmentCount = 0;
                linePositions[x] = new Vector3(b.bounds.max.x - segmentCount, b.bounds.max.y + r);
                lastSegment = 2;
            }
            else
            {
                if (lastSegment != 3)
                    segmentCount = 0;
                linePositions[x] = new Vector3(b.bounds.min.x + r, b.bounds.max.y - segmentCount);
                lastSegment = 3;
            }

            segmentCount += 0.25f;
        }

        linePositions[size - 1] = linePositions[0];

        line.positionCount = size;
        line.SetPositions(linePositions);
    }

    public void RemoveWarning(GameObject c)
    {
        LineRenderer temp = lines.GetValueOrDefault(c);
        lines.Remove(c);
        Destroy(temp.gameObject);
    }

    public void StunEnemy(float d)
    {
        currentEnemy.GetComponent<Enemy>().Stun();
        currentEnemy.GetComponent<Enemy>().stunEnd = Time.time + d;
    }

    public void Damage(float d)
    {
        currentEnemy.GetComponent<Enemy>().health -= d;
        numbers.MakeNumber(currentEnemy, d);
    }
}
