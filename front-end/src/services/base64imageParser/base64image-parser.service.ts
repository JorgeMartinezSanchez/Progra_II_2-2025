import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class Base64imageParserService {

  constructor() { }

  public fileToBase64(file: File): Promise<string> {
    return new Promise((resolve, reject) => {
      const reader = new FileReader();
      reader.readAsDataURL(file);
      
      reader.onload = () => {
        resolve(reader.result as string);
      };
      
      reader.onerror = (error) => {
        reject(error);
      };
    });
  }

  public base64ToBlob(base64Data: string, contentType: string = 'image/png', sliceSize: number = 512): Blob {
    // A veces el base64 viene con la cabecera "data:image/png;base64,", hay que limpiarla si es el caso
    const base64Content = base64Data.includes(',') 
      ? base64Data.split(',')[1] 
      : base64Data;

    const byteCharacters = window.atob(base64Content);
    const byteArrays = [];

    for (let offset = 0; offset < byteCharacters.length; offset += sliceSize) {
      const slice = byteCharacters.slice(offset, offset + sliceSize);

      const byteNumbers = new Array(slice.length);
      for (let i = 0; i < slice.length; i++) {
        byteNumbers[i] = slice.charCodeAt(i);
      }

      const byteArray = new Uint8Array(byteNumbers);
      byteArrays.push(byteArray);
    }

    return new Blob(byteArrays, { type: contentType });
  }

  public base64ToFile(base64Data: string, filename: string): File {
    let mimeType = 'image/png';
    if (base64Data.includes('data:')) {
        mimeType = base64Data.split(';')[0].split(':')[1];
    }

    const blob = this.base64ToBlob(base64Data, mimeType);
    
    return new File([blob], filename, { type: mimeType });
  }
}
