import React, { useState } from "react";
import apiConfig from "../../apiConfig";
import axios from "axios";
import TokenCache from "../TokenCache";
import { Navigate, useNavigate } from "react-router-dom";

const Login: React.FC<any> = () => {
  if (TokenCache.getToken() != null) {
    return <Navigate to="/ngd/partners" />;
  }

  let username: React.RefObject<HTMLInputElement> = React.createRef();
  let password: React.RefObject<HTMLInputElement> = React.createRef();
  const navigate = useNavigate();

  const handleClick = async () => {
    const usrName = username.current;
    const pass = password.current;
    console.log(usrName);
    console.log(pass);

    const resp = await axios.post(`${apiConfig.baseUrl}/keycrm/token`, {
      username: usrName?.value,
      password: pass?.value,
    });

    console.log(resp.data.token);
    TokenCache.setToken(resp.data.token);

    navigate(`/ngd/partners`);
  };

  return (
    <div className="flashcard">
      <input type="text" ref={username} />
      <input type="password" ref={password} />
      <br />
      <button onClick={handleClick}>Login</button>
    </div>
  );
};

export default Login;
