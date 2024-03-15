import { CrudHistory } from "./crud-history-item";
import { ProductItem } from "./product-item";

export interface ProgramItem {
    Id: number,
    Code: string,
    Description: string,
    CertificateIdentifier: string,
    SalesChannel: number,
    IdGroup: number,
    CrudHistoryList: Array<CrudHistory> | string,
    Active: boolean,
    ListProduct: Array<ProductItem>
}