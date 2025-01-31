export class SetUser {
    static readonly type = '[User] Set User';
    constructor(public username: string, public roles: string[]) {}
  }
  
  export class ClearUser {
    static readonly type = '[User] Clear User'
  }