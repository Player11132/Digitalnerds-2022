using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Globalization;

public class Calculate_Air_Quality : MonoBehaviour
{
    [Header("Settings")]
    public bool debug=false;


   public int AQI;
    //Calculation elements
    public int index;
    //pollutant high and low breakpoints
    //values from https://aqicn.org/faq/2013-09-09/revised-pm25-aqi-breakpoints/
    private float[] pol_hi_breakpoints = new float[] {12.0f,35.4f,55.4f,150.4f,250.4f,350.4f,500.4f};
    private float[] pol_lo_breakpoints = new float[] { 0.0f, 12.1f, 35.5f, 55.5f, 150.5f, 250.5f, 350.5f};
    //AQI high and low breakpoints 
    //values from https://aqicn.org/faq/2013-09-09/revised-pm25-aqi-breakpoints/
    private float[] aqi_hi_breakpoints = new float[] { 50f, 100f, 150f, 200f, 300f, 400f, 500f };
    private float[] aqi_lo_breakpoints = new float[] { 0f, 51f, 101f, 151f, 201f, 301f, 401f };
    //consequences of aqi levels
    public string[] Health_Implications = new string[] { "Air quality is considered satisfactory, and air pollution poses little or no risk", "Unusually sensitive people should consider reducing prolonged or heavy exertion.", "Increasing likelihood of respiratory symptoms in sensitive individuals, aggravation of heart or lung disease and premature mortality in persons with cardiopulmonary disease and the elderly.", "Increased aggravation of heart or lung disease and premature mortality in persons with cardiopulmonary disease and the elderly; increased respiratory effects in general population.", "Significant aggravation of heart or lung disease and premature mortality in persons with cardiopulmonary disease and the elderly; significant increase in respiratory effects in general population.", "Serious aggravation of heart or lung disease and premature mortality in persons with cardiopulmonary disease and the elderly; serious risk of respiratory effects in general population.", "Health alert: everyone may experience more serious health effects" };
    public string[] Cautionary_Statement = new string[] { "None", "Active children and adults, and people with respiratory disease, such as asthma, should limit prolonged outdoor exertion.", "Active children and adults, and people with respiratory disease, such as asthma, should limit prolonged outdoor exertion.", "Active children and adults, and people with respiratory disease, such as asthma, should avoid prolonged outdoor exertion; everyone else, especially children, should limit prolonged outdoor exertion", "Active children and adults, and people with respiratory disease, such as asthma, should avoid all outdoor exertion; everyone else, especially children, should limit outdoor exertion.", "Everyone should avoid all outdoor exertion", "Everyone should avoid all outdoor exertion" };
    public string[] Categorys = new string[] { "Good", "Moderate", "Unhealty for Sensitive Groups", "Unhealty", "Very Unhealthy", "Hazardous","Hazardous+" };
    //Test
    /* public float cp;
      private void Start()
      {
          print(Calculate(cp));
          print(index);
      }
    */


    private void Get_Index(float C)
    {
        for(int i=0;i<pol_hi_breakpoints.Length;i++)
        {
            if(pol_hi_breakpoints[i]>C)
            {
                index = i;
                return;
            }
        }
    }

   

    public float Calculate(float Cp)
    {
        //formula from https://metone.com/how-to-calculate-aqi-and-nowcast-indices/
        float Ip;

        Get_Index(Cp);
        Ip = (aqi_hi_breakpoints[index] - aqi_lo_breakpoints[index]) / (pol_hi_breakpoints[index] - pol_lo_breakpoints[index]) * (Cp - pol_lo_breakpoints[index])+aqi_lo_breakpoints[index];
        AQI = Mathf.FloorToInt(Ip);
        if (debug)
        {
            print(aqi_hi_breakpoints[index]);
            print(aqi_lo_breakpoints[index]);
            print(pol_hi_breakpoints[index]);
            print(pol_lo_breakpoints[index]);
        }

        return Ip;
    }

}
