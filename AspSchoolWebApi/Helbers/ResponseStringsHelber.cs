using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspSchoolWebApi.Helbers
{
  public static class Helber
    {

        public static class ResponseStringsHelber
        {

            public static string ValidationError { get { return "validation Error"; } }
            public static string AddedSucessFully { get { return "Added SucessFully"; } }
            public static string UpdatedSucessFully { get { return "Updated SucessFully"; } }
            public static string DeletedSucessFully { get { return "Deleted SucessFully"; } }

            public static string SavingError { get { return "Saving Error"; } }
            public static string NullParams { get { return "Null Params"; } }

            public static string NotExist { get { return "NotExist"; } }
            public static string CanNotDeleted { get { return "Can Not Deleted"; } }




        }
    }
}
