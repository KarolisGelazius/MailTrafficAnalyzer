<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Form1.aspx.cs" Inherits="U5_18.Form1" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>E-mail analizė</title>
    <link href="Style.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <asp:Label ID="LabelMainTitle" runat="server" Text="E-mail laiškų sistema" CssClass="main-title"></asp:Label>
            
            <div class="upload-section">
                <asp:FileUpload ID="FileUpload1" runat="server" AllowMultiple="True" />
                <asp:Button ID="Button1" runat="server" Text="Nuskaityti duomenis" OnClick="Button1_Click" />
            </div>
            
            <asp:Label ID="LabelMessage" runat="server" CssClass="error-msg"></asp:Label>
            
            <asp:PlaceHolder ID="PlaceHolderData" runat="server"></asp:PlaceHolder>
            
            <asp:Panel ID="PanelAnalysis" runat="server" Visible="false">
                
                <div class="analysis-section">
                    <asp:Label ID="LabelAnalysisTitle1" runat="server" Text="1. Serverių neaktyvumas" CssClass="section-title"></asp:Label>
                    
                    <div class="control-group">
                        <asp:Button ID="Button2" runat="server" Text="Vykdyti neaktyvumo analizę" OnClick="Button2_Click" />
                    </div>
                    <asp:PlaceHolder ID="PlaceHolderInactive" runat="server"></asp:PlaceHolder>
                </div>

                <div class="analysis-section">
                    <asp:Label ID="LabelAnalysisTitle2" runat="server" Text="2. Mažiausiai siuntę asmenys" CssClass="section-title"></asp:Label>
                    
                    <div class="control-group">
                        <asp:Label ID="LabelSelectDate" runat="server" Text="Pasirinkite analizės dieną:" CssClass="date-selection-label"></asp:Label>
                        <br />
                        <asp:DropDownList ID="DropDownListDates" runat="server" CssClass="dropdown-style"></asp:DropDownList>
                        <br />
                    </div>
                    <div class="control-group">
                        <asp:Button ID="Button3" runat="server" Text="Rasti mažiausiai siuntusius" OnClick="Button3_Click" />
                    </div>
                    <asp:PlaceHolder ID="PlaceHolderMinSenders" runat="server"></asp:PlaceHolder>
                </div>

            </asp:Panel>
        </div>
    </form>
</body>
</html>