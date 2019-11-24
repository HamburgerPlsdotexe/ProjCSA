using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProjectCSA.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace ProjectCSA.Models
{
    public class ScheduleModel
    {
        public string LessonCode { get; set; }
        public string Day { get; set; }
        public string Classroom { get; set; }
        public int[] Hours { get; set; }
        public string Class { get; set; }

        readonly static string Tcode = LoginController.Tcode;
        string jsonPath = $@"../Content/{Tcode}.json";

        void ReadJson()
        {
            using (StreamReader r = new StreamReader(jsonPath))
            {
                string json = r.ReadToEnd();
                List<string> items = JsonConvert.DeserializeObject<List<string>>(json);
            }
        }
    }
}