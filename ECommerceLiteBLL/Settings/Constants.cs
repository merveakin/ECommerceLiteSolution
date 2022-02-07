using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceLiteBLL.Settings
{
    public static class Constants
    {
        public static string EmailAddress
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["MAILADDRESS"].ToString();
                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message + " There should be a key named 'MAILADDRESS' in the WebConfig file.");
                }
            }
        }

        public static string EmailPassword
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["MAILPASSWORD"].ToString();
                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message + " There should be a key named 'MAILPASSWORD' in the WebConfig file.");
                }
            }
        }
    }
}
