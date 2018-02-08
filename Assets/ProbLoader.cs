using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using System.IO;

public class ProbLoader : MonoBehaviour {

    public UnityEngine.UI.Text banner;
    public GameObject doorPrefab;

    public struct Probability
    {
        public Probability(bool h, bool n, bool s, float p) { hot = h; noisy = n; safe = s; pct = p; }
        public bool hot;
        public bool noisy;
        public bool safe;
        public float pct;
    }

    List<Probability> Probabilities;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.O))
        {
            string path = EditorUtility.OpenFilePanel("Open Probabilities File", "", "txt");
            string file = ReadFile(path);
            if (file.Length > 0)
            {
                //print(file);
                banner.text = file;
                //clean it up and make it useable

                string[] split = file.Split(new char[] { '\n', '\t', ' ' }, System.StringSplitOptions.RemoveEmptyEntries);

                Probabilities = new List<Probability>();

                for (int i = 4; i < split.Length; i+=4)
                {
                    //print(split[i] + split[i + 1] + split[i + 2] + split[i + 3]);
                    float val;
                    float.TryParse(split[i + 3], out val);

                    Probabilities.Add(new Probability(//create and assign new probability
                        split[i].Equals("Y") ? true : false,
                        split[i+1].Equals("Y") ? true : false,
                        split[i+2].Equals("Y") ? true : false,
                        val
                        ));
                }
                
                for (int i = 0; i < transform.childCount; i++)
                {
                    Destroy(transform.GetChild(i).gameObject);
                }

                for (int i = 0; i < 20; i++)
                {
                    float angle = i * Mathf.PI * 2f / 20;
                    Vector3 pos = new Vector3(Mathf.Cos(angle) * 8.5f, 1, Mathf.Sin(angle) * 8.5f);
                    Quaternion rot = new Quaternion();
                    rot.SetLookRotation(pos - new Vector3(0, 1, 0));
                    Door newDoor = Instantiate(doorPrefab, pos, rot, transform).GetComponentInChildren<Door>();

                    float ranval = Random.value;
                    for (int j = 0; j < Probabilities.Count; j++)
                    {
                        ranval -= Probabilities[j].pct;
                        if (ranval <= 0)
                        {
                            if (!Probabilities[j].hot)
                                Destroy(newDoor.effectHot);
                            if (!Probabilities[j].noisy)
                                Destroy(newDoor.effectNoisy);
                            if (!Probabilities[j].safe)
                                Destroy(newDoor.effectSafe);
                            break;
                        }
                    }
                }
            }
        }
	}

    string ReadFile(string path)
    {
        StreamReader reader = new StreamReader(path);
        string content = reader.ReadToEnd();
        reader.Close();
        return content;
    }
}
