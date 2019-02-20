<%@ Control Language="C#" AutoEventWireup="true" CodeFile="RichEditor.ascx.cs" Inherits="Controls_RichEditor"  %>
              
<asp:TextBox ID="TextBox1" runat="server" Columns="50" Rows="10" Height="241px"
    Width="450px"></asp:TextBox>
<act:HtmlEditorExtender ID="Editor1" runat="server" TargetControlID="TextBox1" EnableSanitization="false">
</act:HtmlEditorExtender>
            