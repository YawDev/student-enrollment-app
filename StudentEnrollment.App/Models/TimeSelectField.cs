using System.Collections.Generic;
using StudentEnrollment.Enum;

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

     public partial class GradeSelectField
    {
        public List<string> Options { get; set; }

        public Dictionary<string, LetterGrade> OptionEnumValues { get; set; }

        public GradeSelectField()
        {
            Options =  new List<string>()
            {
                "A+",
                "A",
                "A-",
                "B+",
                "B",
                "B-",
                "C+",
                "C",
                "C-",
                "D+",
                "D",
                "D-",
                "F",
                "W",
               
            };
            
            OptionEnumValues = new Dictionary<string, LetterGrade>()
            {
                {"A+", LetterGrade.A_Plus},
                {"A", LetterGrade.A},
                {"A-", LetterGrade.A_Minus},
                {"B-", LetterGrade.B_Minus},
                {"B+", LetterGrade.B_Plus},
                {"B", LetterGrade.B},
                {"C", LetterGrade.C},
                {"C+", LetterGrade.C_Plus},
                {"C-", LetterGrade.C_Minus},
                {"D+", LetterGrade.D_Plus},
                {"D-", LetterGrade.D_Minus},
                {"D", LetterGrade.D},
                {"F", LetterGrade.F},
                {"W", LetterGrade.W},
            };

            
    } 
        
        
        
    }
}