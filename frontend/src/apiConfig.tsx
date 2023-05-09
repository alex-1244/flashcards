const API_URL = import.meta.env.MODE === "development" ? 'http://localhost:5244' : 'https://hp7nr7ovbmqe42ognx7wbnjncu0hzkba.lambda-url.eu-west-1.on.aws';

export default
{
    baseUrl: API_URL
}