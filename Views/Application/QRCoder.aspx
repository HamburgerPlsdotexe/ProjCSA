<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="QRCoder.aspx.cs" Inherits="ProjectCSA.QRCoder_Page" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:TextBox ID="qr_text_input" runat="server"></asp:TextBox>
            <br />
            <asp:Button ID="qr_button" runat="server" Text="Generate QR" OnClick="qr_button_Click" />
            <br />
            <asp:PlaceHolder ID="PlaceholderQR" runat="server"></asp:PlaceHolder>

        </div>
    </form>
</body>
</html>
