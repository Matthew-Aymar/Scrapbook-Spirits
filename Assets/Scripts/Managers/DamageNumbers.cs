using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageNumbers : MonoBehaviour
{
    public List<TMP_Text> activeNums;
    public List<Vector2> activeDirs;
    public GameObject numberParticle;
    public float speed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        int toRemove = -1;
        int count = 0;
        foreach(TMP_Text t in activeNums)
        {
            t.gameObject.transform.Translate(activeDirs[count] * Time.deltaTime * speed);
            Color nextColor = Vector4.MoveTowards(t.color, new Vector4(1, 1, 1, 0), Time.deltaTime * speed * 0.75f);
            t.color = nextColor;

            if(t.color.a <= 0)
            {
                toRemove = count;
            }

            count++;
        }

        if(toRemove != -1)
        {
            GameObject temp = activeNums[toRemove].gameObject;
            activeNums.RemoveAt(toRemove);
            activeDirs.RemoveAt(toRemove);

            Destroy(temp);
        }
    }

    public void MakeNumber(GameObject target, float num)
    {
        GameObject temp = Instantiate(numberParticle, target.transform.Find("Canvas"));
        activeDirs.Add(new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)).normalized);
        activeNums.Add(temp.GetComponent<TMP_Text>());
        activeNums[activeNums.Count - 1].text = "" + num;
    }
}
