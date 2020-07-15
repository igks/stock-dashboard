import { Injectable } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { MatDialog } from '@angular/material/dialog';
import { ConfirmDialogComponent } from '../components/confirm-dialog/confirm-dialog.component';

@Injectable({
  providedIn: 'root',
})
export class AlertService {
  constructor(private toastr: ToastrService, private dialog: MatDialog) {}

  Success(title, message) {
    this.toastr.success(message, title);
  }

  Error(title, message) {
    this.toastr.error(message, title);
  }

  Warning(title, message) {
    this.toastr.warning(message, title);
  }

  Info(title, message) {
    this.toastr.info(message, title);
  }

  Confirm(
    confirmButton = 'Delete',
    confirmMessage = "The data will be delete and can't be undo!"
  ) {
    var dialogRef = this.dialog.open(ConfirmDialogComponent);
    dialogRef.componentInstance.confirmButton = confirmButton;
    dialogRef.componentInstance.confirmMessage = confirmMessage;
    return dialogRef;
  }
}
