import { Component, Inject, OnInit } from '@angular/core';
import { AbstractControl, FormArray, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { CrudHistoryFormItem } from 'src/app/models/crud-history-item';
import { ProductItem } from 'src/app/models/product-item';
import { ProgramPercentagesFormItem } from '../../pages/program-page/program-page.component';

interface productFormItem {
  Id: FormControl<number>,
  Description: FormControl<string>,
  WebsiteDescription: FormControl<string>,
  IdProductType: FormControl<number>,
  BrandCode: FormControl<string>,
  PeriodLowerLimit: FormControl<number>,
  PeriodUpperLimit: FormControl<number>,
  KilometerLowerLimit: FormControl<number>,
  KilometerUpperLimit: FormControl<number>,
  IdProgram: FormControl<number>,
  Services: FormControl<boolean>,
  TypeWarranty: FormControl<string>,
  CrudHistoryList: FormArray<FormGroup<CrudHistoryFormItem>>,
  Template: FormControl<string>,
  Active: FormControl<boolean>,
  RateForms: FormArray<FormGroup<ProgramPercentagesFormItem>>
}

@Component({
  selector: 'app-modal-crud',
  templateUrl: './modal-crud.component.html',
  styleUrls: ['./modal-crud.component.scss']
})
export class ModalCrudComponent implements OnInit {


  productForm: FormGroup<productFormItem> = new FormGroup<productFormItem>(
    {
      Id: new FormControl<number>(0, {nonNullable: true}),
      Description: new FormControl<string>('', {nonNullable: true, validators: [Validators.required]}),
      WebsiteDescription: new FormControl<string>('', {nonNullable: true}),
      IdProductType: new FormControl<number>(0, {nonNullable: true}),
      BrandCode: new FormControl<string>('', {nonNullable: true}),
      PeriodLowerLimit: new FormControl<number>({ value: 0, disabled: false }, {nonNullable: true}),
      PeriodUpperLimit: new FormControl<number>(0, {nonNullable: true, validators: [Validators.required, this.selectedOption]}),
      KilometerLowerLimit: new FormControl<number>(0, {nonNullable: true, validators: [Validators.required]}),
      KilometerUpperLimit: new FormControl<number>(0, {nonNullable:true, validators: [Validators.required, this.isZeroValidation] }),
      IdProgram: new FormControl<number>(0, {nonNullable: true}),
      Services: new FormControl<boolean>(true, {nonNullable: true}),
      TypeWarranty: new FormControl<string>('', {nonNullable: true}),
      CrudHistoryList: new FormArray<FormGroup<CrudHistoryFormItem>>(
        [
          new FormGroup<CrudHistoryFormItem>(
            {
              Date: new FormControl<string>(new Date().toISOString(), {nonNullable: true}),
              Comments: new FormControl<string>('Creación del producto', {nonNullable: true}),
              IdUser: new FormControl<number>(1, {nonNullable: true}),
              Type: new FormControl<string>('C', {nonNullable: true})
            }
          )
          
        ]
      ),
      Template: new FormControl<string>('', {nonNullable: true}),
      Active: new FormControl<boolean>(true, { nonNullable: true}),
      RateForms: new FormArray<FormGroup<ProgramPercentagesFormItem>>([])
    }
  )

  constructor(
    private dialogRef: MatDialogRef<ModalCrudComponent>,
    @Inject(MAT_DIALOG_DATA) public data: ProductItem
  ) {}
  
  get formControls() { return this.productForm.controls }

  ngOnInit(): void {
    this.productForm.controls.IdProgram.setValue(this.data.IdProgram);
  }

  closeDialog(action: string): void {
  
    if (action === 'save') {
      if (this.productForm.valid) {
        this.data = this.productForm.getRawValue();
        this.dialogRef.close(this.data)
      } else {
        this.productForm.markAllAsTouched();
      }
    } else {
      this.dialogRef.close(null);
    }
  }


  selectedOption(control: AbstractControl): ValidationErrors | null {
    if (Number(control.value) === 0) {
      return {
        'IsNotSelected': 'Select an option'
      };
    }
    return null;
  }


  isZeroValidation(control: AbstractControl): ValidationErrors | null {
    if (Number(control.value) === 0) {
      return {
        'IsZero': true
      };
    }
    return null;
  }
}
