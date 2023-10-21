import { Injectable } from "@angular/core";
import { type } from "os";

export interface Menu{
    state: string;
    name: string;
    type: string;
    icon: string;
    role: string;
}

const MENUITEMS =[
    {state:'dashboard', name:'Dashboard', type:'link', icon:'dashboard',role:''},
    {state:'category', name:'Quản lý phân loại', type:'link', icon:'category',role:'admin'},
    {state:'product', name:'Quản lý sản phẩm', type:'link', icon:'inventory',role:'admin'},
    {state:'order', name:'Quản lý đơn hàng', type:'link', icon:'shopping_cart',role:''},
    {state:'bill', name:'Xem hóa đơn', type:'link', icon:'backup_table',role:''},
    {state:'statistic', name:'Xem thống kê', type:'link', icon:'bar_chart',role:'admin'}
]

@Injectable()
export class MenuItems{
    getMenuItem():Menu[]{
        return MENUITEMS;
    }
}