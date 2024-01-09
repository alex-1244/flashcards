const TOKEN_CACHE = "TOKEN_CACHE";
const ONE_DAY = 1000 * 60 * 60 * 24;

const currentTime = () => {
  return Date.now();
};

const getToken = (): string | null => {
  let tokenCache = {
    token: null,
    nextCleanup: new Date().getTime() + ONE_DAY,
  };

  try {
    const data = localStorage.getItem(TOKEN_CACHE);

    if (data) {
      tokenCache = JSON.parse(data);
    }
  } catch (e: any) {
    console.error(e.message);
  }

  if (tokenCache.nextCleanup < currentTime()) {
    return null;
  }

  return tokenCache.token;
};

const setToken = (token: string) => {
  let tokenCache = {
    token: token,
    nextCleanup: new Date().getTime() + ONE_DAY,
  };

  try {
    localStorage.setItem(TOKEN_CACHE, JSON.stringify(tokenCache));
  } catch (e) {
    localStorage.removeItem(TOKEN_CACHE);
  }
};

const getEmail = (partnerId: number) => {
  const data = localStorage.getItem(`partner_email_${partnerId}`);

  return data;
};

const setEmail = (partnerId: number, email: string) => {
  localStorage.setItem(`partner_email_${partnerId}`, email);
};

export default {
  getToken,
  setToken,
  getEmail,
  setEmail,
};
