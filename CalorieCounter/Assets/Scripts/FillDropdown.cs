using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FillDropdown : MonoBehaviour {

    Dropdown dropdown;

    // Use this for initialization
    void Start () {

        dropdown = GetComponent<Dropdown>();
        fillTable();
    }

    public void fillTable()
    {      
        dropdown.ClearOptions();
        dropdown.AddOptions(new List<string>(CalorieCounter.knownFoods.Keys));
    }

   
	
}
