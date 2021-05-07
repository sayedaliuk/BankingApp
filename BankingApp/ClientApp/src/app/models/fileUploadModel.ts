export class FileUploadModel{
    public file : File;
    public progress : number;
    public errors : string[] = [];
    public info : string;
    public data : any;

    public getForm() : FormData {
        let fd = new FormData();
        fd.append(this.file.name, this.file, this.file.name);
        return fd;
    }

    public getErrorMsg() : string{
       return this.errors?.reduce((a,b) => b + a, '');
    }
}