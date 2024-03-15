import { Component, Inject, OnInit } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, ValidationErrors, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { ProductPriceItem } from 'src/app/models/product-price-item';
import { ProgramPercentagesFormItem } from '../../pages/program-page/program-page.component';
import { GlobalAutomotiveService } from 'src/app/services/global-automotive.service';
import { ClassDataItem, ClassItem } from 'src/app/models/class-item';
import { HttpErrorResponse } from '@angular/common/http';
import { SelectOptionItem } from 'src/app/models/select-option-item';
import { CommonService } from 'src/app/services/common.service';
import { DialogComponent } from 'src/app/components/dialog/dialog.component';
import { DialogDataItem } from 'src/app/models/dialog-data-item';


interface elitaServicesFormItem {
  PrimaRiesgoES: FormControl<number>,
  PrimaRiesgoPercentageES: FormControl<number>,
  AdminES: FormControl<number>,
  AdminPercentageES: FormControl<number>,
  MktES: FormControl<number>,
  MktPercentageES: FormControl<number>,
  CommES: FormControl<number>,
  CommPercentageES: FormControl<number>,
  ProfitES: FormControl<number>,
  ProfitPercentageES: FormControl<number>,
  TotalES: FormControl<number>,
  TotalPercentageES: FormControl<number>
}

interface elitaDamagesFormItem {
  PrimaRiesgoED:  FormControl<number>
  PrimaRiesgoPercentageED:  FormControl<number>
  AdminED:  FormControl<number>
  AdminPercentageED:  FormControl<number>
  MktED:  FormControl<number>
  MktPercentageED:  FormControl<number>
  CommED:  FormControl<number>
  CommPercentageED:  FormControl<number>
  ProfitED:  FormControl<number>
  ProfitPercentageED:  FormControl<number>
  TotalED:  FormControl<number>
  TotalPercentageED:  FormControl<number>
}

@Component({
  selector: 'app-modal-crud-rate',
  templateUrl: './modal-crud-rate.component.html',
  styleUrls: ['./modal-crud-rate.component.scss']
})
export class ModalCrudRateComponent implements OnInit {


  productRateForm!: FormGroup<ProgramPercentagesFormItem>;
  classList: Array<SelectOptionItem> = [];
  warrantyDurationList: Array<SelectOptionItem> = [
    { text: '12 meses', value: 12 },
    { text: '24 meses', value: 24 },
    { text: '36 meses', value: 36 },
    { text: '48 meses', value: 48 },
    { text: '60 meses', value: 60 }
  ]

  isUpperto100: boolean = false;
  
  elitaServicesForm: FormGroup<elitaServicesFormItem> = new FormGroup<elitaServicesFormItem>(
    {
      PrimaRiesgoES: new FormControl<number>(0, {nonNullable: true}),
      PrimaRiesgoPercentageES: new FormControl<number>(0, {nonNullable: true}),
      AdminES: new FormControl<number>(0, {nonNullable: true}),
      AdminPercentageES: new FormControl<number>(0, {nonNullable: true}),
      MktES: new FormControl<number>(0, {nonNullable: true}),
      MktPercentageES: new FormControl<number>(0, {nonNullable: true}),
      CommES: new FormControl<number>(0, {nonNullable: true}),
      CommPercentageES: new FormControl<number>(0, {nonNullable: true}),
      ProfitES: new FormControl<number>(0, {nonNullable: true, validators: [Validators.required, Validators.pattern(/^\d{0,}(.{1}\d{1,})?$/)]}),
      ProfitPercentageES: new FormControl<number>(0, {nonNullable: true}),
      TotalES: new FormControl<number>(0, {nonNullable: true}),
      TotalPercentageES: new FormControl<number>(0, {nonNullable: true})
    }
  )

  elitaDamagesForm: FormGroup<elitaDamagesFormItem> = new FormGroup<elitaDamagesFormItem>(
    {
      PrimaRiesgoED: new FormControl<number>(0, {nonNullable: true}),
      PrimaRiesgoPercentageED: new FormControl<number>(0, {nonNullable: true}),
      AdminED: new FormControl<number>(0, {nonNullable: true}),
      AdminPercentageED: new FormControl<number>(0, {nonNullable: true}),
      MktED: new FormControl<number>(0, {nonNullable: true}),
      MktPercentageED: new FormControl<number>(0, {nonNullable: true}),
      CommED: new FormControl<number>(0, {nonNullable: true}),
      CommPercentageED: new FormControl<number>(0, {nonNullable: true}),
      ProfitED: new FormControl<number>(0, {nonNullable: true}),
      ProfitPercentageED: new FormControl<number>(0, {nonNullable: true}),
      TotalED: new FormControl<number>(0, {nonNullable: true}),
      TotalPercentageED: new FormControl<number>(0, {nonNullable: true})
    }
  )


  constructor(
    private dialogRef: MatDialogRef<ModalCrudRateComponent>,
    @Inject(MAT_DIALOG_DATA) public data: ProductPriceItem,
    private globalAutoService: GlobalAutomotiveService,
    public commonService: CommonService,
    private dialog: MatDialog 
  ){
    this.productRateForm = new FormGroup<ProgramPercentagesFormItem>(
      {
        Id: new FormControl<number>(this.data.Id, {nonNullable: true}),
        PrimaRiesgo: new FormControl<number>(this.data.PrimaRiesgo, {nonNullable: true, validators: [Validators.required, Validators.pattern(/^\d{0,}(.{1}\d{1,})?$/) ]}),
        AdminFee: new FormControl<number>(this.data.AdminFee, {nonNullable: true, validators: [Validators.required, Validators.pattern(/^\d{0,}(.{1}\d{1,})?$/)]}),
        AsistenciaVial: new FormControl<number>(this.data.AsistenciaVial, {nonNullable: true, validators: [Validators.required, Validators.pattern(/^\d{0,}(.{1}\d{1,})?$/)]}),
        CommAnzenLower: new FormControl<number>(this.data.CommAnzenLower, {nonNullable: true}),
        CommAnzenUpper: new FormControl<number>(this.data.CommAnzenUpper, {nonNullable: true}),
        CommAnzenPercentage: new FormControl<number>(this.data.CommAnzenPercentage, {nonNullable: true, validators: [Validators.required, Validators.min(this.data.CommAnzenLower), Validators.max(this.data.CommAnzenUpper)]}),
        CommAnzenPrice: new FormControl<number>(this.data.CommAnzenPrice, {nonNullable: true}),
        MktLower: new FormControl<number>(this.data.MktLower, {nonNullable: true}),
        MktUpper: new FormControl<number>(this.data.MktUpper, {nonNullable: true}),
        MktPercentage: new FormControl<number>(this.data.MktPercentage, {nonNullable: true, validators: [Validators.required, Validators.min(this.data.MktLower), Validators.max(this.data.MktUpper)]}),
        MktPrice: new FormControl<number>(this.data.MktPrice, {nonNullable: true}),
        RewardLower: new FormControl<number>(this.data.RewardLower, {nonNullable: true}),
        RewardUpper: new FormControl<number>(this.data.RewardUpper, {nonNullable: true}),
        RewardPercentage: new FormControl<number>(this.data.RewardPercentage, {nonNullable: true, validators: [Validators.required, Validators.min(this.data.RewardLower), Validators.max(this.data.RewardUpper)]}),
        RewardPrice: new FormControl<number>(this.data.RewardPrice, {nonNullable: true}),
        CommNRLower: new FormControl<number>(this.data.CommNRLower, {nonNullable: true}),
        CommNRUpper: new FormControl<number>(this.data.CommNRUpper, {nonNullable: true}),
        CommNRPercentage: new FormControl<number>(this.data.CommNRPercentage, {nonNullable: true, validators: [Validators.required, Validators.min(this.data.CommNRLower), Validators.max(this.data.CommNRUpper)]}),
        CommNRPrice: new FormControl<number>(this.data.CommNRPrice, {nonNullable: true}),
        CommPlantaLower: new FormControl<number>(this.data.CommPlantaLower, {nonNullable: true}),
        CommPlantaUpper: new FormControl<number>(this.data.CommPlantaUpper, {nonNullable: true}),
        CommPlantaPercentage: new FormControl<number>(this.data.CommPlantaPercentage, {nonNullable: true, validators: [Validators.required, Validators.min(this.data.CommPlantaLower), Validators.max(this.data.CommPlantaUpper)]}),
        CommPlantaPrice: new FormControl<number>(this.data.CommPlantaPrice, {nonNullable: true}),
        CommDealerLower: new FormControl<number>(this.data.CommDealerLower, {nonNullable: true}),
        CommDealerUpper: new FormControl<number>(this.data.CommDealerUpper, {nonNullable: true}),
        CommDealerPercentage: new FormControl<number>(this.data.CommDealerPercentage, {nonNullable: true, validators: [Validators.required, Validators.min(this.data.CommDealerLower), Validators.max(this.data.CommDealerUpper)]}),
        CommDealerPrice: new FormControl<number>(this.data.CommDealerPrice, {nonNullable: true}),
        PWIVA: new FormControl<number>(this.data.PWIVA, {nonNullable: true}),
        PWOIVA: new FormControl<number>(this.data.PWOIVA, {nonNullable: true}),
        IdProgram: new FormControl<number>(this.data.IdProgram, {nonNullable: true}),
        IdProduct: new FormControl<number>(this.data.IdProduct, {nonNullable: true}),
        Class: new FormControl<string>(this.data.Class, {nonNullable: true, validators: [Validators.required]}),
        Deductible: new FormControl<number>(this.data.Deductible, {nonNullable: true, validators: [Validators.required]}),
        ManufacturerWarrantyKM: new FormControl<number>(this.data.ManufacturerWarrantyKM, {nonNullable: true, validators: [Validators.required, Validators.pattern(/^\d{0,}(.{1}\d{1,})?$/) ]}),
        ManufacturerTerm: new FormControl<number>(this.data.ManufacturerTerm, {nonNullable: true, validators:[this.isNotSelected] }),
        Term: new FormControl<number>(this.data.Term, {nonNullable: true, validators: [this.isNotSelected]}),
        MaxKM: new FormControl<number>(this.data.MaxKM, {nonNullable: true, validators: [Validators.required, Validators.pattern(/^\d{0,}(.{1}\d{1,})?$/) ]}),
        PlanCodeEW: new FormControl<number>(this.data.PlanCodeEW, {nonNullable: true, validators: [Validators.required]}),
        PlanCodeRA: new FormControl<number>(this.data.PlanCodeRA, {nonNullable: true, validators: [Validators.required]}),
        Recuperacion: new FormControl<number>(this.data.Recuperacion, {nonNullable: true, validators: [Validators.required, Validators.pattern(/^\d{0,}(.{1}\d{1,})?$/)]}),
        APV: new FormControl<number>(this.data.APV, {nonNullable: true, validators: [Validators.required, Validators.pattern(/^\d{0,}(.{1}\d{1,})?$/)]}),
        Adicional1: new FormControl<number>(this.data.Adicional1, {nonNullable: true, validators: [Validators.required, Validators.pattern(/^\d{0,}(.{1}\d{1,})?$/)]}),
        Adicional2: new FormControl<number>(this.data.Adicional2, {nonNullable: true, validators: [Validators.required, Validators.pattern(/^\d{0,}(.{1}\d{1,})?$/)]}),
        PrimaNeta: new FormControl<number>(this.data.PrimaNeta, {nonNullable: true}),
        PriceEW: new FormControl<number>(this.data.PriceEW, {nonNullable: true}),
        PriceRA: new FormControl<number>(this.data.PriceRA, {nonNullable: true}),
        Profit: new FormControl<number>(this.data.Profit, {nonNullable: true}),
      }
    )
  }

  get formControls() { return this.productRateForm.controls; }
  get formControlsES() { return this.elitaServicesForm.controls; }
  get formControlsED() { return this.elitaDamagesForm.controls; }


  ngOnInit(): void {
    this.calculateRateValues();
    this.globalAutoService.getClasses().subscribe(
      {
        next: (response) => {
          const classes = JSON.parse(response.ClassData) as ClassDataItem;
          for (const key in classes) {
            classes[key].forEach((item, index) => {
              for (const k in item) {
                this.classList.push( { text: key, value: item[k] } )
              }
            })
          }
        }, 
        error: (err: HttpErrorResponse) => {
          this.dialogRef.close(null);
        }
      }
    )
  }


  closeDialog(action: string): void {
    if (action === 'save') {
      if (this.productRateForm.valid && this.elitaServicesForm.valid && !this.isUpperto100) {
        this.formControls.PriceRA.setValue(this.formControlsED.TotalED.value);
        this.formControls.PriceEW.setValue(this.formControlsES.TotalES.value);
        this.formControls.Profit.setValue(this.formControlsED.ProfitED.value);
        this.productRateForm.updateValueAndValidity({onlySelf: false});
        this.data = this.productRateForm.getRawValue();
        this.dialogRef.close(this.data);
      } else {

        if (!this.productRateForm.valid) {
          this.productRateForm.markAllAsTouched();
        }

        if (!this.elitaServicesForm.valid) {
          this.elitaServicesForm.markAllAsTouched();
        }

        if (this.isUpperto100) {
          this.dialog.open<DialogComponent, DialogDataItem, boolean | null>(DialogComponent, 
            {
              data: { Title: 'Advertencia', Content: 'Los porcentajes exceden el 100%', Type: 'message' },
              width: '300px',
              height: '200px'
            }
          )
        }
      }
    } else {
      this.dialogRef.close(null);
    } 
  }


  isValidForCalculate(): boolean {
    let isValid: boolean = true;
    
    if (!this.formControls.PrimaRiesgo.valid) {
      isValid = false;
      this.formControls.PrimaRiesgo.markAsTouched({onlySelf: true});
    }

    if (!this.formControls.AdminFee.valid) {
      isValid = false;
      this.formControls.AdminFee.markAsTouched({onlySelf: true});
    }

    if (!this.formControls.AsistenciaVial.valid) {
      isValid = false;
      this.formControls.AsistenciaVial.markAsTouched({onlySelf: true});
    }

    if (!this.formControls.Recuperacion.valid) {
      isValid = false;
      this.formControls.Recuperacion.markAsTouched({onlySelf: true});
    }

    if (!this.formControls.APV.valid) {
      isValid = false;
      this.formControls.APV.markAsTouched({onlySelf: true});
    }

    if (!this.formControls.Adicional1.valid) {
      isValid = false;
      this.formControls.Adicional1.markAsTouched({onlySelf: true});
    }

    if (!this.formControls.Adicional2.valid) {
      isValid = false;
      this.formControls.Adicional2.markAsTouched({onlySelf: true});
    }
    
    if (!this.formControlsES.ProfitES.valid) {
      isValid = false;
      this.formControlsES.ProfitES.markAsTouched({onlySelf: true});
    }

    return isValid;
  }

  calculateRateValues(): void {
    if (this.isValidForCalculate()) {

      this.formControlsES.PrimaRiesgoES.setValue(this.formControls.PrimaRiesgo.value);

      const sumData:number = Number(this. formControls.PrimaRiesgo.getRawValue()) + Number(this.formControls.AdminFee.getRawValue()) + Number(this.formControls.AsistenciaVial.getRawValue());
      const sumPercentages: number = (
        (
        Number(this.formControls.CommAnzenPercentage.getRawValue()) + 
        Number(this.formControls.MktPercentage.getRawValue()) + 
        Number(this.formControls.RewardPercentage.getRawValue()) + 
        Number(this.formControls.CommNRPercentage.getRawValue()) + 
        Number(this.formControls.CommPlantaPercentage.getRawValue()) + 
        Number(this.formControls.CommDealerPercentage.getRawValue()) +
        Number(this.formControls.Recuperacion.getRawValue()) +
        Number(this.formControls.APV.getRawValue()) +
        Number(this.formControls.Adicional1.getRawValue()) +
        Number(this.formControls.Adicional2.getRawValue())
        ) * 0.01);

      if (sumPercentages < 1) {
        this.isUpperto100 = false;
        const priceWOIVA: number = (sumData / (1 - sumPercentages));
        this.formControls.PWOIVA.setValue(Number(priceWOIVA.toFixed(2)));

        const commAnzen: number = ((Number(this.formControls.CommAnzenPercentage.getRawValue()) * 0.01) * Number(this.formControls.PWOIVA.getRawValue()));
        const marketing: number = ((Number(this.formControls.MktPercentage.getRawValue()) * 0.01) * Number(this.formControls.PWOIVA.getRawValue()));
        const rewards: number = ((Number(this.formControls.RewardPercentage.getRawValue()) * 0.01) * Number(this.formControls.PWOIVA.getRawValue()));
        const commNR: number = ((Number(this.formControls.CommNRPercentage.getRawValue()) * 0.01) * Number(this.formControls.PWOIVA.getRawValue()));
        const commPlanta: number = ((Number(this.formControls.CommPlantaPercentage.getRawValue()) * 0.01) * Number(this.formControls.PWOIVA.getRawValue()));
        const commDealer: number = ((Number(this.formControls.CommDealerPercentage.getRawValue()) * 0.01) * Number(this.formControls.PWOIVA.getRawValue()));

        this.formControls.CommAnzenPrice.setValue(Number(commAnzen.toFixed(2)));
        this.formControls.MktPrice.setValue(Number(marketing.toFixed(2)));
        this.formControls.RewardPrice.setValue(Number(rewards.toFixed(2)));
        this.formControls.CommNRPrice.setValue(Number(commNR.toFixed(2)));
        this.formControls.CommPlantaPrice.setValue(Number(commPlanta.toFixed(2)));
        this.formControls.CommDealerPrice.setValue(Number(commDealer.toFixed(2)));

        this.formControls.PWIVA.setValue(Number((Number(this.formControls.PWOIVA.getRawValue()) * 1.16).toFixed(2)));
        const primaNeta: number = Number(this.formControls.PrimaRiesgo.value) + Number(this.formControls.AdminFee.value) + marketing;
        this.formControls.PrimaNeta.setValue(Number(primaNeta.toFixed(2)))

        this.formControlsES.MktES.setValue(Number(Number(this.formControls.MktPrice.value).toFixed(2)));
        this.formControlsES.AdminES.setValue(Number((Number(this.formControls.AdminFee.value) - Number(this.formControlsES.ProfitES.value)).toFixed(2)));

        const commES: number = commAnzen + rewards + commNR + commPlanta + commDealer;
        this.formControlsES.CommES.setValue(Number(commES.toFixed(2)));

        const totalElitaServices: number = (
          Number(this.formControlsES.PrimaRiesgoES.value) + 
          Number(this.formControlsES.AdminES.value) + 
          Number(this.formControlsES.MktES.value) + 
          commES + Number(this.formControlsES.ProfitES.value)
        );

        this.formControlsES.TotalES.setValue(Number(totalElitaServices.toFixed(2)));

        let primaRiesgoESPer: number = ((Number(this.formControlsES.PrimaRiesgoES.value) / totalElitaServices) * 100);
        let adminESPer: number = ((Number(this.formControlsES.AdminES.value) / totalElitaServices) * 100);
        let mktESPer: number = ((Number(this.formControlsES.MktES.value) / totalElitaServices) * 100);
        let commEsPer: number = ((Number(this.formControlsES.CommES.value) / totalElitaServices) * 100);
        let profitESPer: number = ((Number(this.formControlsES.ProfitES.value) / totalElitaServices) * 100);

        
        this.formControlsES.PrimaRiesgoPercentageES.setValue( (Number.isNaN(primaRiesgoESPer)) ? 0 : Number(primaRiesgoESPer.toFixed(0)));
        this.formControlsES.AdminPercentageES.setValue( (Number.isNaN(adminESPer)) ? 0 :  Number(adminESPer.toFixed(0)));
        this.formControlsES.MktPercentageES.setValue( (Number.isNaN(mktESPer)) ? 0 : Number(mktESPer.toFixed(0)));
        this.formControlsES.CommPercentageES.setValue( (Number.isNaN(commEsPer)) ? 0 : Number(commEsPer.toFixed(0)));
        this.formControlsES.ProfitPercentageES.setValue(  (Number.isNaN(profitESPer)) ? 0 : Number(profitESPer.toFixed(0)));

        const totalPercentageES: number =  primaRiesgoESPer + adminESPer + mktESPer + commEsPer + profitESPer;

        this.formControlsES.TotalPercentageES.setValue((Number.isNaN(totalPercentageES)) ? 0 : Number(totalPercentageES.toFixed(0)));

        const primaRiesgoED: number = (Number(this.formControlsES.PrimaRiesgoES.value) * 0.9);
        const adminED: number = (Number(this.formControlsES.PrimaRiesgoES.value) * 0.05);
        const profitED: number = (Number(this.formControlsES.PrimaRiesgoES.value) * 0.05);

        const totalElitaDamages: number = primaRiesgoED + adminED + profitED;

        const primaRiesgoEDPer: number = ((primaRiesgoED / totalElitaDamages) * 100);
        const adminEDPer: number = ((adminED / totalElitaDamages) * 100);
        const profitEDPer: number = ((profitED / totalElitaDamages) * 100);

        const totalPercentageED: number = primaRiesgoEDPer + adminEDPer + profitEDPer;

        this.formControlsED.PrimaRiesgoED.setValue(Number(primaRiesgoED.toFixed(2)));
        this.formControlsED.AdminED.setValue(Number(adminED.toFixed(2)));
        this.formControlsED.ProfitED.setValue(Number(profitED.toFixed(2)));

        this.formControlsED.PrimaRiesgoPercentageED.setValue( (Number.isNaN(primaRiesgoEDPer)) ? 0 :  Number(primaRiesgoEDPer.toFixed(0)));
        this.formControlsED.AdminPercentageED.setValue( (Number.isNaN(adminEDPer)) ? 0 : Number(adminEDPer.toFixed(0)));
        this.formControlsED.ProfitPercentageED.setValue( (Number.isNaN(profitEDPer)) ? 0 : Number(profitEDPer.toFixed(0)));

        this.formControlsED.TotalED.setValue(Number(totalElitaDamages.toFixed(2)));
        this.formControlsED.TotalPercentageED.setValue((Number.isNaN(totalPercentageED)) ? 0 :  Number(totalPercentageED.toFixed(0)));


        this.productRateForm.updateValueAndValidity({onlySelf: false});
        this.elitaServicesForm.updateValueAndValidity({onlySelf: false});
        this.elitaDamagesForm.updateValueAndValidity({onlySelf: false});

      } else {
        this.isUpperto100 = true;
        this.dialog.open<DialogComponent, DialogDataItem, boolean | null>(DialogComponent,
          {
            data: { Title: 'Advertencia', Content: 'Los porcentajes exceden el 100%', Type: 'message' },
            width: '300px',
            height: '200px'
          }
        )
      }
    }
  }

  isNotSelected(control: AbstractControl): ValidationErrors | null {
    if (Number(control.value) === 0) {
      return {
        "isNotSelected": true
      };
    }
    return null;
  }
}
