using System.Collections.Generic;
using StudentEnrollment.Enum;

namespace StudentEnrollment.App.Models
{
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