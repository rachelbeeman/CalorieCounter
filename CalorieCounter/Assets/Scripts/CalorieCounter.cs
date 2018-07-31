using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;

public class CalorieCounter : MonoBehaviour {

    public static Dictionary<string, int> knownFoods;
    int CaloriesToday;
    
    //UI elements
    public Text CaloriesTodayCounter;
    public Dropdown dropdown;
    public InputField newFoodNameField;
    public InputField newCaloriesField;
    public Button AddNewButton;

    //I/O elements
    private const string fileNameList = "knownFoodsList.txt";
    private const string fileNameCalories = "DailyCalorieCount.txt";
    //.persistantDataPath is used instead of .dataPath for android saving
    private string filePath;

    //adding new food/calorie pairs
    private string NewFoodNameToAdd;
    private int NewCalorieCountToAdd;

    // Use this for initialization
    void Awake () {

        filePath = Application.persistentDataPath + "\\";
        CaloriesTodayCounter.text = "Calories Today: 0";
        knownFoods = new Dictionary<string, int>();

        newFoodNameField.gameObject.SetActive(false);
        newCaloriesField.gameObject.SetActive(false);

        ReadKnownFoodListFromFile();
        ReadDailyCalorieCount();

        DateCheckCalorieCount();
    }

    //This function checks the date stamp of the calorie count. If it is not from today, it resets daily calories to zero.
    private void DateCheckCalorieCount()
    {
        if(File.Exists(filePath + fileNameCalories))
        {
            DateTime dt = File.GetLastAccessTime(filePath + fileNameCalories).Date;
            //If the date the calorie file was last updated was not today
            if (System.DateTime.Now.Date != File.GetLastWriteTime(filePath + fileNameCalories).Date)
            {
                CaloriesToday = 0;
                WriteToDailyCalorieCount();
            }
        }
    }

    //This function is called when the "Add Known Food" button is pressed.
    public void AddKnownFoodButton()
    {
        string foodToAdd = "";
        if (dropdown != null)
        {
            foodToAdd = dropdown.captionText.text;
        }

        if (foodToAdd != null && knownFoods.ContainsKey(foodToAdd))
        {
            CaloriesToday += knownFoods[foodToAdd];
            WriteToDailyCalorieCount();
        }
           
    }

    public void AddNewFoodButton()
    {
        AddNewButton.gameObject.SetActive(false);
        newFoodNameField.gameObject.SetActive(true);
    }

    public void EndNameEdit()
    {
        NewFoodNameToAdd = "";
        NewFoodNameToAdd = newFoodNameField.GetComponentInChildren<Text>().text;

        newFoodNameField.gameObject.SetActive(false);
        newCaloriesField.gameObject.SetActive(true);
    }

    public void EndCalorieEdit()
    {
        NewCalorieCountToAdd = 0;
        string cal = newCaloriesField.GetComponentInChildren<Text>().text;
        Int32.TryParse(cal, out NewCalorieCountToAdd);

        newCaloriesField.gameObject.SetActive(false);
        AddNewButton.gameObject.SetActive(true);

        AddNewFoodCaloriePair(NewFoodNameToAdd, NewCalorieCountToAdd);
        UpdateTable();
        //WriteNewKeyValuePairToFile(NewFoodNameToAdd);
    }

    private void UpdateTable()
    {
        dropdown.GetComponent<FillDropdown>().fillTable();
        
    }

    private void AddNewFoodCaloriePair(string newfood, int newcal)
    {
        if (!knownFoods.ContainsKey(newfood) && newfood != "" && newcal != 0)
        {
            knownFoods.Add(newfood, newcal);
            WriteNewKeyValuePairToFile(newfood);
        }
    }


    //Read into the knownFoods dictionary from a text file, called from Start
    private void ReadKnownFoodListFromFile()
    {
        //If the file exists use it. Otherwise start FirstRun
        if (File.Exists(filePath + fileNameList))
        {
            using (StreamReader sr = new StreamReader(filePath + fileNameList))
            {
                while (!sr.EndOfStream)
                {
                    //Dict entries are stored as [food, calories]
                    string str = sr.ReadLine();
                    str = str.TrimStart('[');
                    str = str.TrimEnd(']');
                    string[] a = new string[2];
                    a = str.Split(',');

                    int x = 0;
                    Int32.TryParse(a[1], out x);

                    knownFoods.Add(a[0], x);
                }
            }
        }
        else
        {
            FirstRun();
        }
    }

    private void ReadDailyCalorieCount()
    {
        if (File.Exists(filePath + fileNameCalories))
        {
            using (StreamReader sr = new StreamReader(filePath + fileNameCalories))
            {
                int x = 0;
                Int32.TryParse(sr.ReadLine(), out x);
                CaloriesToday = x;
            }
        }
    }

    private void WriteToDailyCalorieCount()
    {
        using (StreamWriter sw = new StreamWriter(filePath + fileNameCalories))
        {
            sw.WriteLine(CaloriesToday);
        }
    }

    //This function is called if pre-existing knownFoods file is not found.
    private void FirstRun()
    {
        /*When this app is rewritten for the store, this section will need to initialize a 'firstRun' procedure that asks the user to add foods
         * to their list.
         * Right now, for my personal version of the app, I want it to fill out my list with foods I have already chosen.
         */
        knownFoods = new Dictionary<string, int>() { 
            { "egg", 78 },
            {"baked apple", 165},
            {"iced coffee with milk", 35},
            {"cappuccino", 90},
            {"bacon gouda sandwich", 350},
            {"1 medium strawberry", 4},
            {"3 sweet mini peppers", 25},
            {"1 / 2 cup sugar snap peas", 30},
            {"1 / 2 cup broccoli", 10},
            {"12 baby carrots", 35},
            {"oikos protein yogurt", 120},
            {"string cheese", 80},
         };

        foreach (KeyValuePair<string, int> entry in knownFoods)
        {
            WriteNewKeyValuePairToFile(entry.Key);
        }
    }

    //**Rewrite to only add the newest food
    private void WriteNewKeyValuePairToFile(string newfood)
    {
        using (StreamWriter sw = File.AppendText(filePath + fileNameList))
        {
            sw.WriteLine(new KeyValuePair<string, int>(newfood, knownFoods[newfood]));
        }
    }

    void Update()
    {
        CaloriesTodayCounter.text = "Calories Today: " + CaloriesToday.ToString();
    }
}
