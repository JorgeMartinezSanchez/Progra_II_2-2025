import { HttpClient } from "@angular/common/http";

export abstract class APIdataReciever{
    protected apiUrl = 'http://localhost:5053';
    protected tokenKey = 'authToken';

    constructor(protected http: HttpClient) {}
}