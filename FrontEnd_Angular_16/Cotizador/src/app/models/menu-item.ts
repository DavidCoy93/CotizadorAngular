import { CrudHistory } from "./crud-history-item";

export interface MenuItem {
    Id: number,
    Active: boolean,
    MenuData: MenuDataItem,
    CrudHistoryList: Array<CrudHistory>
}

export interface MenuDataItem {
    Description: string,
    IdParent: number,
    Order: number,
    IsPrivate: string,
    ControllerName: string,
    ActionName: string
}


export interface UserMenu {
    Menu: MenuItem,
    SubMenus: Array<MenuItem>
}



