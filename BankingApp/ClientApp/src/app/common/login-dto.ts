export class LoginDto {
    username : string;
    password : string;
}

export class LoginSummaryDto {
    username: string;
    lastLoggedIn? : Date;
    lastUpdated? : Date;
}