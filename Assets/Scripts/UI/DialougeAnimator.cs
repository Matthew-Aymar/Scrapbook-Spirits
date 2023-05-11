using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WaveValues
{
    public WaveValues(int letters)
    {
        letterCount = letters;
        letterHeights = new List<float>(letters);
        ascending = new bool[letters];
    }

    public int letterCount;
    public List<float> letterHeights;   //float values for each letter 
    public bool[] ascending;           //direction of each letter in sequence
}

public class ShakeValues
{
    public ShakeValues(int letters)
    {
        letterCount = letters;
        letterSize = new List<float>(letters);
        letterDuration = new List<float>(letters);
    }

    public int letterCount;
    public List<float> letterSize;
    public List<float> letterDuration;
}

public class DialougeAnimator : MonoBehaviour
{
    public TMP_Text target;
    public string originalText;
    private string parsedText;
    private string currentText = "";
    private string currentTagged = "";
    //Letter by letter values
    private int currentLetter;
    public float updateRate;
    private float lastUpdate;

    //Wave Values
    public List<int> waveIndexes; // must be in pairs
    public List<WaveValues> waves;
    //Shake Values
    private List<int> shakeIndexes; // must be in pairs
    public List<ShakeValues> shakes;
    
    // Start is called before the first frame update
    void Start()
    {
        waveIndexes = new List<int>();
        shakeIndexes = new List<int>();
        parsedText = originalText;
        bool wClosed = true, sClosed = true;
        while (parsedText.IndexOf("|") != -1)
        {
            int nextBreak = parsedText.IndexOf("|");

            if (parsedText[nextBreak + 1].Equals('w'))
            {
                waveIndexes.Add(nextBreak + 1);
                wClosed = !wClosed;
            }
            else if (parsedText[nextBreak + 1].Equals('s'))
            {
                shakeIndexes.Add(nextBreak + 1);
                sClosed = !sClosed;
            }
            parsedText = parsedText.Substring(0, nextBreak) + parsedText.Substring(nextBreak + 2);
        }

        if(!wClosed)
        {
            waveIndexes.Add(parsedText.Length);
        }
        if(!sClosed)
        {
            shakeIndexes.Add(parsedText.Length);
        }

        lastUpdate = Time.time;
        MakeWaveHeights();
        MakeShakeVals();
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time > lastUpdate + updateRate)
        {
            LetterScroll();
            LetterWave();
            ShakeUpdate();

            AddTags();
            target.text = currentTagged;

            lastUpdate = Time.time;
        }
    }

    public void LetterScroll()
    {
        if(currentLetter < parsedText.Length)
        {
            currentText += parsedText[currentLetter];
            currentLetter++;
        }
    }

    public void AddTags()
    {
        currentTagged = currentText;

        int currentShake = -1;
        int shakeCount = 0;
        bool inShake = false;

        int currentWave = -1;
        int waveCount = 0;
        bool inWave = false;

        for(int x = currentText.Length; x >= 0; x--)
        {
            if(!inWave)
            {
                for (int w = 0; w < waveIndexes.Count - 1; w += 2)
                {
                    if (waveIndexes[w] < x && x < waveIndexes[w + 1])
                    {
                        inWave = true;
                        currentWave = w / 2;
                        if((waveIndexes[w] + waves[currentWave].letterHeights.Count - 1) > x)
                        {
                            waveCount = x - waveIndexes[w];
                        }
                        else
                            waveCount = waves[currentWave].letterHeights.Count - 1;

                        break;
                    }
                }
            }

            if (!inShake)
            {
                for (int s = 0; s < shakeIndexes.Count - 1; s += 2)
                {
                    if (shakeIndexes[s] < x && x < shakeIndexes[s + 1])
                    {
                        inShake = true;
                        currentShake = s / 2;
                        if ((shakeIndexes[s] + shakes[currentShake].letterSize.Count - 1) > x)
                        {
                            shakeCount = x - shakeIndexes[s];
                        }
                        else
                            shakeCount = shakes[currentShake].letterSize.Count - 1;

                        break;
                    }
                }
            }

            if (inWave)
            {
                currentTagged = currentTagged.Substring(0, x - 1) + "<voffset=" + waves[currentWave].letterHeights[waveCount] + "em>" + currentTagged[x - 1] + "</voffset>" + currentTagged.Substring(x);
                waveCount--;

                if (waveCount < 0)
                    inWave = false;
            }

            if(inShake)
            {
                currentTagged = currentTagged.Substring(0, x - 1) + "<size=" + shakes[currentShake].letterSize[shakeCount] + "em>" +
                                                                    currentTagged[x - 1] + "</size>" + currentTagged.Substring(x);

                shakeCount--;

                if (shakeCount < 0)
                    inShake = false;
            }
        }
    }

    public void MakeShakeVals()
    {
        shakes = new List<ShakeValues>();

        int letterCount;
        for (int x = 0; x < shakeIndexes.Count - 1; x += 2)
        {
            letterCount = shakeIndexes[x + 1] - shakeIndexes[x];
            shakes.Add(new ShakeValues(letterCount));

            for (int y = 0; y < shakes.Count; y++)
            {
                for (int z = 0; z < shakes[y].letterCount; z++)
                {
                    shakes[y].letterSize.Add(1.0f);
                    shakes[y].letterDuration.Add(0 - 0.1f * (shakeIndexes[x] + z));
                }
            }
        }
    }

    public void MakeWaveHeights()
    {
        waves = new List<WaveValues>();

        int letterCount;
        for(int x = 0; x < waveIndexes.Count - 1; x += 2)
        {
            letterCount = waveIndexes[x + 1] - waveIndexes[x];
            waves.Add(new WaveValues(letterCount));
        }

        for(int x = 0; x < waves.Count; x++)
        {
            for (int y = 0; y < waves[x].letterCount; y++)
            {
                float offsetHeight = ((float)(y + 1) / waves[x].letterCount) * 0.5f;

                waves[x].letterHeights.Add(Mathf.Round(offsetHeight * 10) * 0.1f);
            }
        }
    }

    public void LetterWave()
    {
        for (int x = 0; x < waves.Count; x++)
        {
            for (int y = 0; y < waves[x].letterHeights.Count; y++)
            {
                if (waves[x].ascending[y] == false)
                    waves[x].letterHeights[y] -= 0.1f;
                else if (waves[x].ascending[y] == true)
                    waves[x].letterHeights[y] += 0.1f;

                if (waves[x].ascending[y] == false && waves[x].letterHeights[y] < 0.0f)
                {
                    waves[x].ascending[y] = true;
                    waves[x].letterHeights[y] = 0.0f;
                }
                else if (waves[x].ascending[y] == true && waves[x].letterHeights[y] > 0.5f)
                {
                    waves[x].ascending[y] = false;
                    waves[x].letterHeights[y] = 0.5f;
                }

                waves[x].letterHeights[y] = Mathf.Round(waves[x].letterHeights[y] * 10) * 0.1f;
            }
        }
    }

    public void ShakeUpdate()
    {
        for(int x = 0; x < shakes.Count; x++)
        {
            for(int y = 0; y < shakes[x].letterSize.Count; y++)
            {
                if(shakes[x].letterDuration[y] <= 0.5f)
                {
                    Debug.Log(shakes[x].letterSize[y] + " : " + shakes[x].letterDuration[y]);
                    shakes[x].letterSize[y] = 1.5f - (shakes[x].letterDuration[y]);
                    shakes[x].letterDuration[y] += 0.1f;
                }
                else
                {
                    shakes[x].letterSize[y] = 1.0f;
                }
            }
        }
    }
}
