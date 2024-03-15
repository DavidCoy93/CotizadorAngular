import { CrudHistory } from "./crud-history-item"

export interface User {
    Id: number|null,
    Email: string,
    Password: string,
    ChangePass: boolean | null,
    EmailpasswordRecovery: string | null,
    Active: boolean | null,
    Userdata: UserData,
    CrudHistoryList: Array<CrudHistory>
}

export interface UserData {
    Name: string,
    MiddleName: string,
    PaternalSurname: string,
    MaternalSurname: string,
    IdBrand: number,
    IdRole: number,
    IdDistributor: number,
    IdLevel: number,
    Goal: number
}

export interface LoginResponse {
    user: User,
    token: string
}