using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Globalization;

public class UI_Manager : MonoBehaviour
{
    [Header("UI Elements Air quality")]
    public RectTransform Air_Quality_Info_Menu;
    public Text _AQI;
    public Text _Health_Implications;
    public Text _Cautionary_Statement;
    public Text _Category;
    public Text _Total_Emissions;
    public InputField Input_Concentration;
    public float Total_Emissions;
    public bool enable_Input = false;
    [Header("UI Elements Building Info")]
    public RectTransform Building_Info_Menu;
    public Text Building_Name;
    public Text Power_Consumption;
    public Text Emmissions;
    [Header("UI Elements Power source info")]
    public Text Pro_cons_Content;
    public RectTransform Pro_Cons_Scroll_menu;
    [Header("Menu")]
    public Slider number_apartments;
    public Text nr_apt;
    public RectTransform menu;
    [Header("Other")]
    public Calculate_Air_Quality Calculate_Air_Quality;
    public Simulation_Manager mg;
    public CityGenerator cityGenerator;

    private float menu_x_cord_air;
    private float menu_x_cord_build;
    private void Start()
    {
        menu_x_cord_air = Air_Quality_Info_Menu.position.x;
        menu_x_cord_build = Building_Info_Menu.position.x;
    }
    // Update is called once per frame
    void Update()
    {
        if (enable_Input && Input_Concentration.text != null)
        {
            Calculate_Air_Quality.Calculate(float.Parse(Input_Concentration.text, CultureInfo.InvariantCulture.NumberFormat));
        }
        Update_Air_UI();
        Check_Mouse();
        if (nr_apt != null)
        nr_apt.text = number_apartments.value.ToString();
    }

    public void start_sim()
    {
        mg.nr_Buildings = Mathf.RoundToInt(number_apartments.value);
        mg.started = true;
        menu.position = new Vector3(1000, 1000, 1000);
        print("Started sim");
        cityGenerator.CityZoneCount = Mathf.RoundToInt(number_apartments.value);
        cityGenerator.BigSectorCount = Mathf.RoundToInt(number_apartments.value);
        cityGenerator.Generate();
        mg.Manage_Power_Sources();
    }

    private void Check_Mouse()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),out hit)&&Input.GetMouseButtonDown(0))
        {
            Debug.Log(hit.collider.name);
            //check if its building
            if (hit.transform.gameObject.GetComponent<CitySample>()!=null)
            {
                CitySample building=hit.transform.gameObject.GetComponent<CitySample>();
                //if it is tell info about the building
                Building_Name.text = "Building Name:" + building.building.Building_Name;
                Power_Consumption.text = "Power Consumption:" + building.building.Consumption + "kwH";
                return;
            }
            else
            {
                Building_Name.text = "Hover over a structure to see its information";
                Power_Consumption.text = "-";
                
            }
            //check if its a power source
            if(hit.transform.gameObject.GetComponent<Power_Source_Id>()!=null)
            {
                Power_Source source = hit.transform.gameObject.GetComponent<Power_Source_Id>().power_source;
                Building_Name.text = source.name;

                string Emmisions="";
                if (source.emmits)
                    Emmisions = "Emmisions:" + source.emmisions + "p.m 25";
                else
                    Emmisions = "No emisions";
                Emmissions.text = Emmisions;

                Power_Consumption.text = source.production_power + "kwH";
                if (Pro_Cons_Scroll_menu.gameObject.active == false)
                    Pro_Cons_Scroll_menu.gameObject.active = true;
                if (source.pros.Length != 0)
                {
                    Pro_cons_Content.text = "Pros: \n\n";
                    for (int i = 0; i < source.pros.Length; i++)
                    {
                        Pro_cons_Content.text += "-" + source.pros[i] + "\n";
                    }
                }
                else
                    Pro_cons_Content.text = "Pros: \n\n -None \n";
                if (source.cons.Length != 0)
                {
                    Pro_cons_Content.text += "\n Cons: \n\n";
                    for (int i = 0; i < source.cons.Length; i++)
                    {
                        Pro_cons_Content.text += "-" + source.cons[i] + "\n";
                    }
                }
                else
                    Pro_cons_Content.text += "Cons: \n\n -None\n";
                return;
            }
            else
            {
               if(Pro_cons_Content.gameObject.GetComponent<Text>()!=null)
               {
                    Destroy(Pro_cons_Content.gameObject.GetComponent<Text>());
               }
                Power_Consumption.text = "-";
                Building_Name.text = "Hover over a structure to see its info";
                Emmissions.text = "-";
                Pro_Cons_Scroll_menu.gameObject.active = false;
            }
        }
    }

    bool retracted1,retracted = false;
    public void Retract_Air_Menu(int offset)
    {
        if(retracted)
            Air_Quality_Info_Menu.position = new Vector3(offset, Air_Quality_Info_Menu.position.y, Air_Quality_Info_Menu.position.z);
        else
            Air_Quality_Info_Menu.position = new Vector3(menu_x_cord_air, Air_Quality_Info_Menu.position.y, Air_Quality_Info_Menu.position.z);
        retracted = !retracted;
    }

    public void Retract_Building_Menu(int offset)
    {
        if (retracted1)
            Building_Info_Menu.position = new Vector3(offset, Building_Info_Menu.position.y, Building_Info_Menu.position.z);
        else
            Building_Info_Menu.position = new Vector3(menu_x_cord_build, Building_Info_Menu.position.y, Building_Info_Menu.position.z);
        retracted1 = !retracted1;
    }

    private void Update_Air_UI()
    {
        if (Calculate_Air_Quality.AQI < 800)
            _AQI.text = "AQI: " + Calculate_Air_Quality.AQI;
        else
            _AQI.text = "AQI: 800+";
        _Health_Implications.text = "Health Implications:" + Calculate_Air_Quality.Health_Implications[Calculate_Air_Quality.index];
        _Cautionary_Statement.text = "Cautionary Statement:" + Calculate_Air_Quality.Cautionary_Statement[Calculate_Air_Quality.index];
        _Category.text = "AQI Category:" + Calculate_Air_Quality.Categorys[Calculate_Air_Quality.index];
        _Total_Emissions.text = Total_Emissions.ToString();
    }
}
