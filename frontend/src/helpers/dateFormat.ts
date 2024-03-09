function toBackendFormat(date: string): string 
{

    return date.split("-").reverse().join("-");
}

const standardFormat = "YYYY-MM-DD";

export {toBackendFormat, standardFormat };