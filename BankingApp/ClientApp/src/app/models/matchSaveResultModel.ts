export class MatchSaveResultModel{
    public data : any;
    public errors : string[] = [];

    public getErrorMsg() : string{
       return this.errors?.reduce((a,b) => b + a, '');
    }
}