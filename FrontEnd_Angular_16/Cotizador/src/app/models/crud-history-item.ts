import { FormControl } from "@angular/forms"

export interface CrudHistory {
    Date: string,
    Type: string,
    IdUser: number,
    Comments: string
}


export interface CrudHistoryFormItem {
    Date: FormControl<string>,
    Type: FormControl<string>,
    IdUser: FormControl<number>,
    Comments: FormControl<string>
}