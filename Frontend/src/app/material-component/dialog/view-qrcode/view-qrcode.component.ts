import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-view-qrcode',
  templateUrl: './view-qrcode.component.html',
  styleUrls: ['./view-qrcode.component.scss']
})
export class ViewQrcodeComponent implements OnInit {

  qrCodeUrl: any;

  constructor(@Inject(MAT_DIALOG_DATA) public dialogData:any,
    public dialogRef: MatDialogRef<ViewQrcodeComponent>) { }

  ngOnInit(): void {
    this.qrCodeUrl = this.dialogData.imageUrl;
  }

}
