import { BrowserRouter, HashRouter, Routes, Route } from "react-router-dom";
import { useDispatch, useSelector } from "react-redux";
import { useAlert } from "react-alert";
import RequireAuth from "./components/auth/RequireAuth";

import Layout from "./components/layout/Layout";
import Header from "./components/layout/Header";
import Menu from "./components/layout/Menu";
import Footer from "./components/layout/Footer";
import Dashboard from "./components/Dashboard";
import WeatherForecast from "./components/WeatherForecast/WeatherForecast";
import Login from "./components/auth/Login";
import Logout from "./components/auth/Logout";
import Missing from "./components/Missing";
import Test from "./components/Test";
import Unauthorized from "./components/auth/Unauthorized";
import PersistLogin from "./components/auth/PersistLogin";

import "./App.css";

const ROLES = {
  User: "User",
  Editor: "Editor",
  Admin: "Admin",
};

function App() {
  const { loading, isAuthenticated, user, error } = useSelector(
    (state) => state.auth
  );
  return (
    <div className="wrapper">
      <Header />
      <Menu />

      <section className={"content-wrapper"}>
        <Routes>
          <Route path="/" element={<Layout />}>
            {/*public routes*/}
            <Route path="/login" element={<Login />} />
            <Route path="/Logout" element={<Logout />} />

            {/*Protected routes & Persist login*/}
            <Route element={<PersistLogin />}>
              <Route
                element={
                  <RequireAuth allowedRoles={[ROLES.User, ROLES.Admin]} />
                }
              >
                <Route path="/" element={<Dashboard />} />
                <Route path="/WeatherForecast" element={<WeatherForecast />} />
              </Route>
              <Route element={<RequireAuth allowedRoles={[ROLES.Admin]} />}>
                <Route path="/Test" element={<Test />} />
              </Route>
            </Route>
            {/*catch all*/}
            <Route path="/Missing" element={<Missing />} />
            <Route path="/unauthorized" element={<Unauthorized />} />
          </Route>
        </Routes>
      </section>
      {<Footer />}
    </div>
  );
}

export default App;
