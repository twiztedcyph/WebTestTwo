using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json.Linq;
using WebTestTwo;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace WebTestTwo
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        string vessel, key;

        protected void Page_Load(object sender, EventArgs e)
        {
            vessel = Request.QueryString["Vessel"];
            key = Request.QueryString["Key"];

            if (vessel != null && key != null && checkKey(key))
            {
                System.Net.WebClient webClient = new System.Net.WebClient();
                string reply = webClient.DownloadString("http://151.236.58.241:8099/api/WorkPlan/GetDailyWorkPlan");

                var json = JsonConvert.DeserializeObject<RootObject>(reply);

                foreach (var ves in json.vesselList)
                {
                    if (Regex.Replace(ves.vesselName.ToLower(), @"\s+", "").Equals(vessel.ToLower()))
                    {
                        mainBody.InnerHtml += "<header role=\"banner\"></header><main role=\"main\"><div class=\"vesselNameDiv\">";
                        mainBody.InnerHtml += "<p class=\"vesselName\">" + ves.vesselName + "</p></ div > ";
                        mainBody.InnerHtml += "<div class=\"dailyInfo\"><p class=\"vesselSubInfo\">Daily plan</p><p class=\"vesselSubInfo\">" + json.date + "</p></div>";
                        mainBody.InnerHtml += "<div class= \"textboxtitle\"><p class =\"SummaryTitle\">Summary</p></div><div class= \"textbox\"><p class =SummaryTextboxText>" + 
                            ves.summary + "</p></div>";

                        foreach (var sch in ves.schedule)
                        {
                            mainBody.InnerHtml += "<div class=\"vesselSummary\"> <p class=\"WFname\">" + sch.turbineName + "</p>";
                            mainBody.InnerHtml += "<p class=\"WFheader\">Instruction</p> <p class=\"WFtext\">" + sch.instructions + "</p>";
                            mainBody.InnerHtml += "<p class=\"WFheader\">Personnel</p> <p class=\"WFtext\">";

                            foreach (var per in sch.personnelList)
                            {
                                mainBody.InnerHtml += per + "<br/>";
                            }

                            mainBody.InnerHtml += "</p> <p class=\"WFheader\">Inventory</p> <p class=\"WFtext\">";

                            foreach (var inv in sch.inventoryList)
                            {
                                mainBody.InnerHtml += inv + "<br/>";
                            }

                            mainBody.InnerHtml += "</p> </div>";
                        }

                        mainBody.InnerHtml += "";
                        mainBody.InnerHtml += "";
                        return;
                    }
                }
                pageTitle.Text = "Not found";
                mainBody.InnerHtml = "<H1>Vessel/plan not found</H1>";
            }
            else
            {
                pageTitle.Text = "404";
                mainBody.InnerHtml = "<H1>404</H1>";
            }
        }

        private bool checkKey(string inputKey)
        {
            return inputKey.Equals("0WM51");
        }
    }

    public class Schedule
    {
        [JsonProperty(PropertyName = "turbine Name")]
        public string turbineName { get; set; }
        public string instructions { get; set; }
        [JsonProperty(PropertyName = "personnel List")]
        public List<string> personnelList { get; set; }
        [JsonProperty(PropertyName = "inventory List")]
        public List<string> inventoryList { get; set; }
    }

    public class VesselList
    {
        [JsonProperty(PropertyName = "vessel Name")]
        public string vesselName { get; set; }
        public List<Schedule> schedule { get; set; }
        public string summary { get; set; }
    }

    public class RootObject
    {
        public string date { get; set; }
        [JsonProperty(PropertyName = "vessel List")]
        public List<VesselList> vesselList { get; set; }
    }
}
