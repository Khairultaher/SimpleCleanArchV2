import React, { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import { BrowserRouter, Routes, Route } from "react-router-dom";
import { Provider } from "react-redux";
import store from "./redux/store";
import { AuthProvider } from "./context/AuthProvider";
import { positions, transitions, Provider as AlertProvider } from "react-alert";
import AlertTemplate from "react-alert-template-basic";

import "./index.css";
import App from "./App";

import reportWebVitals from "./reportWebVitals";

const options = {
  timeout: 5000,
  position: positions.BOTTOM_CENTER,
  transition: transitions.SCALE,
};

const rootElement = document.getElementById("root");
const root = createRoot(rootElement);

root.render(
  <StrictMode>
    <BrowserRouter>
      <Provider store={store}>
        <AuthProvider>
          <AlertProvider template={AlertTemplate} {...options}>
            <Routes>
              <Route path="/*" element={<App />} />
            </Routes>
          </AlertProvider>
        </AuthProvider>
      </Provider>
    </BrowserRouter>
  </StrictMode>
);

reportWebVitals();
