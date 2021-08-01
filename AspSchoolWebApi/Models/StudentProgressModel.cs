using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspSchoolWebApi.Models
{

    public class StudentProgressForsubjectModel
    {
        public Subject Subject { get; set; }
        public decimal totalofsubject { get; set; }
        public decimal sumofdegrees { get; set; }

        public List< studentprogressmodel>  submodel  { get; set; }





    }

    public class studentprogressmodel
    {

        public STask task { get; set; }
        public Answer? ans { get; set; }





    }
}
