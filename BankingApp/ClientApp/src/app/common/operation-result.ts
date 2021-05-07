export class OperationResult {
    success: boolean;
    messages: string[] = [];

    constructor(success : boolean, messages : string[] = null){
        this.success = success;
        if(messages != null) {
            messages.forEach(x => this.messages.push(x));
        }
    }
}

export class ApiLoginResult extends OperationResult {
    tokenKey: string = null;
    tokenValue: string = null;

    constructor(success : boolean, tokenKey: string = null, tokenValue: string = null, messages : string[] = null)         
    {
        super(success, messages);
        this.tokenKey = tokenKey;
        this.tokenValue = tokenValue;
    }
}