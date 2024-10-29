using System;
using System.Collections;
using System.Collections.Generic;
using DynamicPixels.GameService;
using DynamicPixels.GameService.Models.inputs;
using DynamicPixels.GameService.Services.Authentication.Models;
using DynamicPixels.GameService.Services.Table;
using DynamicPixels.GameService.Services.Table.Models;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class TableUserInfo
{
    public int? id;
    public string username;
    public string name;
}

public class TableUserInfoAdd
{
    public string username;
    public string name;
}

public class TableUserScore
{
    public int? id;
    public string username;
    public int score;
}
public class TableUserScoreAdd
{
    public string username;
    public int score;
}

public class JoinedTable
{
    public int? id;
    public string username;
    public int score;
    public string name;
}

public class TableManager : MonoBehaviour
{
    [SerializeField] private GameObject tableContentObject;
    [SerializeField] private GameObject tableSelectionPage;
    [SerializeField] private GameObject tablePage;
    [SerializeField] private TwoColumnSlot twoColumnSlot;
    [SerializeField] private ThreeColumnSlot threeColumnSlot;
    [SerializeField] private TextMeshProUGUI parameter1;
    [SerializeField] private TextMeshProUGUI parameter2;
    public static TableManager Instance;
    private string _selectedTable;


    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        LoginAsGuest();
    }

    public async void LoginAsGuest()
    {
        try
        {
            var result = await ServiceHub.Authentication.LoginAsGuest(new LoginAsGuestParams()
                { name = "Guest-" + Random.Range(0, 100000000) });
            SignInSuccessful(result);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    private void SignInSuccessful(LoginResponse result)
    {
        tableSelectionPage.SetActive(true);
        tablePage.SetActive(false);
    }

    public async void GoToTable(string tableName)
    {
        CleanContent();
        _selectedTable = tableName;
        switch (_selectedTable)
        {
            case "671ff8de052c87010e5998a2":
                var temp1 = await ServiceHub.Table.Find<TableUserScore, FindParams>(new FindParams()
                {
                    tableId = tableName,
                    options = new FindOptions()
                    {
                        Limit = 10,
                    }
                });
        
                foreach (var info in temp1.List)
                {
                    var temp = Instantiate(twoColumnSlot, tableContentObject.transform);
                    temp.Init(info.username, info.score.ToString(), info.id ?? 0);
                }
                break;
            case "671ff3e8052c87010e59941a":
                var temp2 = await ServiceHub.Table.Find<TableUserInfo, FindParams>(new FindParams()
                {
                    tableId = tableName,
                    options = new FindOptions()
                    {
                        Limit = 10,
                    }
                });
        
                foreach (var info in temp2.List)
                {
                    var temp = Instantiate(twoColumnSlot, tableContentObject.transform);
                    temp.Init(info.username, info.name, info.id ?? 0);
                }
                break;
        }
        
    }

    public async void DeleteFromTable(int id)
    {
        await ServiceHub.Table.Delete(new DeleteParams()
        {
            RowIds = new[] { id },
            TableId = _selectedTable
            
        });
        GoToTable(_selectedTable);
    }

    public async void InsertIntoTable()
    {
        switch (_selectedTable)
        {
            case "671ff8de052c87010e5998a2":
                await ServiceHub.Table.Insert<TableUserScoreAdd, InsertParams>(new InsertParams()
                {
                    TableId = _selectedTable,
                    Data = new TableUserScoreAdd()
                    {
                        username = parameter1.text,
                        score = Int32.Parse(parameter2.text.Substring(0, parameter2.text.Length - 1))
                    }
                });
                break;
            case "671ff3e8052c87010e59941a":
                await ServiceHub.Table.Insert<TableUserInfoAdd, InsertParams>(new InsertParams()
                {
                    TableId = _selectedTable,
                    Data = new TableUserInfoAdd()
                    {
                        username = parameter1.text,
                        name = parameter2.text
                    }
                });
                break;
        }
        GoToTable(_selectedTable);
    }
    

    private void CleanContent()
    {
        for (int i = 0; i < tableContentObject.transform.childCount; i++)
            Destroy(tableContentObject.transform.GetChild(i).gameObject);
    }
    
    public async void GoToJoinTable()
    {
        CleanContent();
        var joinParams = new JoinParams()
        {
            TableName = "671ff8de052c87010e5998a2",
            foreignField = "username",
            localField = "username"
        };
        var result = await ServiceHub.Table.Find<JoinedTable, FindParams>(new FindParams()
        {
            tableId = "671ff3e8052c87010e59941a",
            options = new FindOptions()
            {
                Limit = 10,
                Joins = new List<JoinParams>(){joinParams}
            }
        });
        foreach (var info in result.List)
        {
            var temp = Instantiate(threeColumnSlot, tableContentObject.transform);
            temp.Init(info.username, info.name, info.score.ToString());
        }
    } 
}
