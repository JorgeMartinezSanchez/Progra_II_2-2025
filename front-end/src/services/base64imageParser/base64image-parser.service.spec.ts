import { TestBed } from '@angular/core/testing';

import { Base64imageParserService } from './base64image-parser.service';

describe('Base64imageParserService', () => {
  let service: Base64imageParserService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(Base64imageParserService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
