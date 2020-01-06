using static ProjectCSA.DateOperations;
using Newtonsoft.Json;
using ProjectCSA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Hosting;
namespace ProjectCSA
{
    public class JsonOperations
    {
        public static void PopulateJson(string tcode)
        {
            Random rnd = new Random();
            string[] Days = new string[7] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };     //days of the week.
            int ZeroToFiveOuter = rnd.Next(0, 6);

            string[] Classes = new string[5] { "INF2A", "INF2B", "INF2C", "INF2D", "INF2E" };

            string[] ClassRooms = new string[] { $"H.{ZeroToFiveOuter}.114", $"WD.{ZeroToFiveOuter}.002", $"WD.{ZeroToFiveOuter}.016", $"H.{ZeroToFiveOuter}.403", $"H.{ZeroToFiveOuter}.315", $"H.{ZeroToFiveOuter}.308", $"H.{ZeroToFiveOuter}.306", $"H.{ZeroToFiveOuter}.206", $"H.{ZeroToFiveOuter}.204", $"H.{ZeroToFiveOuter}.110", $"H.{ZeroToFiveOuter}.405" };
            List<ScheduleModel> _data = new List<ScheduleModel>();
            for (int i = 0; i < 200; i++)
            {
                int OneToFive = rnd.Next(1, 6);                                                                                         //pick random day of the week.
                int maxvalue = ClassRooms.Length;                                                                                       //random value from array.
                int ClassRoomInt = rnd.Next(0, maxvalue);                                                                               //random int to pick from array.
                int Hours = rnd.Next(1, 13);                                                                                            //pick random hour of the day between 1 and 13, and add the next to upcoming hours into the array.
                int[] hours = new int[] { Hours, Hours + 1, Hours + 2 };
                string ClassCode = Classes[OneToFive - 1];
                string Weeks = ReturnWeeks();
                ScheduleModel tempmodel = new ScheduleModel()
                {
                    Week = Weeks,
                    Day = Days[OneToFive],
                    Classroom = ClassRooms[ClassRoomInt],
                    Hours = hours,
                    Class = ClassCode,
                    LessonCode = ClassCode                                                                                      //INF1I-49-Date
                };
                if (DoesExist(_data, tempmodel))
                {
                    _data.Add(tempmodel);
                }
                else
                {

                }
            };

            string json = JsonConvert.SerializeObject(_data.ToArray());
            string Tcode = tcode.ToUpper();
            System.IO.File.WriteAllText(HostingEnvironment.MapPath($@"~/Content/{Tcode}.json"), json);
        }
        public static bool DoesExist(List<ScheduleModel> _data, ScheduleModel tempmodel)
        {
            bool isEmpty = !_data.Any();
            if (isEmpty)
            {
                return true;
            }
            bool alreadyExists = _data.Any(x => x.Day == tempmodel.Day && x.Week == tempmodel.Week && x.Hours.Intersect(tempmodel.Hours).Any()
);
            if (alreadyExists)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static void JsonClass(string LessonCode)
        {
            List<JsonAttendance> _data = new List<JsonAttendance>();
                JsonAttendance Model = new JsonAttendance()
                {
                    

                };

            string json = JsonConvert.SerializeObject(_data.ToArray());
            System.IO.File.WriteAllText(HostingEnvironment.MapPath($@"~/Content/{LessonCode}.json"), json);
        }
    }
   
}