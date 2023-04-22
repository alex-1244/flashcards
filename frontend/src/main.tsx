import React from 'react'
import ReactDOM from 'react-dom/client'
import App from './App'
import './index.css'
import {
  createBrowserRouter,
  RouterProvider,
} from "react-router-dom";
import Frontpage from './Frontpage/Frontpage';
import FlashcardDeck from './Deck/FlashcardDeck';

const router = createBrowserRouter([
  {
    path: "/",
    element: <Frontpage />,
  },
  {
    path: "/:binId",
    element: <App />,
  },
]);

ReactDOM.createRoot(document.getElementById('root') as HTMLElement).render(
  <React.StrictMode>
     <RouterProvider router={router} />
  </React.StrictMode>,
)
