import { TestBed } from '@angular/core/testing';

import { EncryptationService } from './encryptation.service';

describe('EncryptationService', () => {
  let service: EncryptationService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(EncryptationService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
