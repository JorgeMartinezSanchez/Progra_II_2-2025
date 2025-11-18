import { ChatTest } from "./chatTest"
import { MessageTest } from "./messageTest"

export interface GroupTest extends ChatTest{
    emmisorUser: string
    members: string[];
    messages: MessageTest[]
}