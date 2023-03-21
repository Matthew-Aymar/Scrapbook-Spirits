using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomParticles : MonoBehaviour
{
    public float rate;
    public float variance;
    public float speed;
    public float lifespan;
    public float gravity;

    public GameObject[] particles;
    private List<GameObject> activeParticles = new List<GameObject>();
    private List<float> activeLifespan = new List<float>();
    private List<Vector3> activeDirections = new List<Vector3>();

    private float nextSpawn;
    // Start is called before the first frame update
    void Start()
    {
        nextSpawn = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time > nextSpawn)
        {
            int r = Random.Range(0, particles.Length);
            GameObject newPart = Instantiate(particles[r], transform);
            activeParticles.Add(newPart);
            activeLifespan.Add(lifespan);

            newPart.transform.Rotate(new Vector3(0, 0, Random.Range(0, 360)));
            activeDirections.Add(newPart.transform.right);

            newPart.transform.parent = null;

            if (rate - variance <= 0)
                variance = 0;

            nextSpawn = Time.time + Random.Range(1 / (rate - variance), 1 / (rate + variance));
        }

        if(activeParticles.Count > 0)
        {
            if (activeLifespan[0] <= 0)
            {
                GameObject endPart = activeParticles[0];
                Destroy(endPart);

                activeParticles.RemoveAt(0);
                activeLifespan.RemoveAt(0);
                activeDirections.RemoveAt(0);
            }

            for (int x = 0; x < activeParticles.Count; x++)
            {
                activeParticles[x].transform.Translate(activeDirections[x] * speed * Time.deltaTime);
                activeParticles[x].transform.Translate(Vector3.down * gravity * Time.deltaTime);

                activeLifespan[x] -= Time.deltaTime;
            }
        }
    }

    private void OnDestroy()
    {
        for (int x = 0; x < activeParticles.Count; x++)
        {
            GameObject endPart = activeParticles[x];
            Destroy(endPart);
        }
        activeParticles.Clear();
    }
}
