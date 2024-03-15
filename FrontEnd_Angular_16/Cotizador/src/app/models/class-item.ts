export interface ClassItem {
    Id: number,
    ClassData: string,
    CrudHistoryList: string,
    Active: boolean
}

export interface ClassDataItem {
    [key: string]: Array<ClassCode>
}

export interface ClassCode {
    [key: string]: string
}