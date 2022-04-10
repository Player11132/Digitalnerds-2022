using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Simulation_Manager : MonoBehaviour
{
    [Header("Other Components")]
    public Calculate_Air_Quality Air_Quality;
    public CityGenerator cityGenerator;
    public UI_Manager UI;
    private int total_consumption = 0;//kwh
    private float Cp = 0;//Pollution Factor (pm 2,5 ug/m3)

    [Header("Power source")]
    private List<Power_Source> power_sources = new List<Power_Source>();
    [SerializeField] private Power_Source Power_Plant;
    //As there are only 3 sources I hard coded them but new ones can be added without much effort
    [SerializeField] private Power_Source Wind_Turbine;
    [SerializeField] private Power_Source Solar_Panel;
    [SerializeField] private Power_Source Hidro_Central;
    [Header("Enviorment")]
    [SerializeField] private float absorption = 0.1f;//pm 25...
    public float Day_duration = 5f;
    [Header("Buildings")]
    [Range(20, 1000)]
    public int nr_Buildings;
    public bool started = false;

    //private Dictionary<Power_Source, List<int>> power_sources_indexes = new Dictionary<Power_Source, List<int>>();

    /*private void Start()
    {
        cityGenerator.CityZoneCount = nr_Buildings;
        cityGenerator.BigSectorCount = nr_Buildings;
        cityGenerator.Generate();
        old_nr_Buildings = nr_Buildings;
        //Setup Dicitonary
        power_sources_indexes.Add(Power_Plant, new List<int>());
        power_sources_indexes.Add(Wind_Turbine, new List<int>());
        power_sources_indexes.Add(Hidro_Central, new List<int>());
        power_sources_indexes.Add(Solar_Panel, new List<int>());
    }
    */


    public void Update()
    {
        //Calculate Consumption
        if (Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        if (started)
        {
            
            //Calculate Emmisions
            Cp = -absorption;
            foreach (CitySample B in cityGenerator.Placed)
            {
                Cp += B.building.Emmisions;
            }
            for (int i = 0; i < power_sources.Count; i++)
            {
                if (power_sources[i].emmits)
                {
                    Cp += power_sources[i].emmisions;
                }
            }
            Air_Quality.Calculate(Cp);
            UI.Total_Emissions = Cp;
            Check_Mouse();
        }
    }

    private void Check_Mouse()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
        {
            //check if its a power source
            if (hit.transform.gameObject.GetComponent<Power_Source_Id>() != null)
            {
                Power_Source_Id power_Source_Id = hit.transform.gameObject.GetComponent<Power_Source_Id>();
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    //Change to Wind Tutbine
                    Vector3 pos = power_Source_Id.transform.position;
                    power_sources[power_Source_Id.id] = Wind_Turbine;
                    int id = power_Source_Id.id;
                    Destroy(power_Source_Id.transform.gameObject);
                    GameObject gameobject = Instantiate(Wind_Turbine.Model_Prefab);
                    gameObject.transform.position = pos;
                    Power_Source_Id ident = gameObject.AddComponent<Power_Source_Id>();
                    ident.id = id;
                    ident.power_source = Wind_Turbine;
                }
                else if(Input.GetKeyDown(KeyCode.Alpha2))
                {
                    //Change to Solar array
                    Vector3 pos = power_Source_Id.transform.position;
                    power_sources[power_Source_Id.id] = Solar_Panel;
                    int id = power_Source_Id.id;
                    Destroy(power_Source_Id.transform.gameObject);
                    GameObject gameObject = Instantiate(Solar_Panel.Model_Prefab);
                    gameObject.transform.position = pos;
                    Power_Source_Id ident = gameObject.AddComponent<Power_Source_Id>();
                    ident.id = id;
                    ident.power_source = Solar_Panel;
                }
                else if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    //Change to Hidropower plant
                    Vector3 pos = power_Source_Id.transform.position;
                    power_sources[power_Source_Id.id] = Hidro_Central;
                    int id = power_Source_Id.id;
                    Destroy(power_Source_Id.transform.gameObject);
                    GameObject gameObject = Instantiate(Hidro_Central.Model_Prefab);
                    gameObject.transform.position = pos;
                    Power_Source_Id ident = gameObject.AddComponent<Power_Source_Id>();
                    ident.id = id;
                    ident.power_source = Hidro_Central;
                }
                else if (Input.GetKeyDown(KeyCode.Alpha4))
                {
                    //Change to Powerplant
                    Vector3 pos = power_Source_Id.transform.position;
                    power_sources[power_Source_Id.id] = Power_Plant;
                    int id = power_Source_Id.id;
                    Destroy(power_Source_Id.transform.gameObject);
                    GameObject gameObject = Instantiate(Power_Plant.Model_Prefab);
                    gameObject.transform.position = pos;
                    Power_Source_Id ident = gameObject.AddComponent<Power_Source_Id>();
                    ident.id = id;
                    ident.power_source = Power_Plant;
                }
            }
        }
    }


    //Manage Power Sources
    public void Manage_Power_Sources()
    {
        //calculate consumption
        total_consumption = 0;
        foreach (CitySample B in cityGenerator.Placed)
        {
            total_consumption += B.building.Consumption;
        }
        //Calculte amount of Power sources needed
        //See if sources already exist 
        if (power_sources.Count == 0)
        {
            //By default add power plants
            int powerplants_needed = total_consumption / Power_Plant.production_power;
            for (int i = 0; i < powerplants_needed; i++)
            {
                power_sources.Add(Power_Plant);
                Spawn_Power_Source(i, Power_Plant);
            }
        }
        else
        {
            int powerplants_needed = total_consumption;
            for (int i = 0; i < power_sources.Count; i++)
            {
                powerplants_needed -= power_sources[i].production_power;
            }
            powerplants_needed = powerplants_needed / Power_Plant.production_power;

            for (int i = 0; i < powerplants_needed; i++)
            {
                power_sources.Add(Power_Plant);
                Spawn_Power_Source(i, Power_Plant);
            }
        }
    }

    private void Spawn_Power_Source(int id,Power_Source source)
    {
        print("Spawning");
        GameObject Power_Source = Instantiate(source.Model_Prefab);
        Power_Source.transform.position = source.spawn_points[id];
        Power_Source_Id P_ID=Power_Source.AddComponent<Power_Source_Id>();
        P_ID.id = id;
        P_ID.power_source = source;
    }

    public void Replace_Power_Source(Power_Source replace,Power_Source power_Source)
    {
        for(int i=0;i<power_sources.Count;i++)
        {
            if(power_sources[i].id==replace.id)
            {
                power_sources[i] = power_Source;
                Debug.Log("Replaced power source");
                return;
            }
        }
    }
}
