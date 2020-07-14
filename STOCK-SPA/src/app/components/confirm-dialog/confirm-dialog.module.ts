import { NgModule } from "@angular/core";
import { MatButtonModule } from "@angular/material/button";
import { MatDialogModule } from "@angular/material/dialog";
import { ConfirmDialogComponent } from "./confirm-dialog.component";
import { MatIconModule } from "@angular/material/icon";

@NgModule({
    declarations: [ConfirmDialogComponent],
    imports: [MatDialogModule, MatButtonModule, MatIconModule],
    entryComponents: [ConfirmDialogComponent]
})
export class ConfirmDialogModule {}
