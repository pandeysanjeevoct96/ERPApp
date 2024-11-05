using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.Optimization;
using System.Web.UI;

namespace ERPApp
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            //ScriptResourceMapping.Register(
            //    "jquery",
            //    new ScriptResourceDefinition
            //    {
            //        CdnPath = "https://ajax.googleapis.com/ajax/libs/jquery/3.5.1/jquery.min.js",
            //        CdnDebugPath = "https://ajax.googleapis.com/ajax/libs/jquery/3.5.1/jquery.js",
            //        LoadSuccessExpression = "jQuery"
            //    });
            ScriptManager.ScriptResourceMapping.AddDefinition("jquery", new ScriptResourceDefinition
            {
                Path = "https://ajax.googleapis.com/ajax/libs/jquery/3.5.1/jquery.min.js",
                CdnPath = "https://ajax.googleapis.com/ajax/libs/jquery/3.5.1/jquery.min.js",
                LoadSuccessExpression = "jQuery" // This is used to verify if jQuery is loaded
            });
        }
    }
}