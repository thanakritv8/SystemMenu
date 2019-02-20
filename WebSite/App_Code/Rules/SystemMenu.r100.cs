using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using MyCompany.Data;
using MyCompany.Models;
using System.Xml.Linq;
using System.Web.UI.WebControls;
using System.Xml;

namespace MyCompany.Rules
{
	public partial class SystemMenuBusinessRules : MyCompany.Data.BusinessRules
    {
        
        /// <summary>
        /// This method will execute in any view for an action
        /// with a command name that matches "Custom" and argument that matches "SMTD".
        /// </summary>
        [Rule("r100")]
        public void r100Implementation(SystemMenuModel instance)
        {
            using (XmlReader reader = XmlReader.Create(HttpContext.Current.Server.MapPath("~/web.sitemap")))
            {
                string title = string.Empty;
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {

                        switch (reader.Name)
                        {
                            case "siteMapNode":
                                string name = reader["title"];
                                string description = reader["description"];
                                string url = reader["url"];
                                string roles = reader["roles"] == null?string.Empty:reader["roles"];
                                if (!name.Contains('^'))
                                //if (title != null && description != null && url != null && roles != null && !name.Contains('^'))
                                {
                                    using (SqlProcedure sp = new SqlProcedure("sp_ConfigSystemMenu"))
                                    {
                                        if (description != string.Empty)
                                        {
                                            //sp.AddParameter("@title", title);
                                            sp.AddParameter("@name", name);
                                            sp.AddParameter("@description", description);
                                            sp.AddParameter("@url", url);
                                            sp.AddParameter("@roles", roles);
                                            sp.AddParameter("@username", Context.User.Identity.Name);
                                            sp.ExecuteScalar();
                                        }
                                            
                                    }
                                    //if (description == string.Empty)
                                    //{
                                    //    title = reader["title"];
                                    //}
                                    //else if (title != string.Empty)
                                    //{
                                    //    using (SqlProcedure sp = new SqlProcedure("sp_ConfigSystemMenu"))
                                    //    {

                                    //        sp.AddParameter("@title", title);
                                    //        sp.AddParameter("@name", name);
                                    //        sp.AddParameter("@description", description);
                                    //        sp.AddParameter("@url", url);
                                    //        sp.AddParameter("@roles", roles);
                                    //        sp.AddParameter("@username", Context.User.Identity.Name);
                                    //        sp.ExecuteScalar();
                                    //    }
                                    //}
                                }

                                break;
                        }
                    }
                }
            }
        }
    }
}
