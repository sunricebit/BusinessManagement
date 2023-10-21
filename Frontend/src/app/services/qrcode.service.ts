import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class QrcodeService {

  qrUrl = environment.qrUrl;

  constructor(private httpClient: HttpClient) {
   }

  getQrBase64(data:any){
    return this.httpClient.post(this.qrUrl, data, {
      headers: new HttpHeaders().set('Content-Type', 'application/json')
    });
  }
}
