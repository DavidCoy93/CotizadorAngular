import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { LoginResponse, User } from '../models/user-item';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { MenuItem } from '../models/menu-item';
import { ProgramItem } from '../models/program-item';
import { ProductItem } from '../models/product-item';
import { RateItem } from '../models/rate-item';
import { ClassItem } from '../models/class-item';

@Injectable({
  providedIn: 'root'
})
export class GlobalAutomotiveService {

  constructor(private http: HttpClient) { }


  LoginAccess(userData: User): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${environment.UrlAPI}/Acceso`, userData);
  }

  GetMenu(IdRole: number): Observable<Array<MenuItem>> {
    const parameters = new HttpParams( {fromObject: { 'IdRole': IdRole }} )
    return this.http.get<Array<MenuItem>>(`${environment.UrlAPI}/GetMenuByRole`, { params: parameters });
  }

  GetPrograms(): Observable<Array<ProgramItem>> {
    return this.http.get<Array<ProgramItem>>(`${environment.UrlAPI}/GetPrograms`);
  }

  GetProducts(IdProgram: number): Observable<Array<ProductItem>> {
    return this.http.get<Array<ProductItem>>(`${environment.UrlAPI}/GetProductByIdProgram/${IdProgram}`)
  }

  getRateList(IdProgram: number | null, IdProduct: number): Observable<Array<RateItem>> {
    return this.http.get<Array<RateItem>>(`${environment.UrlAPI}/GetCotizacion/${IdProgram}/${IdProduct}`)
  }

  getClasses(): Observable<ClassItem> {
    return this.http.get<ClassItem>(`${environment.UrlAPI}/GetClasses`);
  }

  InsertCotizacion(program: ProgramItem): Observable<any> {
    return this.http.post(`${environment.UrlAPI}/InsertCotizacion`, program)
  }
}
