import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { AppState } from '../../app/interfaces/appState';
import { RecieveAccount } from '../../app/interfaces/receiveAccount';

// Estado inicial vacío
const INITIAL_STATE: AppState = {
  currentUser: null as any,
  contacts: new Map(),
  sessionKeys: undefined
};

@Injectable({
  providedIn: 'root'
})
export class StateService {

  // 1. La fuente de la verdad (privada)
  // BehaviorSubject retiene el último valor emitido.
  private _state = new BehaviorSubject<AppState>(INITIAL_STATE);

  // 2. El Observable público (para que los componentes se suscriban)
  // Usamos '$' al final por convención de Observables
  public state$: Observable<AppState> = this._state.asObservable();

  constructor() { }

  // --- GETTERS (Para obtener el valor actual sin suscribirse) ---
  
  // Obtener todo el estado de golpe (síncrono)
  get currentState(): AppState {
    return this._state.getValue();
  }

  // Helper para ver si estamos logueados y con llaves listas
  get isCryptoReady(): boolean {
    const s = this.currentState;
    return !!s.currentUser && !!s.sessionKeys?.userPrivateKey;
  }

  // --- ACTIONS (Métodos para modificar el estado) ---

  // Acción: Usuario se logueó exitosamente
  setLoginSuccess(user: RecieveAccount, keys: { userPrivateKey: CryptoKey, derivedPasswordKey?: CryptoKey }) {
    const newState: AppState = {
      ...this.currentState, // Mantenemos lo que hubiera (ej. contactos cacheados)
      currentUser: user,
      sessionKeys: keys
    };
    this._state.next(newState); // Emitimos el cambio a toda la app
  }

  // Acción: Cerrar sesión (Borrado seguro de memoria)
  clearSession() {
    this._state.next(INITIAL_STATE);
  }

  // Acción: Actualizar lista de contactos
  updateContacts(newContacts: any[]) { 
    // Aquí podrías transformar el array a Map si tu interfaz lo requiere
    // Por simplicidad, asumo que manejas la lógica de mapeo aquí o en el componente
    // ...
  }
}
