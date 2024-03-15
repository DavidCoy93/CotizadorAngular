import { CrudHistory } from "./crud-history-item";
import { ProductPriceItem } from "./product-price-item";
import { RateItem } from "./rate-item";

export interface ProductItem {
    Id: number,
    Description: string,
    WebsiteDescription: string,
    IdProductType: number,
    BrandCode: string,
    PeriodLowerLimit: number,
    PeriodUpperLimit: number,
    KilometerLowerLimit: number,
    KilometerUpperLimit: number,
    IdProgram: number,
    Services: boolean,
    TypeWarranty: string,
    CrudHistoryList: Array<CrudHistory> | string,
    Template: string,
    Active: boolean,
    RateForms: Array<ProductPriceItem>
    ListPrices?: Array<RateItem>
}