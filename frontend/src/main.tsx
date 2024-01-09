import React from "react";
import ReactDOM from "react-dom/client";
import App from "./App";
import "./index.css";
import { createBrowserRouter, RouterProvider } from "react-router-dom";
import Frontpage from "./Frontpage/Frontpage";
import FlashcardDeck from "./Deck/FlashcardDeck";
import Login from "./Ngd/Login/Login";
import Partners from "./Ngd/Partners/Partners";
import PartnerDetails from "./Ngd/Partners/PartnerDetails";

const router = createBrowserRouter([
  {
    path: "/",
    element: <Frontpage />,
  },
  {
    path: "/ngd/login",
    element: <Login />,
  },
  {
    path: "/ngd/partners",
    element: <Partners />,
  },
  {
    path: "/ngd/partners/:id",
    element: <PartnerDetails />,
  },
  {
    path: "/:binId",
    element: <App />,
  },
]);

ReactDOM.createRoot(document.getElementById("root") as HTMLElement).render(
  <React.StrictMode>
    <RouterProvider router={router} />
  </React.StrictMode>
);
