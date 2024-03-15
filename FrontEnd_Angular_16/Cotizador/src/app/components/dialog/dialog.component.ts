import { Component, Inject } from '@angular/core';
import { DialogDataItem } from 'src/app/models/dialog-data-item';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-dialog',
  templateUrl: './dialog.component.html',
  styleUrls: ['./dialog.component.scss']
})
export class DialogComponent {
  
  constructor(private dialogRef: MatDialogRef<DialogComponent, boolean>, @Inject(MAT_DIALOG_DATA) public data: DialogDataItem) {
  }

  close(result: boolean): void {
    this.dialogRef.close(result);
  }

}
