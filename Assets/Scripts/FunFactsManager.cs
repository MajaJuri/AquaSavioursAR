using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using TMPro;

public class FunFactsManager : MonoBehaviour
{
    public string fileName;

    [SerializeField]
    private TextMeshProUGUI funFactTextField;

    public void DisplayFunFact()
    {
        string funFact = GetRandomAnimalFact(StaticScript.ARAnimal);
        GetComponentInParent<SetupARScene>().WarningMessage(funFact, -1f); // fun fact ce se maknuti tek kad kliknemo na taj text box
    }

    string GetRandomAnimalFact(string animalName)
    {
        List<string> animalFunFacts = GetAnimalFactsFor(animalName);
        int randomIndex = Random.Range(0, animalFunFacts.Count);
        Debug.Log("Random index = " + randomIndex);
        return animalFunFacts.ElementAt(randomIndex);
    }

    List<string> GetAnimalFactsFor(string animalName)
    {
        AnimalFacts animalFactsInJson = LoadJsonFile(fileName);
        if (animalFactsInJson != null)
        {

            List<string> animalFunFacts = new List<string>();
            foreach (AnimalFact animalFact in animalFactsInJson.animalFacts)
            {
                if (animalFact.animalName.Equals(animalName))
                {
                    foreach (string fact in animalFact.funFacts)
                    {
                        animalFunFacts.Add(fact);
                    }
                }
            }
            return animalFunFacts;
        }
        else
        {
            Debug.Log("AnimalFactsInJson is null.");
            return null;
        }
    }

    AnimalFacts LoadJsonFile(string fileName)
    {
        AnimalFacts animalFactsInJson;

        TextAsset jsonFile = Resources.Load<TextAsset>(fileName);

        if (jsonFile != null)
        {
            string jsonString = jsonFile.text;
            animalFactsInJson = JsonUtility.FromJson<AnimalFacts>(jsonString);
            foreach (AnimalFact animalFact in animalFactsInJson.animalFacts)
            {
                Debug.Log($"Animal: {animalFact.animalName}");
                foreach (string funFact in animalFact.funFacts)
                {
                    Debug.Log($"Fun Fact: {funFact}");
                }
            }
            return animalFactsInJson;
        }
        else
        {
            Debug.LogError($"File not found: {fileName}");
            return null;
        }
    }
 
 }
