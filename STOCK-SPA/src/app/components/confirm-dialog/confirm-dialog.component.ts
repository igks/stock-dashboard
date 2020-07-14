import { Component } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'confirm-dialog',
  templateUrl: './confirm-dialog.component.html',
})
export class ConfirmDialogComponent {
  /**
   * Constructor
   *
   * @param {MatDialogRef<ConfirmDialogComponent>} dialogRef
   */
  constructor(public dialogRef: MatDialogRef<ConfirmDialogComponent>) {}

  cancel() {
    return this.dialogRef.close(false);
  }

  confirm() {
    return this.dialogRef.close(true);
  }
}
