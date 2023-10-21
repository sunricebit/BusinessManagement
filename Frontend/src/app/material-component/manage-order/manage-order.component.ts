import { Component, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { SnackbarService } from 'src/app/services/snackbar.service';
import { BillService } from 'src/app/services/bill.service';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { GlobalContants } from 'src/app/shared/global-constants';
import { ProductService } from 'src/app/services/product.service';
import { CategoryService } from 'src/app/services/category.service';
import { saveAs } from 'file-saver';
import { MatDialog, MatDialogConfig } from '@angular/material/dialog';
import { QrcodeService } from 'src/app/services/qrcode.service';
import { ViewQrcodeComponent } from '../dialog/view-qrcode/view-qrcode.component';
import { Router } from '@angular/router';

@Component({
  selector: 'app-manage-order',
  templateUrl: './manage-order.component.html',
  styleUrls: ['./manage-order.component.scss']
})
export class ManageOrderComponent implements OnInit {

  displayedColumns: string[]  = ['name', 'category', 'price', 'quantity', 'total', 'edit'];
  dataSource:any = [];
  manageOrderForm:any;
  categories: any = [];
  products:any = [];
  price:any;
  totalAmount:number = 0;
  responseMessage:any;

  constructor(private formBuilder:FormBuilder,
    private categoryService:CategoryService,
    private productService:ProductService,
    private snackbarService:SnackbarService,
    private billService:BillService,
    private genQrService:QrcodeService,
    private router: Router,
    private dialog: MatDialog,
    private ngxService:NgxUiLoaderService,) { }

  ngOnInit(): void {
    this.ngxService.start();
    this.getCategories();
    this.manageOrderForm = this.formBuilder.group({
      name:[null, [Validators.required, Validators.pattern(GlobalContants.nameRegex)]],
      email:[null, [Validators.required, Validators.pattern(GlobalContants.emailRegex)]],
      contactNumber:[null, [Validators.required, Validators.pattern(GlobalContants.contactNumberRegex)]],
      paymentMethod:[null, [Validators.required]],
      product:[null, [Validators.required]],
      category:[null, [Validators.required]],
      quantity:[null, [Validators.required]],
      price:[null, [Validators.required]],
      total:[0, [Validators.required]],
    });
  }

  getCategories(){
    this.categoryService.getCategories().subscribe((response:any) =>{
      this.ngxService.stop();
      this.categories = response;
    }, (error:any) =>{
      this.ngxService.stop();
      console.log(error);
      if(error.error?.message){
        this.responseMessage = error.error?.message;
      }else{
        this.responseMessage = GlobalContants.genericError;
      }
      this.snackbarService.openSnackBar(this.responseMessage, GlobalContants.error);
    })
  }

  getProductsByCategory(value:any){
    this.productService.getProductsByCategory(value.id).subscribe((response:any) =>{
      this.products = response;
      this.manageOrderForm.controls['price'].setValue('');
      this.manageOrderForm.controls['quantity'].setValue('');
      this.manageOrderForm.controls['total'].setValue(0);
    }, (error:any) =>{
      this.ngxService.stop();
      console.log(error);
      if(error.error?.message){
        this.responseMessage = error.error?.message;
      }else{
        this.responseMessage = GlobalContants.genericError;
      }
      this.snackbarService.openSnackBar(this.responseMessage, GlobalContants.error);
    })
  }

  getProductDetails(value:any){
    this.productService.getById(value.id).subscribe((response:any) => {
      this.price = response.price;
      this.manageOrderForm.controls['price'].setValue(response.price);
      this.manageOrderForm.controls['quantity'].setValue('1');
      this.manageOrderForm.controls['total'].setValue(this.price * 1);
    }, (error:any) =>{
      this.ngxService.stop();
      console.log(error);
      if(error.error?.message){
        this.responseMessage = error.error?.message;
      }else{
        this.responseMessage = GlobalContants.genericError;
      }
      this.snackbarService.openSnackBar(this.responseMessage, GlobalContants.error);
    })
  }

  setQuantity(value:any){
    var temp = this.manageOrderForm.controls['quantity'].value;
    if(temp > 0){
      this.manageOrderForm.controls['total'].setValue(this.manageOrderForm.controls['quantity'].value * this.manageOrderForm.controls['price'].value);
    }
    else if(temp != ''){
      this.manageOrderForm.controls['quantity'].setValue('1');
      this.manageOrderForm.controls['total'].setValue(this.manageOrderForm.controls['quantity'].value * this.manageOrderForm.controls['price'].value);
    }
  }

  validateProductAdd(){
    if(this.manageOrderForm.controls['total'].value === 0 || this.manageOrderForm.controls['total'].value === null ||
    this.manageOrderForm.controls['quantity'].value <= 0){
      return true;
    }
    else return false;
  }

  validateSubmit(){
    if(this.totalAmount === 0 
      || this.manageOrderForm.controls['name'].value === null 
      || this.manageOrderForm.controls['email'].value === null 
      || this.manageOrderForm.controls['contactNumber'].value === null
      || this.manageOrderForm.controls['paymentMethod'].value === null){
        return true;
      }else {
        return false;
      }
  }

  add(){
    var formData = this.manageOrderForm.value;
    var productName = this.dataSource.find((e: {id:number}) => e.id === formData.product.id);
    if(productName === undefined){
      this.totalAmount = this.totalAmount + formData.total;
      this.dataSource.push({id:formData.product.id, name:formData.product.name, category:formData.category.name, quantity:formData.quantity, price:formData.price, total:formData.total});
      this.dataSource = [...this.dataSource];
      this.snackbarService.openSnackBar(GlobalContants.productAdded, "success");
    }
    else{
      this.snackbarService.openSnackBar(GlobalContants.productExistError, GlobalContants.error);
    } 
  }

  handleDeleteAction(value:any, element:any){
    this.totalAmount =this.totalAmount - element.total;
    this.dataSource.splice(value,1);
    this.dataSource = [...this.dataSource];
  }

  submitAction(){
    var formData = this.manageOrderForm.value;
    var data = {
      name: formData.name,
      email: formData.email,
      contactNumber:formData.contactNumber,
      paymentMethod: formData.paymentMethod,
      totalAmount: this.totalAmount.toString(),
      productDetails: JSON.stringify(this.dataSource)
    }

    if (formData.paymentMethod == 'QRCode'){
      var qrData ={
        accountNo: '8850193456666',
        accountName: 'PHAM QUOC HUNG',
        acqId: '970422',
        addInfo: 'KHACK HANG THANH TOAN',
        amount: this.totalAmount,
        template: 'compact'
      }

      this.ngxService.start();
      this.genQrService.getQrBase64(qrData).subscribe((response: any) =>{
        const dialogConfig = new MatDialogConfig();
        dialogConfig.data = {
          imageUrl: response.data.qrDataURL
        };

        dialogConfig.width = "100%";
        const dialogRef = this.dialog.open(ViewQrcodeComponent, dialogConfig);
        this.router.events.subscribe(() =>{
          dialogRef.close();
        })
      },(error:any) =>{
        this.ngxService.stop();
        console.log(error);
        if(error.error?.message){
          this.responseMessage = error.error?.message;
        }else{
          this.responseMessage = GlobalContants.genericError;
        }
        this.snackbarService.openSnackBar(this.responseMessage, GlobalContants.error);
      })
    }

    this.ngxService.start();
    this.billService.generateReport(data).subscribe((response:any) =>{
      this.downloadFile(response.uuid);
      this.manageOrderForm.reset();
      this.dataSource =[];
      this.totalAmount = 0;
    }, (error:any) =>{
      this.ngxService.stop();
      console.log(error);
      if(error.error?.message){
        this.responseMessage = error.error?.message;
      }else{
        this.responseMessage = GlobalContants.genericError;
      }
      this.snackbarService.openSnackBar(this.responseMessage, GlobalContants.error);
    })
  }

  downloadFile(fileName:string){
    var data = {
      uuid:fileName
    }

    this.billService.getPdf(data).subscribe((response:any) =>{
      saveAs(response, fileName + '.pdf');
      this.ngxService.stop();
    })
  }

  generateQr(data:any){

  }
}
