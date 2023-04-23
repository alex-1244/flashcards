const API_URL = import.meta.env.MODE === "development" ? 'http://localhost:5244' : 'https://flashcardmate.net';

export default
{
    baseUrl: API_URL
}