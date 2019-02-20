using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;



[MyCompany.Web.AquariumFieldEditor()]
public partial class Controls_RichEditor : System.Web.UI.UserControl
{
    
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!(IsPostBack))
        	Page.ClientScript.RegisterClientScriptBlock(GetType(), "ClientScripts", @"
                                
function FieldEditor_Element() {
  var list = document.getElementsByTagName('div');
  for (var i = 0; i < list.length; i++) {
      var elem = list[i];
      if (elem.className && elem.className.match(/ajax__html_editor_extender_texteditor/))
         return elem;
  }
  return null;
}                                 
function FieldEditor_GetValue(){return FieldEditor_Element().innerHTML;}
function FieldEditor_SetValue(value) {FieldEditor_Element().innerHTML=value;}
                              ", true);
    }
}
