import { ChatTest } from "./chatTest"
import { MessageTest } from "./messageTest"

export interface privateChatTest extends ChatTest{
    emmisorUser: string
    receptorUser: string
    messages: MessageTest[]
}