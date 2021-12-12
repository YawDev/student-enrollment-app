using System.Collections.Generic;

namespace StudentEnrollment.App.Models
{
    public partial class TimeSelectField
    {
        public List<string> StartTime { get; set; }
        public List<string> EndTime { get; set; }
        public TimeSelectField()
        {
            StartTime =  new List<string>()
            {
                "8:00am",
                "9:00am",
                "10:00am",
                "11:00am",
                "12:00pm",
                "1:00pm",
                "2:00pm",
                "3:00pm",
                "4:00pm",
                "5:00pm",
                "6:00pm",
                "7:00pm",
                "8:00pm",
                "9:00pm",
            };

            EndTime =  new List<string>()
            {
                "8:00am",
                "9:00am",
                "10:00am",
                "11:00am",
                "12:00pm",
                "1:00pm",
                "2:00pm",
                "3:00pm",
                "4:00pm",
                "5:00pm",
                "6:00pm",
                "7:00pm",
                "8:00pm",
                "9:00pm",
            };
    } 
        
        
        
    }
}