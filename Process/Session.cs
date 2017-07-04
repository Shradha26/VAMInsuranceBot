using System;
using System.Configuration;

namespace VAMInsuranceBot.Process
{
    public class Session
    {
        public static bool CheckSession(DateTime dateTime)
        {
            if ((DateTime.UtcNow - dateTime).TotalMinutes <= Convert.ToInt32(ConfigurationManager.AppSettings["TimeoutInMinutes"]))
                return true;
            else
                return false;
        }
    }
}