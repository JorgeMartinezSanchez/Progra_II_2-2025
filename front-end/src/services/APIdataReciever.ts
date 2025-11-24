import { HttpClient } from "@angular/common/http";

export abstract class APIdataReciever{
    protected apiUrl: string;
    protected tokenKey = 'authToken';

    constructor(protected http: HttpClient, protected areaUrl: string) {
        this.apiUrl = `http://localhost:5053/${areaUrl}`;
    }
}