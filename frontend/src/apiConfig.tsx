const API_URL = import.meta.env.MODE === "development" ? 'http://localhost:5244' : 'http://flashxcards-load-balancer-1208812410.eu-west-1.elb.amazonaws.com';

export default
{
    baseUrl: API_URL
}