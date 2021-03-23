using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMaterial : MonoBehaviour
{
    [SerializeField] MeshRenderer materialTarget = null;
    [SerializeField] string path;

    //store static reference to often-used materials, and organize them by path
    static Dictionary<string, List<Material>> materialDictionary = new Dictionary<string, List<Material>>();

    private void Awake()
    {
        //only load if there is a path to search
        if(string.IsNullOrEmpty(path))
        {
            Destroy(this);
        }
        else if(materialDictionary.Count <= 0 || !materialDictionary.ContainsKey(path))
        {
            materialDictionary.Add(path, Resources.LoadAll<Material>(path).ConvertToList());
        }
    }

    void Start()
    {
        //assign random material 
        if(materialDictionary.ContainsKey(path))
        {
            int index = Random.Range(0, materialDictionary[path].Count);
            Material m = materialDictionary[path][index];
            if(m!= null)
            {
                materialTarget.material = m;
            }
        }

        //remove script
        Destroy(this);
    }
}
