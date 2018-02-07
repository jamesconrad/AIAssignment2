using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using System.IO;

public class ProbLoader : MonoBehaviour {

    public UnityEngine.UI.Text banner;

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
                        split[i].Equals('Y') ? true : false,
                        split[i+1].Equals('Y') ? true : false,
                        split[i+2].Equals('Y') ? true : false,
                        val
                        ));
                }

                //reinit doors
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
