export class Result<T> {
    public errors : string[];
    public info : string;
    public success : boolean;
    public data : T;
}
