import { MessageTest } from "./messageTest";

// chatTest.ts
export interface ChatTest {
    chatId: string;
    name: string;
    lastMessage?: string;
    isEncrypted: boolean;
    type: 'private' | 'group';
    avatar?: string;
    // Para chats privados
    emisorUser?: string;
    receptorUser?: string;
    // Para grupos
    members?: string[];
    messages: MessageTest[];
}