<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ProductCollection.aspx.cs" Inherits="ProductCollection" MasterPageFile="~/HeaderFooter.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:Label ID="lblMessage" runat="server" Font-Size="XX-Large" ForeColor="Red" Text="Your cart is empty"
        Visible="False" Style="padding-top: 500px;"></asp:Label>
    <asp:Repeater ID="Repeater1" runat="server">
        <ItemTemplate>
            <asp:Label ID="lblHiddenCartID" runat="server" Text='<%#Eval("ProductOrderStatusId") %>' Visible="False"></asp:Label>
            <div class="cartWizard">
                <table cellspacing="40" class="tblCart">
                    <tr>
                        <td class="cartImageContainer">
                            <asp:Image ID="imgProductImage" ImageUrl='<%#Eval("ProductImageURL") %>' runat="server" />
                        </td>
                        <td>
                            <asp:Label ID="lblProductName" runat="server" Text='<%#Eval("ProductName") %>'></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlQuantity" runat="server">
                                <asp:ListItem>1</asp:ListItem>
                                <asp:ListItem>2</asp:ListItem>
                                <asp:ListItem>3</asp:ListItem>
                                <asp:ListItem>4</asp:ListItem>
                                <asp:ListItem>5</asp:ListItem>
                                <asp:ListItem>6</asp:ListItem>
                                <asp:ListItem>7</asp:ListItem>
                                <asp:ListItem>8</asp:ListItem>
                                <asp:ListItem>9</asp:ListItem>
                                <asp:ListItem>10</asp:ListItem>
                                <asp:ListItem>11</asp:ListItem>
                                <asp:ListItem>12</asp:ListItem>
                                <asp:ListItem>13</asp:ListItem>
                                <asp:ListItem>14</asp:ListItem>
                                <asp:ListItem>15</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>*
                        </td>
                        <td>
                            <asp:Label ID="lblPrice" runat="server" Text='<%#"Rs. "+Eval("ProductPrice") %>'></asp:Label>
                        </td>
                        <td>
                            <asp:LinkButton ID="lnkRemoveItem" runat="server" Style="color: White; background: silver; padding: 5px; border-radius: 5px"
                                OnClick="lnkRemoveItem_Click" CausesValidation="False">X</asp:LinkButton>
                        </td>
                    </tr>
                </table>
            </div>
        </ItemTemplate>
    </asp:Repeater>
     <asp:Button ID="Button1" runat="server" Text="Place Order" OnClick="Button1_Click"/>
</asp:Content>
