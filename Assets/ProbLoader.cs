using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using System.IO;

public class ProbLoader : MonoBehaviour {

    public UnityEngine.UI.Text banner;
    public GameObject doorPrefab;
    List<Probability> probabilities;

    //basic struct holding each line in the file
    public struct Probability
    {
        public Probability(bool h, bool n, bool s, float p) { hot = h; noisy = n; safe = s; pct = p; }
        public bool hot;
        public bool noisy;
        public bool safe;
        public float pct;
    }

    
    //This is where the probability calculations occur
	void Update () {
		if (Input.GetKeyDown(KeyCode.O))
        {
            //get file and contents
            string path = EditorUtility.OpenFilePanel("Open Probabilities File", "", "txt");
            string file = ReadFile(path);
            if (file.Length > 0)
            {
                banner.text = file;//display file contents

                //clean it up and make it useable
                string[] split = file.Split(new char[] { '\n', '\t', ' ' }, System.StringSplitOptions.RemoveEmptyEntries);

                //reset the list
                probabilities = new List<Probability>();


                //formatting is 4 values per line, first line is irrelevant for values
                for (int i = 4; i < split.Length; i+=4)
                {
                    //grab the percent value
                    float val;
                    float.TryParse(split[i + 3], out val);

                    //create and assign new probability
                    //at this point i = line number, offset to that value will get the "column"
                    probabilities.Add(new Probability(
                        split[i].Equals("Y") ? true : false,//ternary comparison to convert Y/N to true/false
                        split[i+1].Equals("Y") ? true : false,
                        split[i+2].Equals("Y") ? true : false,
                        val
                        ));
                }
                
                //destroy all doors from last file
                for (int i = 0; i < transform.childCount; i++)
                    Destroy(transform.GetChild(i).gameObject);

                for (int i = 0; i < 20; i++)
                {
                    //spawn a new door, with all 3 effects active on it, in a circle facing the origin
                    float angle = i * Mathf.PI * 2f / 20;
                    Vector3 pos = new Vector3(Mathf.Cos(angle) * 8.5f, 1, Mathf.Sin(angle) * 8.5f);
                    Quaternion rot = new Quaternion();
                    rot.SetLookRotation(pos - new Vector3(0, 1, 0));
                    Door newDoor = Instantiate(doorPrefab, pos, rot, transform).GetComponentInChildren<Door>();

                    //generate a random value, unity default has Random.value inclusivley between 0 and 1
                    float ranval = Random.value;

                    //for each line in the file
                    for (int j = 0; j < probabilities.Count; j++)
                    {
                        //subtract its probability from our value
                        ranval -= probabilities[j].pct;

                        //if below 0
                        if (ranval <= 0)
                        {
                            //destroy the effects on the door
                            if (!probabilities[j].hot)
                                Destroy(newDoor.effectHot);
                            if (!probabilities[j].noisy)
                                Destroy(newDoor.effectNoisy);
                            if (!probabilities[j].safe)
                                Destroy(newDoor.effectSafe);
                            break;//exit this line and proceed to next
                        }
                    }
                }
            }
        }
	}

    //open, close file, and return contents
    string ReadFile(string path)
    {
        StreamReader reader = new StreamReader(path);
        string content = reader.ReadToEnd();
        reader.Close();
        return content;
    }
}
