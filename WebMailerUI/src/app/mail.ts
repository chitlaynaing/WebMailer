export class Mail {
    constructor(
        public toemail: string,
        public fromemail: string,
        public subject: string,
        public content: string,
        public ccemail: string,
        public bccemail: string
    ) {}
}
