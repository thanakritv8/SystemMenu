using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using MyCompany.Data;
using MyCompany.Models;
using System.Xml;
using System.Web.Configuration;

namespace MyCompany.Rules
{
	public partial class SystemMenuBusinessRules : MyCompany.Data.BusinessRules
    {
        
        /// <summary>
        /// This method will execute in any view for an action
        /// with a command name that matches "Custom" and argument that matches "DTPMS".
        /// </summary>
        [Rule("r101")]
        public void r101Implementation(SystemMenuModel instance)
        {
            System.Collections.ArrayList mass = new System.Collections.ArrayList();
            System.Collections.ArrayList key1 = new System.Collections.ArrayList();
            System.Collections.ArrayList key2 = new System.Collections.ArrayList();
            System.Collections.ArrayList key3 = new System.Collections.ArrayList();
            using (SqlText sql = new SqlText(String.Format("SELECT PageName, PageDescription, PageRoles, PageUrl FROM SystemMenu")))
            {
                System.Data.Common.DbDataReader readerSender = sql.ExecuteReader();
                while (readerSender.Read())
                {
                    //Web.Sitemap
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(HttpContext.Current.Server.MapPath("~/Web.sitemap"));
                    XmlNodeList elementList = xmlDoc.GetElementsByTagName("siteMapNode");
                    foreach (XmlNode node in elementList)
                    {
                        if (node.Attributes["title"].Value == readerSender["PageName"].ToString())
                        {
                            XmlAttribute description = xmlDoc.CreateAttribute("description");
                            node.Attributes.Append(description);
                            node.Attributes["description"].Value = readerSender["PageDescription"].ToString();

                            XmlAttribute url = xmlDoc.CreateAttribute("url");
                            node.Attributes.Append(url);
                            node.Attributes["url"].Value = readerSender["PageUrl"].ToString();

                            XmlAttribute roles = xmlDoc.CreateAttribute("roles");
                            node.Attributes.Append(roles);
                            node.Attributes["roles"].Value = readerSender["PageRoles"].ToString();

                            xmlDoc.Save(HttpContext.Current.Server.MapPath("~/Web.sitemap"));
                        }
                    }
                    //Web.config
                    xmlDoc = new XmlDocument();
                    xmlDoc.Load(HttpContext.Current.Server.MapPath("~/web.config"));



                    bool pathXml = true;
                    elementList = xmlDoc.GetElementsByTagName("location");
                    foreach (XmlNode node in elementList)
                    {
                        if (node.Attributes["path"].Value == readerSender["PageUrl"].ToString().Substring(2))
                        {
                            //node.InnerXml
                            pathXml = false;
                            string rp = "<system.web><authorization><allow roles=\"" + readerSender["PageRoles"].ToString() + "\" /><deny users=\" * \" /></authorization></system.web>";
                            node.InnerXml = rp;
                            xmlDoc.Save(HttpContext.Current.Server.MapPath("~/web.config"));
                        }
                    }
                    if (pathXml && readerSender["PageRoles"].ToString() != "*")
                    {
                        XmlElement locationNode = xmlDoc.CreateElement("location");
                        locationNode.SetAttribute("path", readerSender["PageUrl"].ToString().Substring(2));
                        XmlNodeList elementListConfig = xmlDoc.GetElementsByTagName("configuration");
                        XmlElement swNode = xmlDoc.CreateElement("system.web");
                        locationNode.AppendChild(swNode);
                        XmlElement authorizationNode = xmlDoc.CreateElement("authorization");
                        swNode.AppendChild(authorizationNode);
                        XmlElement allowNode = xmlDoc.CreateElement("allow");
                        allowNode.SetAttribute("roles", readerSender["PageRoles"].ToString());
                        authorizationNode.AppendChild(allowNode);
                        XmlElement denyNode = xmlDoc.CreateElement("deny");
                        denyNode.SetAttribute("users", " * ");
                        authorizationNode.AppendChild(denyNode);

                        foreach (XmlNode nodeConfig in elementListConfig)
                        {
                            nodeConfig.AppendChild(locationNode);
                            xmlDoc.Save(HttpContext.Current.Server.MapPath("~/web.config"));
                        }

                    }
                }
                readerSender.Close();
                readerSender.Dispose();
            }
            
        }
    }
}
