import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../environments/environment';
import { Content } from '../models/Content';

@Injectable({
  providedIn: 'root'
})
export class ContentService {
  constructor(private _http: HttpClient) { }

  getContent(): Observable<Content[]> {
    console.log("content service :", environment.baseURL);
    return this._http.get<Content[]>(environment.baseURL + "content");
  }

  updateContent(content: Content): Observable<any> {
    console.log("content service :", content);
    return this._http.post(environment.baseURL + "content/" + content.Id, content);
  }

  createContent(content: Content): Observable<any> {
    console.log("content service :", content);
    return this._http.post(environment.baseURL + "content", content);
  }

  deleteContent(Id: number): Observable<any> {
    console.log("content service :", Id);
    return this._http.delete(environment.baseURL + "content/" + Id);
  }
}
