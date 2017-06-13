using System.Collections.Generic;

public class MenuItemModel
{
    public MenuItemModel()
    {
        SubMenu = new List<MenuItemModel>();
    }

    public string Text { get; set; }
    public string Controller { get; set; }
    public string Action { get; set; }
    public bool Selected { get; set; }

    public List<MenuItemModel> SubMenu { get; private set; }
}