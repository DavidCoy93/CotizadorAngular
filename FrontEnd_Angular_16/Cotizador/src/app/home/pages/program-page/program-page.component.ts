import { Component, OnInit } from '@angular/core';
import { FormArray, FormControl, FormGroup, Validators } from '@angular/forms';
import { faCircleInfo } from '@fortawesome/free-solid-svg-icons';
import { ProductItem } from 'src/app/models/product-item';
import { MatDialog }  from '@angular/material/dialog';
import { ModalCrudComponent } from '../../components/modal-crud/modal-crud.component';
import { ModalCrudRateComponent } from '../../components/modal-crud-rate/modal-crud-rate.component';
import { ProductPriceItem } from 'src/app/models/product-price-item';
import { ProgramItem } from 'src/app/models/program-item';
import { RateItem } from 'src/app/models/rate-item';
import { DialogComponent } from 'src/app/components/dialog/dialog.component';
import { DialogDataItem } from 'src/app/models/dialog-data-item';
import { GlobalAutomotiveService } from 'src/app/services/global-automotive.service';
import { CommonService } from 'src/app/services/common.service';
import { CrudHistory } from 'src/app/models/crud-history-item';
import { Router } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';

interface programFormItem {
  Id: FormControl<number>,
  Code: FormControl<string>,
  Description: FormControl<string>,
  CertificateIdentifier: FormControl<string>,
  SalesChannel: FormControl<number>,
  IdGroup: FormControl<number>,
  CrudHistoryList: FormArray<FormGroup<any>>,
  Active: FormControl<boolean>
}

export interface ProgramPercentagesFormItem {
  Id: FormControl<number>,
  PrimaRiesgo: FormControl<number>,
  AdminFee: FormControl<number>,
  AsistenciaVial: FormControl<number>,
  CommAnzenLower: FormControl<number>,
  CommAnzenUpper: FormControl<number>,
  CommAnzenPercentage: FormControl<number>,
  CommAnzenPrice: FormControl<number>,
  MktLower: FormControl<number>,
  MktUpper: FormControl<number>,
  MktPercentage: FormControl<number>,
  MktPrice: FormControl<number>,
  RewardLower: FormControl<number>,
  RewardUpper: FormControl<number>,
  RewardPercentage: FormControl<number>,
  RewardPrice: FormControl<number>,
  CommNRLower: FormControl<number>,
  CommNRUpper: FormControl<number>,
  CommNRPercentage: FormControl<number>,
  CommNRPrice: FormControl<number>,
  CommPlantaLower: FormControl<number>,
  CommPlantaUpper: FormControl<number>,
  CommPlantaPercentage: FormControl<number>,
  CommPlantaPrice: FormControl<number>,
  CommDealerLower: FormControl<number>,
  CommDealerUpper: FormControl<number>,
  CommDealerPercentage: FormControl<number>,
  CommDealerPrice: FormControl<number>,
  PWIVA: FormControl<number>,
  PWOIVA: FormControl<number>,
  IdProgram: FormControl<number>,
  IdProduct: FormControl<number>,
  Class: FormControl<string>,
  Deductible: FormControl<number>,
  ManufacturerWarrantyKM: FormControl<number>,
  ManufacturerTerm: FormControl<number>,
  Term: FormControl<number>,
  MaxKM: FormControl<number>,
  PlanCodeEW: FormControl<number>,
  PlanCodeRA: FormControl<number>,
  Recuperacion: FormControl<number>,
  APV: FormControl<number>,
  Adicional1: FormControl<number>,
  Adicional2: FormControl<number>,
  PrimaNeta: FormControl<number>,
  PriceRA: FormControl<number>,
  PriceEW: FormControl<number>,
  Profit: FormControl<number>
}

@Component({
  selector: 'app-program-page',
  templateUrl: './program-page.component.html',
  styleUrls: ['./program-page.component.scss']
})
export class ProgramPageComponent implements OnInit {

  iconCircleInfo = faCircleInfo;

  products: Array<ProductItem> = [];

  programGroup: FormGroup<programFormItem> = new FormGroup<programFormItem>(
    {
      Id: new FormControl<number>(0, {nonNullable: true}),
      Code: new FormControl<string>('', {nonNullable: true, validators: [Validators.required]}),
      Description: new FormControl<string>('', {nonNullable: true, validators: [Validators.required]}),
      CertificateIdentifier: new FormControl<string>('', {nonNullable: true, validators: [Validators.required]}),
      SalesChannel: new FormControl<number>(0, {nonNullable: true}),
      IdGroup: new FormControl<number>(0, {nonNullable: true}),
      CrudHistoryList: new FormArray<FormGroup<any>>([]),
      Active: new FormControl<boolean>(true, {nonNullable: true})
    }
  )

  programPercentagesForm: FormGroup<ProgramPercentagesFormItem> = new FormGroup<ProgramPercentagesFormItem>(
    {
      Id: new FormControl<number>(0, {nonNullable: true}),
      PrimaRiesgo: new FormControl<number>(0, {nonNullable: true}),
      AdminFee: new FormControl<number>(0, {nonNullable: true}),
      AsistenciaVial: new FormControl<number>(0, {nonNullable: true}),
      CommAnzenLower: new FormControl<number>(0, {nonNullable: true, validators: [Validators.required, Validators.min(0)]}),
      CommAnzenUpper: new FormControl<number>(0, {nonNullable: true, validators: [Validators.required, Validators.max(100)]}),
      CommAnzenPercentage: new FormControl<number>(0, {nonNullable: true}),
      CommAnzenPrice: new FormControl<number>(0, {nonNullable: true}),
      MktLower: new FormControl<number>(0, {nonNullable: true, validators: [Validators.required, Validators.min(0)]}),
      MktUpper: new FormControl<number>(0, {nonNullable: true, validators: [Validators.required, Validators.max(100)]}),
      MktPercentage: new FormControl<number>(0, {nonNullable: true}),
      MktPrice: new FormControl<number>(0, {nonNullable: true}),
      RewardLower: new FormControl<number>(0, {nonNullable: true, validators: [Validators.required, Validators.min(0)]}),
      RewardUpper: new FormControl<number>(0, {nonNullable: true, validators: [Validators.required, Validators.max(100)]}),
      RewardPercentage: new FormControl<number>(0, {nonNullable: true}),
      RewardPrice: new FormControl<number>(0, {nonNullable: true}),
      CommNRLower: new FormControl<number>(0, {nonNullable: true, validators: [Validators.required, Validators.min(0)]}),
      CommNRUpper: new FormControl<number>(0, {nonNullable: true, validators: [Validators.required, Validators.max(100)]}),
      CommNRPercentage: new FormControl<number>(0, {nonNullable: true}),
      CommNRPrice: new FormControl<number>(0, {nonNullable: true}),
      CommPlantaLower: new FormControl<number>(0, {nonNullable: true, validators: [Validators.required, Validators.min(0)]}),
      CommPlantaUpper: new FormControl<number>(0, {nonNullable: true, validators: [Validators.required, Validators.max(100)]}),
      CommPlantaPercentage: new FormControl<number>(0, {nonNullable: true}),
      CommPlantaPrice: new FormControl<number>(0, {nonNullable: true}),
      CommDealerLower: new FormControl<number>(0, {nonNullable: true, validators: [Validators.required, Validators.min(0)]}),
      CommDealerUpper: new FormControl<number>(0, {nonNullable: true, validators: [Validators.required, Validators.max(100)]}),
      CommDealerPercentage: new FormControl<number>(0, {nonNullable: true}),
      CommDealerPrice: new FormControl<number>(0, {nonNullable: true}),
      PWIVA: new FormControl<number>(0, {nonNullable: true}),
      PWOIVA: new FormControl<number>(0, {nonNullable: true}),
      IdProgram: new FormControl<number>(0, {nonNullable: true}),
      IdProduct: new FormControl<number>(0, {nonNullable: true}),
      Class: new FormControl<string>('', {nonNullable: true}),
      Deductible: new FormControl<number>(0, {nonNullable: true}),
      ManufacturerWarrantyKM: new FormControl<number>(0, {nonNullable: true}),
      ManufacturerTerm: new FormControl<number>(0, {nonNullable: true}),
      Term: new FormControl<number>(0, {nonNullable: true}),
      MaxKM: new FormControl<number>(0, {nonNullable: true}),
      PlanCodeEW: new FormControl<number>(0, {nonNullable: true}),
      PlanCodeRA: new FormControl<number>(0, {nonNullable: true}),
      Recuperacion: new FormControl<number>(0, {nonNullable: true}),
      APV: new FormControl<number>(0, {nonNullable: true}),
      Adicional1: new FormControl<number>(0, {nonNullable: true}),
      Adicional2: new FormControl<number>(0, {nonNullable: true}),
      PrimaNeta: new FormControl<number>(0, {nonNullable: true}),
      PriceEW: new FormControl<number>(0, {nonNullable: true}),
      PriceRA: new FormControl<number>(0, {nonNullable: true}),
      Profit: new FormControl<number>(0, {nonNullable: true})
    }
  )

  constructor(
    private dialog: MatDialog,
    private globalAutoService: GlobalAutomotiveService,
    private commonService: CommonService,
    private router: Router
  ){}

  ngOnInit(): void {
    
  }

  get programPercentagesControls() { return this.programPercentagesForm.controls }
  get programFormControls()  { return this.programGroup.controls }


  openModalAddProduct(): void {
    this.dialog.open<ModalCrudComponent, ProductItem, ProductItem | null | undefined>(ModalCrudComponent,
      {
        data: {
          Id: 0,
          Description: '',
          WebsiteDescription: '',
          IdProductType: 0,
          BrandCode: '',
          PeriodLowerLimit: 0,
          PeriodUpperLimit: 0,
          KilometerLowerLimit: 0,
          KilometerUpperLimit: 0,
          IdProgram: this.programGroup.controls.Id.value,
          Services: true,
          TypeWarranty: '',
          CrudHistoryList: [],
          Template: '',
          Active: true,
          RateForms: [],
          ListPrices: []
        },
        height: '50%',
        width: '80%',
        disableClose: true
      }
    ).afterClosed().subscribe(result => {
      if(typeof result !== 'undefined') {
        if (result !== null) {
          this.products.push(result);
        }
      }
    })
  }

  openModalAddRate(indexProduct: number, productPrice?: ProductPriceItem, indexPriceItem?: number): void {

    const data: ProductPriceItem = (typeof productPrice !== 'undefined') ? productPrice : {
      Id: 0,
      PrimaRiesgo: 0,
      AdminFee: 0,
      AsistenciaVial: 0,
      CommAnzenLower: this.programPercentagesControls.CommAnzenLower.value,
      CommAnzenUpper: this.programPercentagesControls.CommAnzenUpper.value,
      CommAnzenPercentage: 0,
      CommAnzenPrice: 0,
      MktLower: this.programPercentagesControls.MktLower.value,
      MktUpper: this.programPercentagesControls.MktUpper.value,
      MktPercentage: 0,
      MktPrice: 0,
      RewardLower: this.programPercentagesControls.RewardLower.value,
      RewardUpper: this.programPercentagesControls.RewardUpper.value,
      RewardPercentage: 0,
      RewardPrice: 0,
      CommNRLower: this.programPercentagesControls.CommNRLower.value,
      CommNRUpper: this.programPercentagesControls.CommNRUpper.value,
      CommNRPercentage: 0,
      CommNRPrice: 0,
      CommPlantaLower: this.programPercentagesControls.CommPlantaLower.value,
      CommPlantaUpper: this.programPercentagesControls.CommPlantaUpper.value,
      CommPlantaPercentage: 0,
      CommPlantaPrice: 0,
      CommDealerLower: this.programPercentagesControls.CommDealerLower.value,
      CommDealerUpper: this.programPercentagesControls.CommDealerUpper.value,
      CommDealerPercentage: 0,
      CommDealerPrice: 0,
      PWIVA: 0,
      PWOIVA: 0,
      IdProgram: 0,
      IdProduct: 0,
      Class: '',
      Deductible: 0,
      ManufacturerWarrantyKM: 0,
      ManufacturerTerm: 0,
      Term: 0,
      MaxKM: 0,
      PlanCodeEW: 0,
      PlanCodeRA: 0,
      Recuperacion: 0,
      APV: 0,
      Adicional1: 0,
      Adicional2: 0,
      PrimaNeta: 0,
      PriceRA: 0,
      PriceEW: 0,
      Profit: 0
    }

    this.dialog.open<ModalCrudRateComponent, ProductPriceItem, ProductPriceItem | null>(ModalCrudRateComponent,
      {
        data: data,
        height: '80%',
        width: '80%',
        disableClose: true
      }
    ).afterClosed().subscribe(result => {
      if (typeof result !== 'undefined') {
        if (result !== null) {

          if (typeof productPrice !== 'undefined' && typeof indexPriceItem !== 'undefined') {
            this.products[indexProduct].RateForms[indexPriceItem] = result;
          } else {
            this.products[indexProduct].RateForms.push(result);
          }
        }
      }
    })
  }



  saveProgram(): void {

    if (this.programGroup.valid && this.products.length > 0) {

      this.dialog.open<DialogComponent, DialogDataItem, boolean>(DialogComponent,
        {
          data: { Title: 'Advertencia', Content: '¿Esta seguro de continuar?', Type: 'question' },
          height: '200px',
          width: '300px',
          disableClose: true
        }
      ).afterClosed().subscribe(result => {
        if (typeof result === 'boolean') {
          if (result) {
            const currentTime: Date = new Date();
            let programRequest: ProgramItem = {
              Id: this.programFormControls.Id.value,
              Code: this.programFormControls.Code.value,
              Description: this.programFormControls.Description.value,
              CertificateIdentifier: this.programFormControls.CertificateIdentifier.value,
              SalesChannel: this.programFormControls.SalesChannel.value,
              IdGroup: this.programFormControls.IdGroup.value,
              CrudHistoryList: this.StringCrudHistory(currentTime),
              Active: this.programFormControls.Active.value,
              ListProduct: this.products.map((prod, indice) => {
        
                return {
                  Id: prod.Id,
                  Description: prod.Description,
                  WebsiteDescription: prod.Description + ' ' + prod.KilometerLowerLimit + ' - ' + prod.KilometerUpperLimit + ' KM',
                  IdProductType: 1,
                  BrandCode: prod.BrandCode,
                  PeriodLowerLimit: Number(prod.PeriodLowerLimit),
                  PeriodUpperLimit: Number(prod.PeriodUpperLimit),
                  KilometerLowerLimit: Number(prod.KilometerLowerLimit),
                  KilometerUpperLimit: Number(prod.KilometerUpperLimit),
                  IdProgram: 0,
                  Services: prod.Services,
                  TypeWarranty: prod.TypeWarranty,
                  CrudHistoryList: this.StringCrudHistory(currentTime),
                  Template: prod.Template,
                  Active: prod.Active,
                  RateForms: [],
                  ListPrices: prod.RateForms.map((prodPrice, ind) => {
        
                    const rateItem: RateItem = {
                      Id: 0,
                      Term: prodPrice.Term,
                      IdProduct: 0,
                      IdClassType: 1,
                      ClassCode: prodPrice.Class,
                      MaxKm: Number(prodPrice.MaxKM),
                      Deductible: Number(prodPrice.Deductible),
                      PriceCert: prodPrice.PWIVA,
                      PriceEW: prodPrice.PriceEW,
                      PriceWP: prodPrice.PWOIVA,
                      MKT: prodPrice.MktPrice,
                      PlanCodeEW: Number(prodPrice.PlanCodeEW),
                      PriceRA: prodPrice.PriceRA,
                      PlanCodeRA: Number(prodPrice.PlanCodeRA),
                      Prime: 0,
                      AgencyComission: 0,
                      VendorComission: 0,
                      DealerComission: 0,
                      AdminFee: Number(prodPrice.AdminFee),
                      InsurancePolicy: 0,
                      CommAnzen: prodPrice.CommAnzenPrice,
                      Rewards: prodPrice.RewardPrice,
                      CommNR: prodPrice.CommNRPrice,
                      CommPlant: prodPrice.CommPlantaPrice,
                      RiskPremium: Number(prodPrice.PrimaRiesgo),
                      Profit: prodPrice.Profit,
                      IdProgram: 0,
                      Taxes_Percent: 16,
                      CrudHistoryList: this.StringCrudHistory(currentTime),
                      ManufacturerTerm: prodPrice.ManufacturerTerm,
                      ManufacturerWarrantyKM: Number(prodPrice.ManufacturerWarrantyKM),
                      Recuperacion: prodPrice.Recuperacion,
                      APV: prodPrice.APV,
                      Adicional1: prodPrice.Adicional1,
                      Adicional2: prodPrice.Adicional2,
                      PrimaNeta: prodPrice.PrimaNeta,
                      Active: true
                    }
        
                    return rateItem;
                  })
                }
        
              })
            }
            this.dialog.open<DialogComponent, DialogDataItem, boolean>(DialogComponent,
              {
                data: { Title: 'Guardando', Content: 'Por favor espere', Type: 'loading' },
                height: '200px',
                width: '200px',
                disableClose: true
              }
            );
            this.globalAutoService.InsertCotizacion(programRequest).subscribe(
              {
                next: (response: ProgramItem) => {
                  this.dialog.closeAll();
                  this.dialog.open<DialogComponent, DialogDataItem, boolean | null>(DialogComponent, 
                    {
                      data: { Title: 'Exito', Content: 'Se ha guardado el programa ' + response.Description + ' correctamente', Type: 'message' },
                      height: '200px',
                      width: '300px',
                    }
                  ).afterClosed().subscribe(res => {
                    this.router.navigate(['/', 'home', 'admon-rates'], { queryParams: { pr: response.Id } })
                  })
                },
                error: (err: HttpErrorResponse) => {
                  this.dialog.closeAll();
                  this.dialog.open<DialogComponent, DialogDataItem, boolean | null>(DialogComponent, 
                    {
                      data: { Title: 'Error', Content: err.error.Message + ', por favor vuelva a intentar mas tarde' || 'Ocurrio un error inesperado, por favor vuelva a intentar mas tarde', Type: 'message' },
                      height: '200px',
                      width: '300px',
                    }
                  )
                }
              }
            )
          }
        }
      })
    } else {

      if (!this.programGroup.valid) {
        this.programGroup.markAllAsTouched();
      } else if (this.products.length === 0) {
        this.dialog.open<DialogComponent, DialogDataItem, boolean | null>(DialogComponent, 
          {
            data: { Title: 'Advertencia', Content: 'Agregue al menos un producto', Type: 'message' },
            height: '200px',
            width: '300px',
          }
        )
      }
    } 
  }


  StringCrudHistory = (currentTime: Date): string => {
    const splittedDate = currentTime.toJSON().split('T');
    const ShortDate = splittedDate[0];
    const time = splittedDate[1].substring(0, splittedDate[1].indexOf('Z'));
    const creationDate = `${ShortDate} ${time}`;

    let crudHistoryItem: Array<CrudHistory> = [
      { Date: creationDate, Comments: 'Creación inicial', Type: 'C', IdUser: this.commonService.userData.Id || 0  }
    ]
    

    return JSON.stringify(crudHistoryItem);
  }

}
