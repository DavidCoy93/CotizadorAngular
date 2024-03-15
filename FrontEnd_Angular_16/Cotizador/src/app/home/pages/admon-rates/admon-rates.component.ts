import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FormArray, FormControl, FormGroup, Validators } from '@angular/forms';
import { MatSelectChange } from '@angular/material/select';
import { ProductItem } from 'src/app/models/product-item';
import { ProgramItem } from 'src/app/models/program-item';
import { GlobalAutomotiveService } from 'src/app/services/global-automotive.service';
import { HomeComponent } from '../../home.component';
import { CrudHistory } from 'src/app/models/crud-history-item';
import { MatDialog } from '@angular/material/dialog';
import { ModalCrudComponent } from '../../components/modal-crud/modal-crud.component';
import { ActivatedRoute } from '@angular/router';


export type TControlRate = number | string | boolean | null;

export interface FormControlsRate {
  [key: string]: FormControl<TControlRate>
}

export interface RateForm {
  Program: FormControl<number>,
  Product: FormControl<number>,
  Rates: FormArray<FormGroup<FormControlsRate>>
}

@Component({
  selector: 'app-admon-rates',
  templateUrl: './admon-rates.component.html',
  styleUrls: ['./admon-rates.component.scss']
})
export class AdmonRatesComponent implements OnInit {

  programs: Array<ProgramItem> = [];
  products: Array<ProductItem> = [];

  RateForm: FormGroup<RateForm> = new FormGroup<RateForm>(
    {
      Program: new FormControl<number>({value: 0, disabled: false}, {nonNullable: true}),
      Product: new FormControl<number>({value: 0, disabled: true}, {nonNullable: true}),
      Rates: new FormArray<FormGroup<FormControlsRate>>([])
    }
  )

  isSmallDevice: boolean = false;

  constructor(
    private globalAutoService: GlobalAutomotiveService,
    private homeComponent: HomeComponent,
    private route: ActivatedRoute
  ) {
    
    
  }


  get rateControlNames() { return Object.keys(this.RateForm.controls.Rates.controls) }

  get arrayForms() { return this.RateForm.controls.Rates.controls }

  ngOnInit(): void {

    this.homeComponent.$IsSmall.subscribe(value => {
      this.isSmallDevice = value;
    })

    this.globalAutoService.GetPrograms().subscribe(
      {
        next: (response) => {
          this.programs = response;
          const IdProgram = this.route.snapshot.queryParams['pr'] || '';
          if (typeof IdProgram === 'string') {
            if (IdProgram !== '') {
              this.RateForm.controls.Program.setValue(Number(IdProgram), {onlySelf: true});
              let product = this.RateForm.controls.Product;

              const resetValuesProduct = (): void => {
                product.setValue(0, {onlySelf: true});
                product.disable({onlySelf: true});

                if(this.arrayForms.length > 0) {
                  this.RateForm.controls.Rates.clear();
                }
              }
              this.globalAutoService.GetProducts(this.RateForm.controls.Program.value).subscribe(
                {
                  next: (response) => {
        
                    
                    if (this.arrayForms.length > 0) {
                      this.RateForm.controls.Rates.clear();
                    }
        
                    if (product.disabled) {
                      product.enable({onlySelf: true});
                    } else {
                      if (product.value !== 0) {
                        product.setValue(0);
                      }
                    }
                    
                    this.products = response;
                    product.updateValueAndValidity({onlySelf: true});
                    
                  },
                  error: (err: HttpErrorResponse) => {
                    if (product.enabled) {
                      resetValuesProduct();
                    }
                  }
                }
              );
            }
          }
        },
        error: (err: HttpErrorResponse) => {
          let product = this.RateForm.controls.Product;
          product.setValue(0, {onlySelf: true})
          if (product.enabled) {
            product.disable();
          }
        }
      }
    )
  }

  changeProgram(program: MatSelectChange): void {
    let product = this.RateForm.controls.Product;

    const resetValuesProduct = (): void => {
      product.setValue(0, {onlySelf: true});
      product.disable({onlySelf: true});

      if(this.arrayForms.length > 0) {
        this.RateForm.controls.Rates.clear();
      }
    }


    if (program.value !== 0) {

      this.globalAutoService.GetProducts(program.value).subscribe(
        {
          next: (response) => {

            
            if (this.arrayForms.length > 0) {
              this.RateForm.controls.Rates.clear();
            }

            if (product.disabled) {
              product.enable({onlySelf: true});
            } else {
              if (product.value !== 0) {
                product.setValue(0);
              }
            }
            
            this.products = response;
            product.updateValueAndValidity({onlySelf: true});
            
          },
          error: (err: HttpErrorResponse) => {
            if (product.enabled) {
              resetValuesProduct();
            }
          }
        }
      );
    } else {
      if (product.enabled) {
        resetValuesProduct();
      }
    }
  }


  changeProduct(product: MatSelectChange): void {
    if (product.value !== 0) {
      this.globalAutoService.getRateList(this.RateForm.controls.Program.value, product.value).subscribe(
        {
          next: (rateList) => {

            rateList.every((val, ind) => {
              val.Active = false;
              return true;
            });

            rateList.forEach((rate, index) => {
              const formGroup = new FormGroup<FormControlsRate>({});

              for (const key in rate) {
                const property = rate[key as keyof typeof rate];
                if (typeof property === 'string') {

                  if (property.startsWith('[', 0) || property.startsWith('{', 0)) {
                    const crudHistory = JSON.parse(property);

                    if (Array.isArray(crudHistory)) {
                      let crudHistoryList = crudHistory as Array<CrudHistory>;
                      for (const history of crudHistoryList) {
                        if (history.Type === 'C') {
                          formGroup.addControl('CreationDate', new FormControl<string>(history.Date));
                        } else if (history.Type === 'D') {
                          formGroup.addControl('DeleteDate', new FormControl<string>(history.Date));
                        }
                      }
                    }
                  } else {
                    formGroup.addControl(key, new FormControl<string>(property));
                  }
                } else if (typeof property === 'number') {

                  switch(key) {
                    case 'Deductible':
                    case 'PriceCert':
                    case 'PriceEW':
                    case 'PriceWP':
                    case 'MKT':
                    case 'PriceRA':
                    case 'Prime':
                    case 'AgencyComission':
                    case 'VendorComission':
                    case 'DealerComission':
                    case 'AdminFee':
                    case 'InsurancePolicy':
                    case 'CommAnzen':
                    case 'Rewards':
                    case 'CommNR':
                    case 'CommPlant':
                    case 'RiskPremium':
                    case 'Profit':
                    case 'Taxes_Percent':
                      formGroup.addControl(key, new FormControl<number>(property));
                      break;
                    case 'Term':
                      formGroup.addControl(key, new FormControl<number>({ value: property, disabled: true }));
                      break;
                  }
                } else if (typeof property === 'boolean') {
                  formGroup.addControl(key, new FormControl<boolean>(property));
                }
              }

              this.RateForm.controls.Rates.push(formGroup);
            })
          }
        }
      )
    } else {
      if (this.arrayForms.length > 0) {
        this.RateForm.controls.Rates.clear();
      }
    }
    this.RateForm.updateValueAndValidity({onlySelf: false});
  }


  getControlNamesFromGroup(formGroup: FormGroup): Array<string> {
    return Object.keys(formGroup.controls);
  }

  showHideFormRate(control: FormControl, spanElement: HTMLSpanElement): void {
    if (spanElement.innerText === 'Mostrar') {
      control.setValue(true);
    } else {
      control.setValue(false);
    }

    control.updateValueAndValidity({onlySelf: true});
  }

}
